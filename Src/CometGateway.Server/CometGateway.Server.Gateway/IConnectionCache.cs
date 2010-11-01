using System;
using AspComet;

namespace CometGateway.Server.Gateway
{
    public interface IConnectionCache<TData>
    {
        void Remove(AspComet.IClient client);
        void Remove(string clientId);

        void Add(IClient client, IConnection<TData> connection);
        void Add(string clientId, IConnection<TData> connection);

        IConnection<TData> Get(IClient client);
        IConnection<TData> Get(string clientId);

        IConnection<TData> this[AspComet.IClient client] { get; set; }
        IConnection<TData> this[string clientId] { get; set; }
    }
}
