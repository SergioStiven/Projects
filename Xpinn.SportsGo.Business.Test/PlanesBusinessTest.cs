using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class PlanesBusinessTest
    {
        [TestMethod]
        public async Task PlanesBusiness_ListarPlanesPorIdioma_ShouldList()
        {
            PlanesBusiness planesBusiness = new PlanesBusiness();

            Planes plan = new Planes
            {
                SkipIndexBase = 0,
                TakeIndexBase = 20,
                CodigoPaisParaBuscarMoneda = 1,
                IdiomaBase = Idioma.Español,
                TipoPerfil = TipoPerfil.Anunciante
            };

            List<PlanesDTO> listaPlanes = await planesBusiness.ListarPlanesPorIdioma(plan);

            Assert.IsNotNull(listaPlanes);
        }

        [TestMethod]
        public async Task PlanesBusiness_BuscarPlanUsuario_ShouldSearch()
        {
            PlanesBusiness planesBusiness = new PlanesBusiness();

            PlanesUsuarios plan = new PlanesUsuarios
            {
                Consecutivo = 13,
                IdiomaBase = Idioma.Español
            };

            PlanesUsuariosDTO planUsuario = await planesBusiness.BuscarPlanUsuario(plan);

            Assert.IsNotNull(planUsuario);
        }

        [TestMethod]
        public async Task PlanesBusiness_EliminarPlan_ShouldDelete()
        {
            PlanesBusiness planesBusiness = new PlanesBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            Planes plan = new Planes
            {
                Consecutivo = 13,
                CodigoArchivo = 2344
            };

            WrapperSimpleTypesDTO wrapper = await planesBusiness.EliminarPlan(plan);

            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task PlanesBusiness_VerificarSiPlanSoportaLaOperacion_ShouldVerify()
        {
            PlanesBusiness planBusiness = new PlanesBusiness();

            PlanesUsuarios planUsuario = new PlanesUsuarios
            {
                Consecutivo = 2,
                TipoOperacionBase = TipoOperacion.MultiplesCategorias
            };

            WrapperSimpleTypesDTO wrapper = await planBusiness.VerificarSiPlanSoportaLaOperacion(planUsuario);

            Assert.IsNotNull(wrapper);
        }
    }
}
