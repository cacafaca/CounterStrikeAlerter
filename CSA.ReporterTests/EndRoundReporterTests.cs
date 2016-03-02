using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSA.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Reporter.Tests
{
    [TestClass()]
    public class EndRoundReporterTests
    {
        [TestMethod()]
        public void StartTest()
        {
            EndRoundReporter endRoundReporter = new EndRoundReporter();
            endRoundReporter.Start();
            while (!endRoundReporter.CancellationPending)
                System.Threading.Thread.Sleep(1000);
            //Assert.Fail();
        }

        [TestMethod()]
        public void EndRoundReporterTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void StopTest()
        {
            Assert.Fail();
        }
    }
}