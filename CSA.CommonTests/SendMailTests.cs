using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.CommonTests
{
    [TestClass()]
    public class SendMailTests
    {
        [TestMethod()]
        public void SendTest()
        {
            var credential = new System.Net.NetworkCredential("kanter.brezna@gmail.com", "ladan");
            SendMail sender = new SendMail("smtp.gmail.com", 25, credential);
                
            sender.Send("<nemanja.simovic@gmail.com>;<nemanja.simovic@brezna.info>", "Test " + DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            Assert.IsTrue(true);
        }
    }
}