using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class ConversionLayerTest
    {
        private class SampleConversionLayer : ProtocolLayer<string, string, string, string>
        {
            public SampleConversionLayer(IConnection<string, string> connection)
                : base(connection)
            {
            }


            internal override string ConvertToInternalFormat(string data)
            {
                Assert.AreEqual("abc", data);
                return "ABC";
            }

            internal override string ConvertFromInternalFormat(string data)
            {
                Assert.AreEqual("ABC", data);
                return "abc";
            }
        }

        [TestMethod]
        public void TestPassThroughMethods()
        {
            MockRepository mocks = new MockRepository();
            IConnection<string, string> stubConnection = mocks.DynamicMock<IConnection<string, string>>();

            stubConnection.Expect(c => c.StartConnect("server", 7));
            stubConnection.Expect(c => c.Connected).Return(true);                        
            stubConnection.Expect(c => c.StartDisconnect());
            stubConnection.Expect(c => c.Send("ABC"));


            mocks.ReplayAll();
            var conversionLayer = new SampleConversionLayer(stubConnection);
            conversionLayer.StartConnect("server", 7);
            Assert.IsTrue(conversionLayer.Connected);
            conversionLayer.Send("abc");
            conversionLayer.StartDisconnect();
            mocks.VerifyAll();
        }

        [TestMethod]
        public void TestPassThroughEvents()
        {
            MockRepository mocks = new MockRepository();
            IConnection<string, string> stubConnection = mocks.StrictMock<IConnection<string, string>>();

            var conversionLayer = new SampleConversionLayer(stubConnection);

            bool connectionSucceeded = false;
            conversionLayer.ConnectionSucceeded += () => { connectionSucceeded = true; };
            stubConnection.Raise(c => c.ConnectionSucceeded += null);
            Assert.IsTrue(connectionSucceeded);

            string data = null;
            conversionLayer.DataReceived += (x) => { data = x; };
            stubConnection.Raise(c => c.DataReceived += null, "ABC");
            Assert.AreEqual("abc", data);

            bool disconnected = false;
            conversionLayer.ServerDisconnected += () => { disconnected = true; };
            stubConnection.Raise(c => c.ServerDisconnected += null);
            Assert.IsTrue(disconnected);
        }
    }
}
