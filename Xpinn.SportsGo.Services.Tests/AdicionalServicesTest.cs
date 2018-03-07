using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class AdicionalServicesTest
    {
        [TestMethod]
        public async Task AdicionalService_ListarPeriodicidades_ShouldList()
        {
            AdicionalServices adicionalService = new AdicionalServices();

            List<PeriodicidadesDTO> lstPeriodicidades = await adicionalService.ListarPeriodicidades();

            Assert.IsNotNull(lstPeriodicidades);
            Assert.IsTrue(lstPeriodicidades.Count > 0);
            Assert.IsTrue(lstPeriodicidades.TrueForAll(x => x.Planes.Count == 0));
        }
    }
}
