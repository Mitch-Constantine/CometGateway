using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CometGateway.Server.TelnetDemo.AspCometMessageHandlers;
using CometGateway.Server.Gateway;
using Rhino.Mocks;

using AspComet;
using System.Diagnostics;


namespace CometGateway.Server.TelnetDemo.Tests.AspCometMessageHandlers
{
    [TestClass]
    public class TelnetProtocolTranslatorTest
    {
        [TestMethod]
        public void ConnectStartsConnectionAndCaches()
        {
            MockRepository mockRepository = new MockRepository();
            var socketConnection = mockRepository.DynamicMock<IConnection<string>>();
            TelnetProtocolTranslator connectMessageHandler = new TelnetProtocolTranslator(socketConnection, null);

            socketConnection.Expect(connection => connection.StartConnect("test.com", 25));

            socketConnection.Expect(connection => connection.ServerDisconnected += connectMessageHandler.OnDisconnected);
            socketConnection.Expect(connection => connection.ConnectionSucceeded += connectMessageHandler.OnConnectSucceeded);
            socketConnection.Expect(connection => connection.ErrorOccurred += connectMessageHandler.OnErrorOccurred);

            var message = new Message();
            message.SetData("type", "connect");
            message.SetData("server", "test.com");
            message.SetData("port", 25);
            message.clientId = "abc";
            message.channel = "def";
            

            mockRepository.ReplayAll();
            connectMessageHandler.HandleMessage(message);
            Assert.AreEqual("abc", connectMessageHandler.ClientId);
            Assert.AreEqual("def", connectMessageHandler.Channel);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void ConnectSendsBackConnectionDone()
        {
            MockRepository mockRepository = new MockRepository();

            var clientRepository = mockRepository.Stub<IClientRepository>();
            var aClient = mockRepository.StrictMock<IClient>();


            TelnetProtocolTranslator connectMessageHandler = new TelnetProtocolTranslator(null, clientRepository);
            connectMessageHandler.ClientId = "abc";
            connectMessageHandler.Channel = "def";
            clientRepository.Expect(repository => repository.GetByID("abc"))
                            .Return(aClient);
            aClient.Expect(client => client.Enqueue(null))
                   .IgnoreArguments()
                   .WhenCalled(mi =>
                   {
                       Assert.AreEqual(1, mi.Arguments.Length);
                       var messages = mi.Arguments[0] as Message[];
                       Message reply = messages.Single() as Message;
                       Assert.AreEqual("connectionSucceeded", reply.GetData<string>("type"));
                       Assert.AreEqual("def", reply.channel);
                   });
            aClient.Expect(c => c.FlushQueue());

            mockRepository.ReplayAll();
            connectMessageHandler.OnConnectSucceeded();
            mockRepository.VerifyAll();
         }

        [TestMethod]
        public void ReceiveForwardsToBrowser()
        {
            MockRepository mockRepository = new MockRepository();

            var clientRepository = mockRepository.Stub<IClientRepository>();
            var aClient = mockRepository.StrictMock<IClient>();


            var messageHandler = new TelnetProtocolTranslator(
                    null, 
                    clientRepository
            );
            messageHandler.ClientId = "abc";
            messageHandler.Channel = "def";
            clientRepository.Expect(repository => repository.GetByID("abc"))
                            .Return(aClient);
            aClient.Expect(client => client.Enqueue(null))
                   .IgnoreArguments()
                   .WhenCalled(mi =>
                   {
                       Assert.AreEqual(1, mi.Arguments.Length);
                       var messages = mi.Arguments[0] as Message[];
                       Message reply = messages.Single() as Message;
                       Assert.AreEqual(
                           "textReceived", 
                           reply.GetData<string>("type")
                       );
                       Assert.AreEqual("ABCD", reply.GetData<string>("text"));
                       Assert.AreEqual("def", reply.channel);
                   });
            aClient.Expect(c => c.FlushQueue());

            mockRepository.ReplayAll();
            messageHandler.OnDataReceived("ABCD");
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void ConnectSendsBackError()
        {
            MockRepository mockRepository = new MockRepository();

            var clientRepository = mockRepository.Stub<IClientRepository>();
            var aClient = mockRepository.StrictMock<IClient>();

            clientRepository.Expect(repository => repository.GetByID("abc"))
                            .Return(aClient);
            aClient.Expect(client => client.Enqueue(null))
                   .IgnoreArguments()
                   .WhenCalled(mi =>
                   {
                       Assert.AreEqual(1, mi.Arguments.Length);
                       var messages = mi.Arguments[0] as Message[];
                       Message reply = messages.Single() as Message;
                       Assert.AreEqual("errorOccurred", reply.GetData<string>("type"));
                       Assert.AreEqual("it's wrong", reply.GetData<string>("errorMessage"));
                       Assert.AreEqual("def", reply.channel);
                   });
            aClient.Expect(c => c.FlushQueue());

            TelnetProtocolTranslator connectMessageHandler = new TelnetProtocolTranslator(null, clientRepository);
            connectMessageHandler.ClientId = "abc";
            connectMessageHandler.Channel = "def";

            mockRepository.ReplayAll();
            connectMessageHandler.OnErrorOccurred("it's wrong");
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Disconnect()
        {
            MockRepository mockRepository = new MockRepository();

            var clientRepository = mockRepository.Stub<IClientRepository>();
            var aClient = mockRepository.StrictMock<IClient>();


            TelnetProtocolTranslator connectMessageHandler = new TelnetProtocolTranslator(null, clientRepository);
            connectMessageHandler.ClientId = "abc";
            connectMessageHandler.Channel = "def";
            clientRepository.Expect(repository => repository.GetByID("abc"))
                            .Return(aClient);
            aClient.Expect(client => client.Enqueue(null))
                   .IgnoreArguments()
                   .WhenCalled(mi =>
                   {
                       Assert.AreEqual(1, mi.Arguments.Length);
                       var messages = mi.Arguments[0] as Message[];
                       Message reply = messages.Single() as Message;
                       Assert.AreEqual("disconnected", reply.GetData<string>("type"));
                       Assert.AreEqual("def", reply.channel);
                   });
            aClient.Expect(c => c.FlushQueue());

            mockRepository.ReplayAll();
            connectMessageHandler.OnDisconnected();
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void FindTranslatorObject_ReturnsExistingObject()
        {
            MockRepository mockRepository = new MockRepository();

            var clientRepository = mockRepository.Stub<IClientRepository>();
            var aClient = mockRepository.StrictMock<IClient>();
            clientRepository.Expect(repository => repository.GetByID("abc"))
                            .Return(aClient);


            Message message = new Message();
            message.clientId = "abc";

            var cache = new MessageHandlerCache();
            TelnetProtocolTranslator translator = new TelnetProtocolTranslator(null, null);
            cache["abc"] = translator;

            Assert.AreEqual(translator, TelnetProtocolTranslator.FindTranslatorObject(cache, message, null));
        }

        [TestMethod]
        public void FindTranslatorObject_MakesNewObjectIfNeeded()
        {
            MockRepository mockRepository = new MockRepository();

            var clientRepository = mockRepository.Stub<IClientRepository>();
            var aClient = mockRepository.StrictMock<IClient>();
            clientRepository.Expect(repository => repository.GetByID("abc"))
                            .Return(aClient);


            var message = new Message();
            message.clientId = "abc";

            var cache = new MessageHandlerCache();
            TelnetProtocolTranslator translator = new TelnetProtocolTranslator(null, null);

            Assert.AreEqual(translator, TelnetProtocolTranslator.FindTranslatorObject(cache, message, ()=>translator));
        }

        [TestMethod]
        public void TextTypedForwardsLine()
        {
            MockRepository mockRepository = new MockRepository();
            var socketConnection = mockRepository.DynamicMock<IConnection<string>>();
            TelnetProtocolTranslator connectMessageHandler = new TelnetProtocolTranslator(socketConnection, null);

            socketConnection.Expect(connection => connection.StartConnect("test.com", 25));
            socketConnection.Expect(connection => connection.ServerDisconnected += connectMessageHandler.OnDisconnected);
            socketConnection.Expect(connection => connection.ConnectionSucceeded += connectMessageHandler.OnConnectSucceeded);
            socketConnection.Expect(connection => connection.ErrorOccurred += connectMessageHandler.OnErrorOccurred);

            var message = new Message();
            message.SetData("type", "connect");
            message.SetData("server", "test.com");
            message.SetData("port", 25);
            message.clientId = "abc";
            message.channel = "def";


            mockRepository.ReplayAll();
            connectMessageHandler.HandleMessage(message);
            Assert.AreEqual("abc", connectMessageHandler.ClientId);
            Assert.AreEqual("def", connectMessageHandler.Channel);
            mockRepository.VerifyAll();
        }
    }
}
