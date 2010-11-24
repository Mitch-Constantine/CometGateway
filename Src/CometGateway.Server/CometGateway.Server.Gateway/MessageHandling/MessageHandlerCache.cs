using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspComet;
using AspComet.Eventing;

namespace CometGateway.Server.Gateway.MessageHandling
{
    public class MessageHandlerCache : IMessageHandlerCache
    {
        readonly Dictionary<string, MessageHandler> connectionMapping = 
                        new Dictionary<string, MessageHandler>();

        public MessageHandler this[IClient client]
        {
            get { return this[client.ID]; }
            set { this[client.ID] = value; }
        }

        public MessageHandlerCache()
        {
            EventHub.Subscribe<DisconnectedEvent>(OnClientDisconnected);
        }

        private void OnClientDisconnected(DisconnectedEvent ev)
        {
            IClient client = ev.Client;
            MessageHandler messageHandler = this[client];

            if (messageHandler != null)
            {
                messageHandler.HandleDisconnect(client);
                Remove(client);
            }
        }

        public MessageHandler this[string clientId]
        {
            get
            {
                MessageHandler communicationLayer = null;
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

    }
}
