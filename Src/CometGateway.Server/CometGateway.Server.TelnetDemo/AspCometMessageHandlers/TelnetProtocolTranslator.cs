﻿using System;
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
        IConnectionCache<byte[]> connectionCache;
        ISocketConnection socketConnection;
        IClientRepository clientRepository;

        public string ClientId { get; internal set; }
        public string Channel { get; internal set; }
        
        public TelnetProtocolTranslator(
            IConnectionCache<byte[]> connectionCache, 
            ISocketConnection socketConnection,
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
            socketConnection.ConnectionSucceeded += OnConnectSucceeded;
            connectionCache.Add(rawMessage.clientId, socketConnection);
            ClientId = rawMessage.clientId;
            Channel = rawMessage.channel;
            socketConnection.ConnectionSucceeded += OnConnectSucceeded;
            socketConnection.ErrorOccurred += OnErrorOccurred;
            socketConnection.ServerDisconnected += OnDisconnected;
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
    }
}