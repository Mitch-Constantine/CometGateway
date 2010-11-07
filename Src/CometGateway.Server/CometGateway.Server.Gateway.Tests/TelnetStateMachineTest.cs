using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class TelnetStateMachineTest
    {
        [TestMethod]
        public void TelnetStateMachineReturnsByteForByte()
        {
            AssertCommandResult(
                new byte[] {},
                new byte[] { 5 }, 
                new byte[] { 5 }
            );
        }

        [TestMethod]
        public void TelnetStateMachineEscapesIAC()
        {
            AssertCommandResult(
                new byte[] { }, 
                new byte[] { 255 },
                new byte[] { 255, 255 }
            );
        }

        [TestMethod]
        public void TelnetStateMachineIgnoresTwoByteIAC()
        {
            AssertCommandResult(
                new byte[] {}, 
                new byte[] {}, 
                new byte[] { 255, 33 });
        }

        [TestMethod]
        public void TelnetStateMachineAnswersDoWithWont()
        {
            AssertCommandResult(
                new byte[] { 255, 252, 4 }, 
                new byte[] {},
                new byte[] { 255, 253, 4 }
            );
        }

        [TestMethod]
        public void TelnetStateMachineAnswersDontWithWont()
        {
            AssertCommandResult(
                new byte[] { 255, 252, 4 }, 
                new byte[] {},
                new byte[] { 255, 254, 4 }
            );
        }

        [TestMethod]
        public void TelnetStateMachineIgnoresOptions()
        {
            AssertCommandResult(
                new byte[] {}, 
                new byte[] {},
                new byte[] { 255, 250, 12, 13, 255, 240 }
            );
        }

        [TestMethod]
        public void TelnetStateMachineEmptiesCommandAfterAnswer()
        {
            AssertCommandResult(
                new byte[] {},
                new byte[] {},
                new byte[] { 255, 1, 255, 2 }
            );
        }

        private static void AssertCommandResult(
            byte[] expectedSendBack,
            byte[] expectedRealData,
            byte[] command
        )
        {
            TelnetStateMachine machine = new TelnetStateMachine();
            TelnetConnection connection = new TelnetConnection(
                    null,
                    machine
                );

            List<byte> actualSendBack;
            List<byte> actualRealData;
            connection.Process(
                    command,
                    out actualSendBack,
                    out actualRealData
                );

            CollectionAssert.AreEqual(expectedSendBack, actualSendBack);
            CollectionAssert.AreEqual(expectedRealData, actualRealData);
        }
    }
}
