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
        public char Prefix = '-';

        public List<BaseDevice> Devices { get; private set; }
        private readonly List<Commands.Command> _commands = new();

        public Client()
        {
            char p = Utils.LoadPrefix();

            if (p != ' ')
                Prefix = p;

            _commands.Add(new DeviceListCommand(this));
            _commands.Add(new HelpCommand(this));
            _commands.Add(new PowerAllCommand(this));
            _commands.Add(new PowerCommand(this));
            _commands.Add(new PrefixCommand(this));
        }

        public async Task Login(GatewayOptions options)
        {
            GatewayConnection gateway = new(options);
            await gateway.Connect();
            Devices = gateway.Devices;

            Task.Run(() => WaitForCommand());
        }

        private void WaitForCommand()
        {
            Utils.Print($"Type '{Prefix}' before any command.", ConsoleColor.Yellow);
            Utils.Print($"{Prefix}help for help.", ConsoleColor.Yellow);

            while (true)
            {
                string line = Console.ReadLine();

                if (line != null && line[0] == Prefix)
                {
                    string[] arguments = line.Substring(1, line.Length - 1).ToLower().Split(' ');
                    string command = arguments[0];

                    for (int i = 0; i < _commands.Count; i++)
                    {
                        Commands.Command cmd = _commands[i];
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