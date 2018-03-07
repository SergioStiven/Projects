using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using FreshMvvm;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class MetricasServicesTest
    {
        [TestMethod]
        public async Task MetricasServices_ListarUsuariosMetricas_ShouldList()
        {
            MetricasServices metricasServices = new MetricasServices();

            MetricasDTO metricas = new MetricasDTO
            {
                SkipIndexBase = 0,
                TakeIndexBase = 5
            };

            List<PersonasDTO> listaUsuario = await metricasServices.ListarUsuariosMetricas(metricas);

        }
    }
}
