using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public interface ISocketConnection : IConnection<byte[]>
    {
    }
}
