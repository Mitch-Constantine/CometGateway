using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CometGateway.Server.Gateway;

namespace CometGateway.Server.TelnetDemo.AspCometMessageHandlers
{
    public class TelnetConnection : BytesToStringConversionLayer
    {
        public TelnetConnection(ISocketConnection connection) : 
            base(connection)
        {
        }
    }
}