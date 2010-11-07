using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class TelnetStateMachineTest
    {
        [TestMethod]
        public void TelnetStateMachineReturnsByteForByte()
        {
            AssertCommandResult(new Byte[] { 5 }, 5);
        }

        [TestMethod]
        public void TelnetStateMachineEscapesIAC()
        {
            AssertCommandResult(new byte[] { 255 }, 255, 255);
        }

        private static void AssertCommandResult(
            Byte[] expectedResult, 
            params byte[] command
        )
        {
            TelnetStateMachine machine = new TelnetStateMachine();

            var answers =
                command
                    .Select(cmdByte => machine.Translate(cmdByte))
                    .Where(answer => answer != null)
                    .SelectMany(answer => answer)
                    .ToArray();

            CollectionAssert.AreEqual(expectedResult, answers);
        }

        [TestMethod]
        public void TelnetStateMachineIgnoresTwoByteIAC()
        {
            AssertCommandResult(new byte[] { 33 }, 255, 33);
        }

        [TestMethod]
        public void TelnetStateMachineAnswersDoWithWont()
        {
            //TODO values
            AssertCommandResult(new byte[] { 255, 252, 4 }, 255, 253, 4);
        }

        [TestMethod]
        public void TelnetStateMachineAnswersDontWithWont()
        {
            //TODO values
            AssertCommandResult(new byte[] { 255, 252, 4 }, 255, 254, 4);
        }

        [TestMethod]
        public void TelnetStateMachineIgnoresOptions()
        {
            AssertCommandResult(new byte[] {}, 255, 250, 12, 13, 255, 240);
        }

        [TestMethod]
        public void TelnetStateMachineEmptiesCommandAfterAnswer()
        {
            AssertCommandResult(new byte[] { 1, 2 }, 255, 1, 255, 2);
        }
    }
}
