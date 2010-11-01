using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CometGateway.Server.Gateway.Tests
{
    [TestClass]
    public class ConvertToTextTest
    {
        [TestMethod]
        public void TestConvertStringToBytes()
        {
            IConnection<byte[]> connection = MockRepository.GenerateStub<IConnection<byte[]>>();
            BytesToStringConversionLayer conversion = new BytesToStringConversionLayer(connection);
            CollectionAssert.AreEqual(new byte[] { 65, 66, 67 }, conversion.ConvertToInternalFormat("ABC"));
        }

        [TestMethod]
        public void TestConvertBytesToString()
        {
            IConnection<byte[]> connection = MockRepository.GenerateStub<IConnection<byte[]>>();
            BytesToStringConversionLayer conversion = new BytesToStringConversionLayer(connection);
            Assert.AreEqual("ABC", conversion.ConvertFromInternalFormat(new byte[] { 65, 66, 67 }));
        }
    }
}
