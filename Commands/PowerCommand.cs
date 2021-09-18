using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeniteApiCsharp.Commands
{
    class PowerCommand : Command
    {
        public override void Action()
        {
            bool valid = int.TryParse(arguments[1], out int targetID);

            if (valid)
            {
                if (client.Devices.Find(x => x.Id == targetID) == null) //test
                {
                    Utils.Print($"Device With ID {targetID} Not Found.", ConsoleColor.Red);
                    return;
                }

                try //test
                {
                    if (arguments.Length >= 3)
                    {
                        if (arguments[2] == "on" || arguments[2] == "1") client.Devices[targetID].SetPower(1);
                        else if (arguments[2] == "off" || arguments[2] == "0") client.Devices[targetID].SetPower(0);
                        else
                        {
                            Utils.Print("INVALID SYNTAX.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        client.Devices[targetID].TogglePower();
                    }
                }
                catch
                {
                    Utils.Print("UNKNOWN ERROR.", ConsoleColor.Red);
                }
            }
            else
            {
                Utils.Print("INVALID ID.", ConsoleColor.Red);
            }
        }

        public override void Setup(string[] arguments)
        {
            name = "power";
            this.arguments = arguments;
        }

        public PowerCommand(Client client)
        {
            this.client = client;
        }
    }
}
