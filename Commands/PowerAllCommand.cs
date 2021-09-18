using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeniteApiCsharp.Commands
{
    class PowerAllCommand : Command
    {
        public override void Action()
        {
            foreach (var device in client.Devices)
            {
                if (client.Devices.Find(x => x.Id == device.Id) == null)
                {
                    Utils.Print($"Device With ID {device.Id} Not Found.", ConsoleColor.Red);
                    return;
                }

                try
                {
                    if (arguments.Length >= 2)
                    {
                        if (arguments[1] == "on" || arguments[1] == "1") client.Devices[device.Id].SetPower(1);
                        else if (arguments[1] == "off" || arguments[1] == "0") client.Devices[device.Id].SetPower(0);
                        else
                        {
                            Utils.Print("INVALID SYNTAX.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        client.Devices[device.Id].TogglePower();
                    }
                }
                catch
                {
                    Utils.Print("UNKNOWN ERROR.", ConsoleColor.Red);
                }
            }
        }

        public override void Setup(string[] arguments)
        {
            this.arguments = arguments;
            name = "powerall";
        }

        public PowerAllCommand(Client client)
        {
            this.client = client;
        }
    }
}
