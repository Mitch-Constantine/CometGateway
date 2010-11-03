using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CometGateway.Server.Gateway;
using AspComet;
using Microsoft.Practices.ServiceLocation;
using System.Threading;

namespace CometGateway.Server.TelnetDemo.AspCometMessageHandlers
{
    public class TelnetProtocolTranslator : MessageHandler
    {
        IConnectionCache<string> connectionCache;
        IConnection<string> socketConnection;
        IClientRepository clientRepository;

        public string ClientId { get; internal set; }
        public string Channel { get; internal set; }
        
        public TelnetProtocolTranslator(
            IConnectionCache<string> connectionCache, 
            IConnection<string> socketConnection,
            IClientRepository clientRepository
        )
            : base (GetMessageMap())
        {
            this.connectionCache = connectionCache;
            this.socketConnection = socketConnection;
            this.clientRepository = clientRepository;
        }

        private static MessageMap GetMessageMap()
        {
            return new MessageMap(typeof(TelnetProtocolTranslator).Assembly,
                                   typeof(TelnetProtocolTranslator).Namespace);
        }

        public void Handle(ConnectMessage message, Message rawMessage)
        {
            socketConnection.StartConnect(message.server, message.port);
            connectionCache.Add(rawMessage.clientId, socketConnection);

            ConfigureForHandling(rawMessage);
        }

        private void ConfigureForHandling(Message rawMessage)
        {
            ClientId = rawMessage.clientId;
            Channel = rawMessage.channel;

            socketConnection.ConnectionSucceeded += OnConnectSucceeded;
            socketConnection.ErrorOccurred += OnErrorOccurred;
            socketConnection.ServerDisconnected += OnDisconnected;
            socketConnection.DataReceived += OnDataReceived;
        }

        public void OnConnectSucceeded()
        {
            Send(new ConnectionSucceededMessage());
        }

        public void OnErrorOccurred(string errorMessage)
        {
            Send(new ErrorOccurredMessage { errorMessage = errorMessage });
        }

        public void OnDisconnected()
        {
            Send(new Disconnected());
        }

        private void Send(object message)
        {
            var rawMessage = MessageMap.Encode(message);
            rawMessage.channel = Channel;
            IClient client = clientRepository.GetByID(ClientId);
            if (client != null)
            {
                client.Enqueue(rawMessage);
                client.FlushQueue();
            }
        }

        internal void OnDataReceived(string dataReceived)
        {
            Send(new TextReceivedMessage { text = dataReceived });
        }

        public static void WireUp()
        {
            MessageHandler.WireUp<TelnetProtocolTranslator>((message) => 
                {
                    IMessageHandlerCache cache = ServiceLocator.Current.GetInstance<IMessageHandlerCache>();
                    return FindTranslatorObject(
                        cache, 
                        message,
                        () => ServiceLocator.Current.GetInstance<TelnetProtocolTranslator>()
                    );
                }
            );
        }

        public static TelnetProtocolTranslator FindTranslatorObject(
            IMessageHandlerCache cache, 
            Message message,
            Func<MessageHandler> messageHandlerFactory
        )
        {
            var messageHandler = cache[message.clientId];
            if (messageHandler == null)
            {
                messageHandler = messageHandlerFactory();
                cache[message.clientId] = messageHandler;
            }

            return cache[message.clientId] as TelnetProtocolTranslator;
        }
    }
}