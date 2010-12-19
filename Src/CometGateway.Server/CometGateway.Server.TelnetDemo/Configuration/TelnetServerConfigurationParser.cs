using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;

namespace CometGateway.Server.TelnetDemo.Configuration
{
    public class TelnetServerConfigurationParser : CometGateway.Server.TelnetDemo.Configuration.ITelnetServerConfigurationParser
    {
        private string configString;

        public TelnetServerConfigurationParser()
            : this(GetConfigurationString())
        {        
        }

        public TelnetServerConfigurationParser(string configString)
        {
            this.configString = configString;
        }

        public virtual TelnetServerConfiguration[] Parse()
        {
            if (configString == null)
                return new TelnetServerConfiguration[] { };

            string[] serverConfigs = configString.Split(',');
            return serverConfigs
                        .Select(config => ParseOneServerConfig(config))
                        .ToArray();
        }

        private static string GetConfigurationString()
        {
            return ConfigurationManager.AppSettings["TelnetServers"];
        }

        private TelnetServerConfiguration ParseOneServerConfig(string configString)
        {
            Regex regex = new Regex(@"^\s*(\w+)\s*=\s*([^:]+)\s*:\s*(\d+)\s*$");
            Match match = regex.Match(configString);

            return match.Success ?
                    new TelnetServerConfiguration
                    {
                        Name = match.Groups[1].Value,
                        Server = match.Groups[2].Value,
                        Port = Int32.Parse(match.Groups[3].Value)
                    } :
                    null;
        }
    }
}
