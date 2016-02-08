using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSA.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel.Tests
{
    [TestClass()]
    public class ServerTests
    {
        [TestMethod()]
        public void QueryServerTest()
        {
            Server server = new Server("192.168.0.147", 27015);
            //Server server = new Server("192.168.0.255", 0);
            server.QueryServerHeader();
            Assert.AreEqual("Brezna Arena".ToLower(), server.ServerModel.Name.ToLower());
        }

        [TestMethod()]
        public void QueryPlayerTest()
        {
            Server server = new Server("193.104.68.49", 27040);
            //Server server = new Server("192.168.0.147", 27015);
            server.QueryPlayers();
            foreach(var player in server.ServerModel.Players)
            {
                System.Diagnostics.Trace.WriteLine(player.ToString());
            }
            Assert.IsTrue(server.ServerModel.Players.Count > 0);
        }
    }
}