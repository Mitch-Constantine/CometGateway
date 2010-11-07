using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public abstract class ProtocolLayer<TExposed, TInternal> : IConnection<TExposed>
    {
        public IConnection<TInternal> InternalConnection { get; private set; }

        public ProtocolLayer(IConnection<TInternal> connection)
        {
            InternalConnection = connection;

            connection.ServerDisconnected += OnServerDisconnected;
            connection.ConnectionSucceeded += OnConnectionSucceeded;
            connection.ErrorOccurred += OnErrorOccurred;
            connection.DataReceived += OnDataReceived;  
        }


        public virtual void StartConnect(string server, int port)
        {
            InternalConnection.StartConnect(server, port);
        }

        public virtual void StartDisconnect()
        {
            InternalConnection.StartDisconnect();
        }

        public void Send(TExposed data)
        {
            var convertedData = ConvertToInternalFormat(data);
            InternalConnection.Send(convertedData);
        }

        public bool Connected
        {
            get { return InternalConnection.Connected; }
        }

        public event Action ServerDisconnected;
        public event Action ConnectionSucceeded;
        public event Action<string> ErrorOccurred;
        public event Action<TExposed> DataReceived;

        internal abstract TInternal ConvertToInternalFormat(TExposed data);
        internal abstract TExposed ConvertFromInternalFormat(TInternal data);

        internal void OnConnectionSucceeded()
        {
            if (ConnectionSucceeded != null)
                ConnectionSucceeded();
        }

        internal void OnServerDisconnected()
        {
            if (ServerDisconnected != null)
                ServerDisconnected();
        }

        internal void OnErrorOccurred(string message)
        {
            if (ErrorOccurred != null)
                ErrorOccurred(message);
        }

        internal void OnDataReceived(TInternal data)
        {
            if (DataReceived != null)
            {
                var convertedData = ConvertFromInternalFormat(data);
                DataReceived(convertedData);
            };
        }
    }
}
