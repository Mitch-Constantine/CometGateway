using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspComet;
using AspComet.Eventing;
using Microsoft.Practices.ServiceLocation;
using System.Reflection;
using log4net;

namespace CometGateway.Server.Gateway
{
    public class MessageHandler     
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(MessageHandler).Name);

        protected MessageMap MessageMap { get; private set; }

        public MessageHandler(MessageMap messageMap)
        {
            MessageMap = messageMap;
        }

        public virtual void HandleMessage(Message rawMessage)
        {
            LogMessage("Message received", rawMessage);
            object decoded = MessageMap.Decode(rawMessage);
            Type messageType = decoded.GetType();

            MethodInfo messageHandler = this.GetType().GetMethods()
                                                      .SingleOrDefault(mi =>
                                                            mi.Name == "Handle" &&
                                                            mi.GetParameters().Length == 2 &&
                                                            mi.GetParameters()[0].ParameterType.Equals(messageType) &&
                                                            mi.GetParameters()[1].ParameterType.Equals(typeof(Message)) 
                                                      );
            if (messageHandler != null)
                messageHandler.Invoke(this, new object[] { decoded, rawMessage });
        }

        protected static void LogMessage(string header, Message rawMessage)
        {
            log.Info(header);
            log.InfoFormat("Client ID:" + rawMessage.clientId);
            foreach (var dataKey in rawMessage.data as Dictionary<string, object>)
            {
                log.InfoFormat("Message: {0} as {1}", dataKey.Key, dataKey.Value);
            }
        }

        public virtual void HandleDisconnect(IClient client)
        {
        }

        public static void WireUp<THandler>(Func<Message, MessageHandler> findHandler) 
                where THandler : MessageHandler
        {
            EventHub.Subscribe<PublishingEvent>(ev => 
            {
                MessageHandler handler = findHandler(ev.Message);
                handler.HandleMessage(ev.Message); 
            });

            EventHub.Subscribe<DisconnectedEvent>(ev =>
                {
                    MessageHandler handler = ServiceLocator.Current.GetInstance<THandler>();
                    handler.HandleDisconnect(ev.Client);
                });
        }
    }
}
