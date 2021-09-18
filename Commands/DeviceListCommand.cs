using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeniteApiCsharp.Commands
{
    class DeviceListCommand : Command
    {
        public override void Action()
        {
            if (client.Devices.Count < 1)
            {
                Utils.Print("List is Empty", ConsoleColor.Yellow);
                return;
            }

            foreach (var deviceID in client.Devices)
            {
                Utils.Print(deviceID.Id.ToString(), ConsoleColor.Yellow);
            }
        }

        public override void Setup(string[] arguments)
        {
            name = "devicelist";
            this.arguments = arguments;
        }

        public DeviceListCommand(Client client)
        {
            this.client = client;
        }
    }
}
