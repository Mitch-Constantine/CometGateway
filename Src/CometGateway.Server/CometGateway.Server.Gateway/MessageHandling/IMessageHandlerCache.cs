using System;

namespace CometGateway.Server.Gateway.MessageHandling
{
    public interface IMessageHandlerCache
    {
        void Remove(AspComet.IClient client);
        void Remove(string clientId);
        MessageHandler this[AspComet.IClient client] { get; set; }
        MessageHandler this[string clientId] { get; set; }
    }
}
