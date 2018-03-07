using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.DomainEntities;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class GruposBusinessTest
    {
        [TestMethod]
        public async Task GruposBusiness_BuscarGrupoEventoPorConsecutivo_ShouldSearch()
        {
            GruposBusiness gruposBusiness = new GruposBusiness();

            GruposEventos grupoEvento = new GruposEventos
            {
                Consecutivo = 2,
                Idioma = Idioma.Español
            };

            GruposEventosDTO grupoEventoBuscado = await gruposBusiness.BuscarGrupoEventoPorConsecutivo(grupoEvento);

            Assert.IsNotNull(grupoEventoBuscado);
        }

        [TestMethod]
        public async Task GruposBusiness_ListarEventos_ShouldSearch()
        {
            GruposBusiness gruposBusiness = new GruposBusiness();

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
                IdentificadorParaBuscar = string.Empty
                //FechaInicio = new DateTime,
                //FechaFinal = FechaFinal
            };

            List<GruposEventosDTO> grupoEventoBuscado = await gruposBusiness.ListarEventos(buscadorDTO);

            Assert.IsNotNull(grupoEventoBuscado);
        }

        [TestMethod]
        public async Task GruposBusiness_CrearGrupoEvento_ShouldCreate()
        {
            GruposBusiness gruposBusiness = new GruposBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            GruposEventos grupoEvento = await secure.DecryptMessageToEntity<GruposEventos>(@"f886TdSackKFkuUgmcuDFU7GIkIUlxeSc88k9qRaorfvJOZDWrH0VAJ/asGVKGBLEtZ+YYqJv3HhBHrHc4CcG/ZWm61PVOtrxJVEieAiK4agCwOvp1422Ovwx5Jv7b0z8Tsr2YwPJPBwh2MmDpK+az4KwgeYQyV/JgDS8DFrqh5nTaWqHyVIaIYXN8nwnZ/e5V39D0Qy1om0rnBk2H+aKPc0HhFwrZDphnyacwWgYmN3CYe9qY4O9s8ZIRgcaBpneX45+CAbYo0Xb4fLY9BDy1VFT94QD2l4uQ73dhDJxDrIOhTFJKgBifswz14DlQd+E7yxFVcmXfIB0xNwH4N0j3E+RMGVwFyMlbhTxxfGPaI+NxUtP+Kn/250c9xpwWqnhjEt1BMBbi9tfqmyvHNRgaEZdMa2K0MOrO5VumXTRQT+ks4BCudPM3nrR0JVM14jSQeQIvMEhK530Zydp6buQane/vHLGcEG0r3adrpaRVXKseBjM1D1O6PHwMsjqXPKNe2Aex9/ZnZAfjOIUtXCJOHcvzHuBvDc4zG0suaFuQy2qKwPWqq8LK4Lrs5Xs/mUA+UHHVj5O1TW5yY9KK4YnrHXlhLtDDnygEdsmEVuWuAeRDn1/PZaY4nup/c7LQKsI3ezKMTiAtIMmJLtLRAJ9lkDYu6+peHQCEhcZdwMPDIF61EwuB0RFIBYDuKpAzEBHjhSIZy9wt/RNHk5I3zT1CNtvR03PMCRarHSDY5Si4DL5EZWOt4kzaccja1wXCNfJDkmmSZN1iPX0u79tom4Va0WcuZ53dw5aEtVlFipUnfnUZC2pCYPVGc63DX9vnHlDkRbBq220wPIV7aQhqj3VkuW3t53Kkysfshx/AZVwCp8cR/hwhaQK9CE6VWxN6koCsoF4iVzAUuMe2tsjOeEyjgmejN1n4MhOcRSKtZ9hBpfKdpqCsdBgSXr5O6bgs0rNnCqrC0beRJhtFf0IZgWuFHows6Jt1D7b6IKREoCnYAe8ZAfvIuAxjNs5nUN5Bvv");

            WrapperSimpleTypesDTO wrapper = await gruposBusiness.CrearGrupoEvento(grupoEvento);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }

        [TestMethod]
        public async Task GruposBusiness_EliminarGrupoEvento_ShouldDelete()
        {
            GruposBusiness gruposBusiness = new GruposBusiness();

            GruposEventos grupoEvento = new GruposEventos
            {
                Consecutivo = 2045
            };

            WrapperSimpleTypesDTO wrapper = await gruposBusiness.EliminarGrupoEvento(grupoEvento);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }

        [TestMethod]
        public async Task GruposBusiness_CrearGruposEventosAsistentes_ShouldCreate()
        {
            GruposBusiness gruposBusiness = new GruposBusiness();

            GruposEventosAsistentes grupoEvento = new GruposEventosAsistentes
            {
                CodigoEvento = 13,
                CodigoPersona = 7
            };

            Tuple<WrapperSimpleTypesDTO,TimeLineNotificaciones> wrapper = await gruposBusiness.CrearGruposEventosAsistentes(grupoEvento);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Item1.Exitoso);
        }

        [TestMethod]
        public async Task GruposBusiness_CrearGrupo_ShouldCreate()
        {
            GruposBusiness gruposBusiness = new GruposBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            Grupos grupo = await secure.DecryptMessageToEntity<Grupos>(@"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");

            WrapperSimpleTypesDTO wrapper = await gruposBusiness.CrearGrupo(grupo, null, null);
            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }
    }
}
