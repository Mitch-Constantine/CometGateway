using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CometGateway.Server.Gateway;

namespace CometGateway.Server.TelnetDemo.AspCometMessageHandlers
{
    [MessageType("textReceived")]
    public class TextReceivedMessage
    {
        public string text;
    }
}