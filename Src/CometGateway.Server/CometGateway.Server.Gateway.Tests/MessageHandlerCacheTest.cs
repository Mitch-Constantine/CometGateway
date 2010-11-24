using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspComet;
using System.Collections;
using AspComet.Eventing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using CometGateway.Server.Gateway.MessageHandling;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class MessageHandlerCacheTest
    {
        [TestCleanup]
        public void ResetEventHub()
        {
            EventHub.Reset();
        }
        
        [TestMethod]
        public void MessageHandlerCacheStoresMessageHandlers()
        {
            var client = MockRepository.GenerateStub<IClient>();
            client.Expect(c => c.ID).Return("abc");

            MessageHandler messageHandler = new MessageHandler(null);
            var messageHandlerCache = new MessageHandlerCache();
            messageHandlerCache[client] = messageHandler;
            Assert.AreEqual(messageHandler, messageHandlerCache[client]);
        }

        [TestMethod]
        public void RemoveRemovesMessageHandlerFromCache()
        {
            var client = MockRepository.GenerateStub<IClient>();
            client.Expect(c => c.ID).Return("abc");

            var messageHandler = new MessageHandler(null);
            var messageHandlerCache = new MessageHandlerCache();
            messageHandlerCache[client] = messageHandler;
            messageHandlerCache.Remove(client);
            Assert.AreEqual(null, messageHandlerCache[client]);            
        }

        [TestMethod]
        public void RemoveDoesNotCrashOnMissingID()
        {
            var client = MockRepository.GenerateStub<IClient>();
            client.Expect(c => c.ID).Return("abc");

            var messageHandler = new MessageHandlerCache();
            messageHandler.Remove(client);
        }

        [TestMethod]
        public void RemoveMessageHandlerCallsOnDisconnect()
        {
            var mockRepository = new MockRepository();
            var client = mockRepository.Stub<IClient>();
            client.Expect(c => c.ID).Return("abc").Repeat.Any();

            var emptyMessageMap = new MessageMap(Enumerable.Empty<KeyValuePair<string, Type>>());
            var messageHandler = mockRepository.StrictMock<MessageHandler>(emptyMessageMap);
            messageHandler.Expect(h => h.HandleDisconnect(client));

            mockRepository.ReplayAll();
            var messageHandlerCache = new MessageHandlerCache();
            messageHandlerCache[client] = messageHandler;
            EventHub.Publish(new DisconnectedEvent(client));
            Assert.AreEqual(null, messageHandlerCache[client]);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void DisconnectDoesNotCrashIfNoMessageHandler()
        {
            var mockRepository = new MockRepository();
            var client = mockRepository.Stub<IClient>();
            client.Expect(c => c.ID).Return("abc").Repeat.Any();

            mockRepository.ReplayAll();
            new MessageHandlerCache();
            EventHub.Publish(new DisconnectedEvent(client));
            mockRepository.VerifyAll();
        }
    }
}
