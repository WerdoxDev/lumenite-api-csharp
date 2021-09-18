using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LumeniteApiCsharp
{
    public class Client
    {
        public List<BaseDevice> Devices { get; private set; }

        public Client()
        {
        }

        public async Task Login(GatewayOptions options)
        {
            GatewayConnection gateway = new(options);
            await gateway.Connect();
            Devices = gateway.Devices;
        }

        public T GetDeviceById<T>(int id) where T : BaseDevice
        {
            return Devices.Where(x => x.Id == id).Cast<T>().FirstOrDefault();
        }
    }

    public record GatewayOptions(string Url, int Port,bool Ssl, string Username, string Password);
}