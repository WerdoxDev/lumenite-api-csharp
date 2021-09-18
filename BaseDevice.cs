using System;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LumeniteApiCsharp
{
    public class BaseDevice
    {
        public DeviceConfiguration Config;
        public int Id { get; }
        public string Name;
        public DeviceStatus Status;
        public DeviceType Type;

        public BaseDevice(int id, string name, DeviceType type, DeviceConfiguration config, DeviceStatus status = null)
        {
            Id = id;
            Name = name;
            Type = type;
            Status = status;
            Config = config;
        }

        public void ExecuteCommand(Command command)
        {
            if (command.Id == CommandType.PowerChanged)
            {
                CurrentStatus.Power = JsonConvert.DeserializeObject<Status>(command.Payload[0]).Power;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Power of device with id of {Id}, changed to {CurrentStatus.Power}");
                Console.ResetColor();
            }
        }

        public void SetPower(int power)
        {
            FutureStatus.Power = power;
            (CurrentStatus, LastStatus) = (LastStatus, CurrentStatus);

            Command command = new(CommandType.Power, Id,
                new[] {JsonConvert.SerializeObject(FutureStatus), JsonConvert.SerializeObject(LastStatus)});
            GatewayConnection.Instance.Client.Publish($"module/{Config.ModuleToken}/execute-command",
                Encoding.Default.GetBytes(JsonConvert.SerializeObject(command)));
        }

        public void TogglePower()
        {
            int power = CurrentStatus.Power == 1 ? 0 : 1;
            SetPower(power);
        }

        public Status FutureStatus
        {
            get => Status.FutureStatus;
            set => Status.FutureStatus = value;
        }

        public Status CurrentStatus
        {
            get => Status.CurrentStatus;
            set => Status.CurrentStatus = value;
        }

        public Status LastStatus
        {
            get => Status.LastStatus;
            set => Status.LastStatus = value;
        }
    }

    public class OutputDevice : BaseDevice
    {
        public OutputSettings Settings;

        public OutputDevice(int id, string name, DeviceType type, DeviceConfiguration config, OutputSettings settings,
            DeviceStatus status = null) : base(id, name, type, config, status)
        {
            Settings = settings;
        }
    }

    public record OutputSettings(AutomaticTiming[] AutomaticTimings, int TimeoutTime);

    public record AutomaticTiming(int Id, string Name, AutomaticDate[] Dates, AutomaticWeekday[] Weekdays);

    public record AutomaticDate(int Year, int Month, int Date);

    public record AutomaticWeekday(int Day);

    public record DeviceStatus
    {
        public Status FutureStatus { get; set; }
        public Status CurrentStatus { get; set; }
        public Status LastStatus { get; set; }

        public DeviceStatus(Status futureStatus, Status currentStatus, Status lastStatus)
        {
            FutureStatus = futureStatus;
            CurrentStatus = currentStatus;
            LastStatus = lastStatus;
        }
    };

    public record DeviceConfiguration(PinConfiguration PinConfig, string ModuleToken, int[] ValidCommands);

    public record PinConfiguration(int Pin, int PinCheck = -1);

    public record Status
    {
        [JsonProperty(PropertyName = "power")] public int Power { get; set; }

        public Status(int power)
        {
            Power = power;
        }
    }

    public record Command
    {
        [JsonProperty(PropertyName = "id")] public int Id;

        [JsonProperty(PropertyName = "deviceId")]
        public  int DeviceId;

        [JsonProperty(PropertyName = "payload")]
        public  string[] Payload;

        public Command(int id, int deviceId, string[] payload)
        {
            Id = id;
            DeviceId = deviceId;
            Payload = payload;
        }
    }

    public enum DeviceType
    {
        None,
        OUTPUT_DEVICE,
        InputDevice,
        RgbLight,
        TemperatureSensor
    }

    public class StatusType
    {
        public static int Offline = -2;
        public static int Processing = -1;
        public static int Off = 0;
        public static int On = 1;
    }

    public class CommandType
    {
        public static int Power = 0;
        public static int PowerChanged = 1;
    }
}