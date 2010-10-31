using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspComet;
using AspComet.Eventing;

namespace CometGateway.Server.Gateway
{
    public class ConnectionCache : CometGateway.Server.Gateway.IConnectionCache
    {
        readonly Dictionary<string, IConnection> connectionMapping = new Dictionary<string, IConnection>();

        public ConnectionCache()
        {
            EventHub.Subscribe<DisconnectedEvent>(OnClientDisconnected);
        }

        public IConnection this[IClient client]
        {
            get { return this[client.ID]; }
            set { this[client.ID] = value; }
        }

        public IConnection this[string clientId]
        {
            get
            {
                IConnection communicationLayer = null;
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
            IConnection connection = this[client];

            if (connection != null)
            {
                if (connection.Connected)
                {
                    connection.StartDisconnect();
                }
                Remove(client);
            }
        }


        public void Add(IClient client, IConnection connection)
        {
            this[client] = connection;
        }
        public void Add(string clientId, IConnection connection)
        {
            this[clientId] = connection;
        }

        public IConnection Get(IClient client)
        {
            return this[client.ID];
        }
        public IConnection Get(string clientId)
        {
            return this[clientId];
        }
    }
}
