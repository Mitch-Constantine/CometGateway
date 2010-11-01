using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public interface IConnection<TData>
    {
        void StartConnect(string server, int port);
        void StartDisconnect();

        void Send(TData data);

        bool Connected { get; }

        event Action ServerDisconnected;
        event Action ConnectionSucceeded;
        event Action<string> ErrorOccurred;
        event Action<TData> DataReceived;
    }
}
