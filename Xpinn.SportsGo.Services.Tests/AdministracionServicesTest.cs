using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class AdministracionServicesTest
    {
        [TestMethod]
        public async Task AdministracionServices_BuscarPais_ShouldSearch()
        {
            AdministracionServices adminServices = new AdministracionServices();

            PaisesDTO paisParaBuscar = new PaisesDTO
            {
                Consecutivo = 1
            };

            PaisesDTO paisBuscado = await adminServices.BuscarPais(paisParaBuscar);

            Assert.IsNotNull(paisBuscado);
            Assert.IsNotNull(paisBuscado.Monedas);
        }

        [TestMethod]
        public async Task AdministracionServices_CrearPais_ShouldCreate()
        {
            AdministracionServices adminServices = new AdministracionServices();

            PaisesDTO paisParaBuscar = new PaisesDTO
            {
                Consecutivo = 1
            };

            WrapperSimpleTypesDTO wrapper = await adminServices.CrearPais(paisParaBuscar);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }

    }
}
