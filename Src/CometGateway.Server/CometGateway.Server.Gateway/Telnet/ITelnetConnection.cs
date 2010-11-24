using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CometGateway.Server.Gateway.Infrastructure;

namespace CometGateway.Server.Gateway.Telnet
{
    public interface ITelnetConnection : IConnection<byte[], byte[]>
    {
    }
}
