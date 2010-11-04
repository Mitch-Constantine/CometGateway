using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CometGateway.Server.Gateway;

namespace CometGateway.Server.TelnetDemo.AspCometMessageHandlers
{
    [MessageType("textEntered")]
    public class TextEnteredMessage
    {
        public string text;
    }
}