using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;

namespace Xpinn.SportsGo.Util.Tests
{
    [TestClass]
    public class HasherCryptoServiceTest
    {
        [TestMethod]
        public void HasherCryptoService_GetStringHash_ShouldHash()
        {
            HasherCryptoService<MD5Cng> hashingService = new HasherCryptoService<MD5Cng>();

            string hashing = hashingService.GetStringHash("4Vj8eK4rloUd272L48hsrarnUA~508029~TestPayU~3~USD");

            Assert.IsNotNull(hashing);
            Assert.AreEqual(hashing, "ba9ffa71559580175585e45ce70b6c37");
        }
    }
}