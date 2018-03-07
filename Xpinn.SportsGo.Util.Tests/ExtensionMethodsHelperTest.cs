using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Util.Tests
{
    [TestClass]
    public class ExtensionMethodsHelperTest
    {
        [TestMethod]
        public void DateTimeHelper_DiferenciaEntreDosFechas_ShouldPositiveBeOne()
        {
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaComparar = DateTime.Now.AddMonths(1);

            long numeroMeses = Portable.HelperClasses.DateTimeHelper.DiferenciaEntreDosFechas(fechaComparar, fechaHoy);

            Assert.AreEqual(1, numeroMeses);
        }

        [TestMethod]
        public void DateTimeHelper_DiferenciaEntreDosFechas_ShouldBeNegativeOne()
        {
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaComparar = DateTime.Now.AddMonths(1);

            long numeroMeses = Portable.HelperClasses.DateTimeHelper.DiferenciaEntreDosFechas(fechaHoy, fechaComparar);

            Assert.AreEqual(-1, numeroMeses);
        }
    }
}
