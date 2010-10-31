using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public interface IConnection
    {
        void StartConnect(string server, int port);
        void StartDisconnect();

        bool Connected { get; }

        event Action ServerDisconnected;
        event Action ConnectionSucceeded;
        event Action<string> ErrorOccurred; 
    }
}
