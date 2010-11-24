using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspComet;
using System.Reflection;
using CometGateway.Server.Gateway.MessageHandling;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class MessageMapTest
    {
        class SampleObject
        {
            public string V1 { get; set; }
            public int V2;
        }

        [TestMethod]
        public void TestEncode()
        {
            var map = new MessageMap(new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("type1", typeof(SampleObject)) });
            Message message = map.Encode(new SampleObject() { V1 = "ABC", V2 = 5 });

            Assert.AreEqual("type1", message.GetData<string>("type"));
            Assert.AreEqual("ABC", message.GetData<string>("V1"));
            Assert.AreEqual(5, message.GetData<int>("V2"));
        }

        [TestMethod]
        public void TestDecode()
        {
            var map = new MessageMap(new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("type1", typeof(SampleObject)) });
            var message = new Message();
            message.SetData("type", "type1");
            message.SetData("V1", "ABC");
            message.SetData("V2", 5);
            var obj = map.Decode(message) as SampleObject;

            Assert.AreEqual("ABC", obj.V1);
            Assert.AreEqual(5, obj.V2);
        }

        [TestMethod]
        public void TestFromAssembly()
        {
            var messageMap = new MessageMap(Assembly.GetExecutingAssembly(), "CometGateway.Server.Gateway.Tests.SampleMessages");

            List<KeyValuePair<string, Type>> map = messageMap
                                                            .MessageTypeMap
                                                            .OrderBy(pair => pair.Key)
                                                            .ToList();
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual("type1", map[0].Key);
            Assert.AreEqual(typeof(SampleMessages.SampleMessage1), map[0].Value);
            Assert.AreEqual("type2", map[1].Key);
            Assert.AreEqual(typeof(SampleMessages.SampleMessage2), map[1].Value);            
        }
    }
}
