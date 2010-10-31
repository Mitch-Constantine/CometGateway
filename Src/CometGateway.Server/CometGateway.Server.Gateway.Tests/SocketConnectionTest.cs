using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class SocketConnectionTest
    {
        [TestMethod]
        public void TestConnect()
        {
            var ev = new AutoResetEvent(false);
            var succeeded = false;
            var communicationLayer = new SocketConnection();
            communicationLayer.ConnectionSucceeded += () =>
            {
                succeeded = true;
                ev.Set();
            };
            communicationLayer.ErrorOccurred += (msg) =>
            {
                succeeded = false;
                ev.Set();
            };
            communicationLayer.StartConnect("localhost", 80);
            ev.WaitOne(5000);
            Assert.IsTrue(succeeded);
            Assert.IsTrue(communicationLayer.Connected);
            communicationLayer.StartDisconnect();
        }

        [TestMethod]
        public void TestConnectFailed()
        {
            var ev = new AutoResetEvent(false);
            var failed = false;
            var communicationLayer = new SocketConnection();
            communicationLayer.ConnectionSucceeded += () => ev.Set();
            communicationLayer.ErrorOccurred += (msg) =>
            {
                failed = true;
                ev.Set();
            };
            communicationLayer.StartConnect("localhost", 99);
            ev.WaitOne(5000);
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestDisconnect()
        {
            var ev = new AutoResetEvent(false);
            var succeeded = false;
            var communicationLayer = new SocketConnection();
            communicationLayer.ConnectionSucceeded += () =>
            {
                succeeded = true;
                ev.Set();
            };
            communicationLayer.ErrorOccurred += (msg) =>
            {
                succeeded = false;
                ev.Set();
            };
            communicationLayer.StartConnect("localhost", 80);
            ev.WaitOne(5000);
            Assert.IsTrue(succeeded);

            var disconnectEv = new AutoResetEvent(false);
            communicationLayer.ServerDisconnected += () => disconnectEv.Set();
            communicationLayer.StartDisconnect();

            disconnectEv.WaitOne(5000);
            Assert.IsFalse(communicationLayer.Connected);
        }
    }
}
