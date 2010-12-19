using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace CometGateway.Server.TelnetDemo.Configuration
{
    public class TelnetServerConfiguration
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }

        public override bool Equals(object obj)
        {
            var otherTelnetServerConfiguration = obj as TelnetServerConfiguration;
            return otherTelnetServerConfiguration != null &&
                    Name == otherTelnetServerConfiguration.Name &&
                    Server == otherTelnetServerConfiguration.Server &&
                    Port == otherTelnetServerConfiguration.Port;
        }

        public override int GetHashCode()
        {
            return (Name ?? "").GetHashCode() ^
                   (Server ?? "").GetHashCode() ^ 
                   Port.GetHashCode();
        }
    }
}
