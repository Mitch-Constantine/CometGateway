using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.Infrastructure
{
    public abstract class ProtocolLayer<
        TExposedReceived, 
        TExposedSent, 
        TInternalReceived, 
        TInternalSent> : IConnection<TExposedReceived, TExposedSent>
    {
        public IConnection<TInternalReceived, TInternalSent> InternalConnection { get; private set; }

        public ProtocolLayer(IConnection<TInternalReceived, TInternalSent> connection)
        {
            InternalConnection = connection;

            if (connection != null)
            {
                connection.ServerDisconnected += OnServerDisconnected;
                connection.ConnectionSucceeded += OnConnectionSucceeded;
                connection.ErrorOccurred += OnErrorOccurred;
                connection.DataReceived += OnDataReceived;  
            }
         }


        public virtual void StartConnect(string server, int port)
        {
            InternalConnection.StartConnect(server, port);
        }

        public virtual void StartDisconnect()
        {
            InternalConnection.StartDisconnect();
        }

        public void Send(TExposedSent data)
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
        public event Action<TExposedReceived> DataReceived;

        internal abstract TInternalSent ConvertToInternalFormat(TExposedSent data);
        internal abstract TExposedReceived ConvertFromInternalFormat(TInternalReceived data);

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

        internal void OnDataReceived(TInternalReceived data)
        {
            if (DataReceived != null)
            {
                var convertedData = ConvertFromInternalFormat(data);
                DataReceived(convertedData);
            };
        }
    }
}
