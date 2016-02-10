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
        string GlobalGamingNisAddress = "193.104.68.49:27040";
        string StyleIndungiAddress = "93.119.24.12:27015";
        /// <summary>
        /// Source server test
        /// </summary>
        [TestMethod()]
        public void QueryServerHeaderTest_Source()
        {
            Server server = new Server(GlobalGamingNisAddress);
            server.QueryServerHeader();
            Assert.AreEqual("Global Gaming Nis", server.ServerModel.Name);
        }

        [TestMethod()]
        public void QueryPlayerTest_Source()
        {
            Server server = new Server(GlobalGamingNisAddress);
            server.QueryPlayers();
            foreach (var player in server.ServerModel.Players)
            {
                System.Diagnostics.Trace.WriteLine(player.ToString());
            }
            Assert.IsTrue(server.ServerModel.Players.Count > 0);
        }

        /// <summary>
        /// GoldSource server test
        /// </summary>
        [TestMethod()]
        public void QueryServerHeaderTest_GoldSource()
        {
            Server server = new Server(StyleIndungiAddress);
            server.QueryServerHeader();
            Assert.AreEqual("STYLE.INDUNGI.RO # FATAL ERROR".ToLower(), server.ServerModel.Name.ToLower());
        }
    }
}