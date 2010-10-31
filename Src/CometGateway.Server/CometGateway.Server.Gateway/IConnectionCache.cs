using System;
using AspComet;

namespace CometGateway.Server.Gateway
{
    public interface IConnectionCache
    {
        void Remove(AspComet.IClient client);
        void Remove(string clientId);

        void Add(IClient client, IConnection connection);
        void Add(string clientId, IConnection connection);

        IConnection Get(IClient client);
        IConnection Get(string clientId);

        IConnection this[AspComet.IClient client] { get; set; }
        IConnection this[string clientId] { get; set; }
    }
}
