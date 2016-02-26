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
    public class UtilTests
    {
        [TestMethod()]
        public void EncryptTest()
        {
            string expected = "Hello World!";
            string encrypted = CSA.Common.Util.Encrypt(expected);
            string actual = CSA.Common.Util.Decrypt(encrypted);
            Assert.AreEqual(expected, actual);
        }
    }
}