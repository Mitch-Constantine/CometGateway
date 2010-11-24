using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.Infrastructure
{
    public interface IConnection<TDataReceived, TDataSent>
    {
        void StartConnect(string server, int port);
        void StartDisconnect();

        void Send(TDataSent data);

        bool Connected { get; }

        event Action ServerDisconnected;
        event Action ConnectionSucceeded;
        event Action<string> ErrorOccurred;
        event Action<TDataReceived> DataReceived;
    }
}
