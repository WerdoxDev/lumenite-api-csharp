using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LumeniteApiCsharp.Commands;

namespace LumeniteApiCsharp
{
    public class Client
    {
        public char prefix = '-';

        public List<BaseDevice> Devices { get; private set; }
        private List<Commands.Command> commands;

        public Client()
        {
            char p = Utils.LoadPrefix();

            if (p != ' ')
                prefix = p;

            commands.Add(new DeviceListCommand(this));
            commands.Add(new HelpCommand(this));
            commands.Add(new PowerAllCommand(this));
            commands.Add(new PowerCommand(this));
            commands.Add(new PrefixCommand(this));
        }

        public async Task Login(GatewayOptions options)
        {
            GatewayConnection gateway = new(options);
            await gateway.Connect();
            Devices = gateway.Devices;

            Thread thread = new Thread(new ThreadStart(WaitForCommand));
            thread.Start();
        }

        private void WaitForCommand()
        {
            Utils.Print($"Type '{prefix}' before any command.", ConsoleColor.Yellow);
            Utils.Print($"{prefix}help for help.", ConsoleColor.Yellow);

            while (true)
            {
                string line = Console.ReadLine();

                if (line[0] == prefix)
                {
                    string[] arguments = line.Substring(1, line.Length - 1).ToLower().Split(' ');
                    string command = arguments[0];

                    for (int i = 0; i < commands.Count; i++)
                    {
                        Commands.Command cmd = commands[i];
                        cmd.Setup(arguments);

                        if (cmd.name == command || cmd.aliases.Contains(command))
                        {
                            cmd.Action();
                        }
                    }
                }
            }
        }

        public T GetDeviceById<T>(int id) where T : BaseDevice
        {
            return Devices.Where(x => x.Id == id).Cast<T>().FirstOrDefault();
        }
    }

    public record GatewayOptions(string Url, int Port,bool Ssl, string Username, string Password);
}