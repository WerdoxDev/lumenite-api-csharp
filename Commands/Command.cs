using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeniteApiCsharp.Commands
{
    public abstract class Command
    {
        public string name;
        public string[] arguments;
        public List<string> aliases = new();
        public Client client;

        public abstract void Action();
        public abstract void Setup(string[] arguments);
    }
}
