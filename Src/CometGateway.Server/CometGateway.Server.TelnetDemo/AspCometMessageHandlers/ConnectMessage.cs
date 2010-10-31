using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CometGateway.Server.Gateway;

namespace CometGateway.Server.TelnetDemo.AspCometMessageHandlers
{
    [MessageType("connect")]
    public class ConnectMessage
    {
        public string server;
        public int port;
    }
}
