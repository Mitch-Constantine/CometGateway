using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspComet;
using System.Collections;
using AspComet.Eventing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class ConnectionCacheTest
    {
        [TestCleanup]
        public void ResetEventHub()
        {
            EventHub.Reset();
        }
        
        [TestMethod]
        public void ConnectionCacheStoresConnections()
        {
            var client = MockRepository.GenerateStub<IClient>();
            client.Expect(c => c.ID).Return("abc");

            var socketCommunicationLayer = MockRepository.GenerateMock<IConnection<byte[]>>();
            var connectionCache = new ConnectionCache<byte[]>();
            connectionCache[client] = socketCommunicationLayer;
            Assert.AreEqual(socketCommunicationLayer, connectionCache[client]);
        }

        [TestMethod]
        public void RemoveRemovesConnectionFromCache()
        {
            var client = MockRepository.GenerateStub<IClient>();
            client.Expect(c => c.ID).Return("abc");

            var socketCommunicationLayer = MockRepository.GenerateMock<IConnection<byte[]>>();
            var connectionCache = new ConnectionCache<byte[]>();
            connectionCache[client] = socketCommunicationLayer;
            connectionCache.Remove(client);
            Assert.AreEqual(null, connectionCache[client]);
            
        }

        [TestMethod]
        public void RemoveDoesNotCrashOnMissingID()
        {
            var client = MockRepository.GenerateStub<IClient>();
            client.Expect(c => c.ID).Return("abc");

            var connectionCache = new ConnectionCache<byte[]>();
            connectionCache.Remove(client);
        }

        [TestMethod]
        public void RemoveConnectionAutomaticallyOnDisconnect()
        {
            var mockRepository = new MockRepository();
            var client = mockRepository.Stub<IClient>();
            client.Expect(c => c.ID).Return("abc").Repeat.Any();

            var connection = mockRepository.StrictMock<IConnection<byte[]>>();
            connection.Expect(c => c.Connected).Return(true).Repeat.Any();
            connection.Expect(c => c.StartDisconnect());

            mockRepository.ReplayAll();
            var connectionCache = new ConnectionCache<byte[]>();
            connectionCache[client] = connection;
            EventHub.Publish(new DisconnectedEvent(client));
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void RemoveConnectionDoesNotDisconnectIfNotConnected()
        {
            var mockRepository = new MockRepository();
            var client = mockRepository.Stub<IClient>();
            client.Expect(c => c.ID).Return("abc").Repeat.Any();

            var connection = mockRepository.StrictMock<IConnection<byte[]>>();
            connection.Expect(c => c.Connected).Return(false).Repeat.Any();

            mockRepository.ReplayAll();
            var connectionCache = new ConnectionCache<byte[]>();
            connectionCache[client] = connection;
            EventHub.Publish(new DisconnectedEvent(client));
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void RemoveConnectionDoesNotCrashIfNoConnection()
        {
            var mockRepository = new MockRepository();
            var client = mockRepository.Stub<IClient>();
            client.Expect(c => c.ID).Return("abc").Repeat.Any();


            mockRepository.ReplayAll();
            new ConnectionCache<byte[]>();
            EventHub.Publish(new DisconnectedEvent(client));
            mockRepository.VerifyAll();
        }
    }
}
