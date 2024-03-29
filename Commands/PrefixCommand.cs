﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumeniteApiCsharp;

namespace LumeniteApiCsharp.Commands
{
    class PrefixCommand : Command
    {
        public override void Action()
        {
            if (arguments.Length > 1)
            {
                if (arguments[1].Length == 1)
                {
                    client.Prefix = arguments[1][0];
                    Utils.Print($"New Prefix: {client.Prefix}", ConsoleColor.Green);
                    Utils.Print($"Type '{client.Prefix}' before any command", ConsoleColor.Yellow);
                    Utils.SavePrefix(client.Prefix);
                }
                else
                {
                    Utils.Print("PREFIX MUST BE 1 CHARACTER.", ConsoleColor.Red);
                }
            }
            else
            {
                Utils.Print("INVALID SYNTAX.", ConsoleColor.Red);
            }
        }

        public override void Setup(string[] arguments)
        {
            name = "prefix";
            this.arguments = arguments;
        }

        public PrefixCommand(Client client)
        {
            this.client = client;
        }
    }
}
