using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspComet;
using AspComet.Eventing;

namespace CometGateway.Server.Gateway
{
    public class ConnectionCache<TData> : CometGateway.Server.Gateway.IConnectionCache<TData>
    {
        readonly Dictionary<string, IConnection<TData>> connectionMapping = new Dictionary<string, IConnection<TData>>();

        public ConnectionCache()
        {
            EventHub.Subscribe<DisconnectedEvent>(OnClientDisconnected);
        }

        public IConnection<TData> this[IClient client]
        {
            get { return this[client.ID]; }
            set { this[client.ID] = value; }
        }

        public IConnection<TData> this[string clientId]
        {
            get
            {
                IConnection<TData> communicationLayer = null;
                if (!connectionMapping.TryGetValue(clientId, out communicationLayer))
                    return null;
                return communicationLayer;
            }
            set { connectionMapping[clientId] = value; }
        }
        
        public void Remove(IClient client)
        {
            Remove(client.ID);
        }

        public void Remove(string clientId)
        {
            connectionMapping.Remove(clientId);
        }

        private void OnClientDisconnected(DisconnectedEvent ev)
        {
            IClient client = ev.Client;
            IConnection<TData> connection = this[client];

            if (connection != null)
            {
                if (connection.Connected)
                {
                    connection.StartDisconnect();
                }
                Remove(client);
            }
        }


        public void Add(IClient client, IConnection<TData> connection)
        {
            this[client] = connection;
        }
        public void Add(string clientId, IConnection<TData> connection)
        {
            this[clientId] = connection;
        }

        public IConnection<TData> Get(IClient client)
        {
            return this[client.ID];
        }
        public IConnection<TData> Get(string clientId)
        {
            return this[clientId];
        }
    }
}
