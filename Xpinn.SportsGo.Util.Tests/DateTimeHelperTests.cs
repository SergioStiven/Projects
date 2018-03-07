using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Util.HelperClasses;

namespace Xpinn.SportsGo.Util.Tests
{
    [TestClass]
    public class DateTimeHelperTests
    {
        [TestMethod]
        public void DateTimeHelper_ConvertDateTimeFromAnotherTimeZone_ShouldConvert_NegativePositive()
        {
            DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

            DateTime dateSource = new DateTime(2017, 9, 5, 16, 12, 0);

            DateTime newDate = helper.ConvertDateTimeFromAnotherTimeZone(-5, 2, dateSource);

            Assert.AreEqual(newDate, dateSource.AddHours(-7));

            //int numeroAnunciosPosibles = 3;

            //Random random = new Random();
            //int skipIndexRandom = random.Next(0, numeroAnunciosPosibles);

            //int diferenciaSkipConTotal = numeroAnunciosPosibles - skipIndexRandom;
            //if (diferenciaSkipConTotal < 4)
            //{
            //    skipIndexRandom -= 0 - diferenciaSkipConTotal;

            //    if (skipIndexRandom < 0)
            //    {
            //        skipIndexRandom = 0;
            //    }
            //}
        }

        [TestMethod]
        public void DateTimeHelper_ConvertDateTimeFromAnotherTimeZone_ShouldConvert_PositiveNegative()
        {
            DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

            DateTime dateSource = new DateTime(2017, 9, 5, 16, 12, 0); // Hora españa

            DateTime newDate = helper.ConvertDateTimeFromAnotherTimeZone(2, -5, dateSource);

            Assert.AreEqual(newDate, dateSource.AddHours(7));
        }

        [TestMethod]
        public void DateTimeHelper_ConvertDateTimeFromAnotherTimeZone_ShouldConvert_NegativeNegative()
        {
            DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

            DateTime dateSource = new DateTime(2017, 9, 5, 16, 12, 0); // Hora españa

            DateTime newDate = helper.ConvertDateTimeFromAnotherTimeZone(-4, -5, dateSource);

            Assert.AreEqual(newDate, dateSource.AddHours(1));
        }
    }
}
