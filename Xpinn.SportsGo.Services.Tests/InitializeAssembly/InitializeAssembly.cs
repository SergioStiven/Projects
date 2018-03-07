using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FreshMvvm;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class InitializeAssemblyTest
    {
        [AssemblyInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            FreshIOC.Container.Register<ISecureableMessage, SecureMessagesHelper>();
        }
    }
}
