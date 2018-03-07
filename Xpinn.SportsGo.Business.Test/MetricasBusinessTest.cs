using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.DomainEntities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class MetricasBusinessTest
    {
        [TestMethod]
        public async Task MetricasBusiness_MetricasUsuarios_ShouldDoMetrics()
        {
            MetricasBusiness metricasBusiness = new MetricasBusiness();

            MetricasDTO metricasParaBuscar = new MetricasDTO
            {
                PlanesParaBuscar = new List<int> { 18 }
            };

            MetricasDTO metricasBuscada = await metricasBusiness.MetricasUsuarios(metricasParaBuscar);

            Assert.IsNotNull(metricasBuscada);
        }

        [TestMethod]
        public async Task MetricasBusiness_ListarUsuariosMetricas_ShouldList()
        {
            MetricasBusiness metricasBusiness = new MetricasBusiness();

            MetricasDTO metricasParaBuscar = new MetricasDTO
            {
                NombrePersonaParaBuscar = "Grupo",
                SkipIndexBase = 0,
                TakeIndexBase = 10,
                ZonaHorariaGMTBase = -5
            };

            List<PersonasDTO> metricasBuscada = await metricasBusiness.ListarUsuariosMetricas(metricasParaBuscar);

            Assert.IsNotNull(metricasBuscada);
        }

        [TestMethod]
        public async Task MetricasBusiness_NumeroUsuariosRegistradosUltimoMes_ShouldDoMetrics()
        {
            MetricasBusiness metricasBusiness = new MetricasBusiness();

            WrapperSimpleTypesDTO wrapper = await metricasBusiness.NumeroUsuariosRegistradosUltimoMes();

            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task MetricasBusiness_NumeroAnunciosRegistradosUltimoMes_ShouldDoMetrics()
        {
            MetricasBusiness metricasBusiness = new MetricasBusiness();

            WrapperSimpleTypesDTO wrapper = await metricasBusiness.NumeroAnunciosRegistradosUltimoMes(new MetricasDTO());

            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task MetricasBusiness_NumeroEventosRegistradosUltimoMes_ShouldDoMetrics()
        {
            MetricasBusiness metricasBusiness = new MetricasBusiness();

            WrapperSimpleTypesDTO wrapper = await metricasBusiness.NumeroEventosRegistradosUltimoMes();

            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task MetricasBusiness_NumeroVecesClickeadosUltimoMes_ShouldDoMetrics()
        {
            MetricasBusiness metricasBusiness = new MetricasBusiness();

            MetricasDTO metricas = new MetricasDTO
            {
                CodigoAnunciante = 1
            };

            WrapperSimpleTypesDTO wrapper = await metricasBusiness.NumeroVecesClickeadosUltimoMes(metricas);

            Assert.IsNotNull(wrapper);
        }
    }
}
