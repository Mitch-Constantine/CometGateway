using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CometGateway.Server.TelnetDemo.Tests
{
    public static class TestUtilities
    {
        public static void AssertStringsMatch(string expected, string actual)
        {
            var expectedLines = expected.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var actualLines = actual.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        
            foreach (var ndxLineCrt in Enumerable.Range(0, expectedLines.Length))
            {
                Assert.AreEqual(expectedLines[ndxLineCrt].Trim(), actualLines[ndxLineCrt].Trim());
            }

            Assert.AreEqual(expectedLines.Length, actualLines.Length);
        }
    }
}
