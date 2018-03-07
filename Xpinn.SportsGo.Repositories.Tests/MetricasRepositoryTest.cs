using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Repositories.Tests
{
    [TestClass]
    public class MetricasRepositoryTest
    {
        [TestMethod]
        public async Task MetricasRepository_ListarUsuariosMetricas_ShouldList()
        {
            using (SportsGoEntities context = new SportsGoEntities())
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);

                MetricasDTO metricas = new MetricasDTO
                {
                    SkipIndexBase = 0,
                    TakeIndexBase = 10
                };

                List<PersonasDTO> listaPersonas = await metricasRepo.ListarUsuariosMetricas(metricas);

                Assert.IsNotNull(listaPersonas);
            }

        }
    }
}
