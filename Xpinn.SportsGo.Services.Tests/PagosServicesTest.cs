using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class PagosServicesTest
    {
        [TestMethod]
        public async Task PagosServices_ModificarMoneda_ShouldModify()
        {
            PagosServices pagosService = new PagosServices();

            MonedasDTO moneda = new MonedasDTO()
            {
                Consecutivo = 2,
                AbreviaturaMoneda = "COP",
                CambioMoneda = 2
            };

             WrapperSimpleTypesDTO wrapper = await pagosService.ModificarMoneda(moneda);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }
    }
}
