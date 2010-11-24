using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CometGateway.Server.Gateway.Tests.SampleMessages;
using AspComet;
using CometGateway.Server.Gateway.MessageHandling;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class TestMessageHandler
    {
        public class SampleHandler : MessageHandler
        {
            public SampleHandler(MessageMap messageMap) : base(messageMap) { }
            
            public string called;
            public string clientId;

            public void Handle(SampleMessage1 message, Message rawMessage) 
            { 
                Record("SampleMessage1", rawMessage.clientId); 
            }
            public void Handle(SampleMessage2 message, Message rawMessage) 
            { 
                Record("SampleMessage2", rawMessage.clientId); 
            }

            private void Record(string called, string clientId)
            {
                this.called = called;
                this.clientId = clientId;
            }
        }

        [TestMethod]
        public void MessageHandlerDispatchesByType()
        {
            Message message = new Message();
            message.SetData("type", "type1");
            MessageMap messageMap = new MessageMap(GetType().Assembly, "CometGateway.Server.Gateway.Tests.SampleMessages");
            SampleHandler handler = new SampleHandler(messageMap);
            handler.HandleMessage(message);
            Assert.AreEqual("SampleMessage1", handler.called);
        }
    }
}
