using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CometGateway.Server.TelnetDemo.Configuration;

namespace CometGateway.Server.TelnetDemo.Tests.Configuration
{
    [TestClass]
    public class ServersTest
    {
        [TestMethod]
        public void ServersTestDetectsServers()
        {
            TelnetServerConfiguration[] servers =
                new TelnetServerConfigurationParser("name1=server1:10,name2=server2:20")
                    .Parse();

            CollectionAssert.AreEquivalent(
                    new TelnetServerConfiguration[] { 
                        new TelnetServerConfiguration 
                        {
                            Name = "name1",
                            Server="server1",
                            Port=10
                        },
                        new TelnetServerConfiguration
                        {
                            Name = "name2",
                            Server = "server2",
                            Port=20
                        }
                    },
                    servers
                );
        }
    }
}
