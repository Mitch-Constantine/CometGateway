using System;

namespace CometGateway.Server.TelnetDemo.Configuration
{
    public interface ITelnetServerConfigurationParser
    {
        TelnetServerConfiguration[] Parse();
    }
}
