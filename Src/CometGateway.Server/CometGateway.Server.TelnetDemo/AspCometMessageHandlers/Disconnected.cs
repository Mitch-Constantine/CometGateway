﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CometGateway.Server.Gateway;
using CometGateway.Server.Gateway.MessageHandling;

namespace CometGateway.Server.TelnetDemo.AspCometMessageHandlers
{
    [MessageType("disconnected")]
    public class Disconnected
    {
    }
}