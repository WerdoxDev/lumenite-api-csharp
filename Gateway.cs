using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace LumeniteApiCsharp
{
    public class GatewayConnection
    {
        public static GatewayConnection Instance;
        readonly GatewayOptions _options;
        public readonly List<BaseDevice> Devices = new();
        ClientConfiguration _config;
        public MqttClient Client;
        string _id;

        int _connectedClients = 0, _connectedModules = 0;

        bool _isReady;

        public GatewayConnection(GatewayOptions options)
        {
            _options = options;
            _id = Utils.GenerateRandomId();
            Instance = this;
        }

        public async Task Connect()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connecting...");
            Client = new MqttClient(_options.Url, _options.Port, _options.Ssl, MqttSslProtocols.None, null, null);
            Client.Connect($"csharp_api_{_id}", _options.Username, _options.Password, false, 0, true, "client/offline", _id, true, 15);
            if(Client.IsConnected) Console.WriteLine("Connected!");
            await DefaultSubscribe();
        }

        private async Task DefaultSubscribe()
        {
            Console.WriteLine("Initializing...");
            Client.Subscribe(
                new[] {$"client/{_id}/set-connected", $"client/{_id}/initialize", "server/connect", "server/offline"},
                new[]
                {
                    MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE
                });
            Client.MqttMsgPublishReceived += MqttMsgPublishReceived;
            Client.Publish("client/connect", Encoding.Default.GetBytes(_id));
            await Task.Run(async () =>
            {
                while (!_isReady) await Task.Delay(25);
            });
            
            Console.WriteLine("Done!");
        }

        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.Default.GetString(e.Message);
            if (Utils.CheckTopic(e.Topic, "server/connect"))
            {
                Client.Publish("client/connect", Encoding.Default.GetBytes(_id));
            }
            else if (Utils.CheckTopic(e.Topic, "server/offline"))
            {
            }
            else if (Utils.CheckTopic(e.Topic, "client/initialize", 1))
            {
                ClientInitializePayload payload = JsonConvert.DeserializeObject<ClientInitializePayload>(message);
                ClientInitialize(payload);
            }
            else if (Utils.CheckTopic(e.Topic, "client/set-connected", 1))
            {
                int[] payload = JsonConvert.DeserializeObject<int[]>(message);
                ClientSetConnected(payload);
            }
            else if (Utils.CheckTopic(e.Topic, "module/client/set-devices", 1, 3))
            {
                ClientSetDevicesPayload[] payload = JsonConvert.DeserializeObject<ClientSetDevicesPayload[]>(message);
                ModuleClientSetDevices(payload);
                _isReady = true;
            }
            else if (Utils.CheckTopic(e.Topic, "module/execute-client-command", 1))
            {
                Command command = JsonConvert.DeserializeObject<Command>(message);
                OutputDevice device = Devices.Where(x => command != null && x.Id == command.DeviceId)
                    .Cast<OutputDevice>().FirstOrDefault();
                if (device != null) device.ExecuteCommand(command);
            }
        }

        private void ClientInitialize(ClientInitializePayload payload)
        {
            payload.Devices.ToList().ForEach(x =>
            {
                if (x.Type == DeviceType.RgbLight) return;
                else Devices.Add(x);
            });
            _config = payload.Config;
            _config.RegisteredModuleTokens.ToList().ForEach(x =>
            {
                Client.Subscribe(
                    new[]
                    {
                        $"module/{x}/execute-client-command", $"module/{x}/device-settings-changed",
                        $"module/{x}/client/{_id}/set-devices"
                    },
                    new[]
                    {
                        MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                        MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE
                    });
            });
            Client.Publish($"client/{_id}/initialize-finished", Encoding.Default.GetBytes(""));
        }

        private void ClientSetConnected(int[] payload)
        {
            _connectedClients = payload[0];
            _connectedModules = payload[1];
        }

        private void ModuleClientSetDevices(ClientSetDevicesPayload[] payload)
        {
            Devices.ForEach(x =>
            {
                int index = payload.ToList().FindIndex(y => y.Id == x.Id);
                x.Status = new DeviceStatus(Utils.EmptyStatus(), payload[index].Status?.CurrentStatus,
                    Utils.EmptyStatus());
            });
        }

        record ClientConfiguration(string[] RegisteredModuleTokens);

        record ClientInitializePayload(OutputDevice[] Devices, ClientConfiguration Config);

        record ClientSetDevicesPayload(int Id, DeviceStatus Status);
    }
}