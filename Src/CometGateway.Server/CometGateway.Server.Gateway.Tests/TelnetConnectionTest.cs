using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class TelnetConnectionTest
    {
        [TestMethod]
        public void TelnetConnectionDoesAsStateMachineSays()
        {
            var mocks = new MockRepository();

            var socketConnection = mocks.StrictMock<ISocketConnection>();
            socketConnection.Expect(s => s.Send(new byte[] { 1, 2 }));
            socketConnection
                .Expect( c=> c.ServerDisconnected += null )
                .IgnoreArguments();
            socketConnection
                .Expect(c => c.ConnectionSucceeded += null)
                .IgnoreArguments();
            socketConnection
                .Expect(c => c.ErrorOccurred += null)
                .IgnoreArguments();
            socketConnection
                .Expect(c => c.DataReceived+= null)
                .IgnoreArguments();

            byte[] sendBack1;
            bool isRealData1;
            var telnetStateMachine = mocks.StrictMock<ITelnetStateMachine>();
            telnetStateMachine
                    .Expect(ts => ts.HandleIncomingByte(30, out sendBack1, out isRealData1))
                    .OutRef(new byte[] { 1, 2 }, false);

            byte[] sendBack2;
            bool isRealData2;
            telnetStateMachine
                    .Expect(ts => ts.HandleIncomingByte(40, out sendBack2, out isRealData2))
                    .OutRef(new byte[] {}, true);

            mocks.ReplayAll();
            var connection = new TelnetConnection(socketConnection, telnetStateMachine);
            byte[] realData = connection.ConvertFromInternalFormat(new byte []{30, 40});
            mocks.VerifyAll();

            CollectionAssert.AreEqual(new byte[] { 40 }, realData);
        }
    }
}
