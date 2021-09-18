using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeniteApiCsharp.Commands
{
    class HelpCommand : Command
    {
        public override void Action()
        {
            Utils.Print($"Type '{Program.prefix}' before any command.", ConsoleColor.Yellow);
            Utils.Print("DeviceList", ConsoleColor.Yellow);
            Utils.Print("Power <deviceID> [On|Off|1|0]", ConsoleColor.Yellow);
            Utils.Print("PowerAll [On|Off|1|0]", ConsoleColor.Yellow);
            Utils.Print("Prefix <newPrefix>", ConsoleColor.Yellow);
        }

        public override void Setup(string[] arguments)
        {
            name = "help";
            this.arguments = arguments;
        }

        public HelpCommand(Client client)
        {
            this.client = client;
        }
    }
}
