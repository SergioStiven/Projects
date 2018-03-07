using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using FreshMvvm;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class GruposServicesTest
    {
        [TestMethod]
        public async Task GruposServices_ListarEventos_ShouldSearch()
        {
            GruposServices gruposServices = new GruposServices();

            BuscadorDTO buscadorDTO = new BuscadorDTO
            {
                CategoriasParaBuscar = new List<int>
                    {
                        3
                    },
                EstaturaInicial = 0,
                EstaturaFinal = 0,
                PesoInicial = 0,
                PesoFinal = 0,
                SkipIndexBase = 0,
                TakeIndexBase = 5,
                IdiomaBase = Idioma.Español,
                IdentificadorParaBuscar = null,
                FechaInicio = new DateTime(1950, 1, 1),
                FechaFinal = new DateTime(2050, 1, 1)
            };

            List<GruposEventosDTO> grupoEventoBuscado = await gruposServices.ListarEventos(buscadorDTO);

            Assert.IsNotNull(grupoEventoBuscado);
        }

        [TestMethod]
        public async Task GruposServices_CrearGruposEventosAsistentes_ShouldCreate()
        {
            GruposServices gruposBusiness = new GruposServices();

            GruposEventosAsistentesDTO grupoEvento = new GruposEventosAsistentesDTO
            {
                CodigoEvento = 13,
                CodigoPersona = 7
            };

            WrapperSimpleTypesDTO wrapper = await gruposBusiness.CrearGruposEventosAsistentes(grupoEvento);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }
    }
}
