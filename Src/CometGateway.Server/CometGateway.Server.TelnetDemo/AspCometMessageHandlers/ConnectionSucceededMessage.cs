using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CometGateway.Server.Gateway;
using CometGateway.Server.Gateway.MessageHandling;

namespace CometGateway.Server.TelnetDemo.AspCometMessageHandlers
{
    [MessageType("connectionSucceeded")]
    public class ConnectionSucceededMessage
    {
    }
}
