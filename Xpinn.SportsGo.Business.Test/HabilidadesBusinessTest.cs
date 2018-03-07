using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.DomainEntities;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class HabilidadesBusinessTest
    {
        [TestMethod]
        public async Task HabilidadesBusiness_ModificarHabilidad_ShouldModify()
        {
            HabilidadesBusiness habilidadBusiness = new HabilidadesBusiness();

            Habilidades habilidad = new Habilidades
            {
                Consecutivo = 1,
                TipoHabilidad = TipoHabilidad.Tecnica,
                HabilidadesContenidos = new List<HabilidadesContenidos>
                {
                    new HabilidadesContenidos { Consecutivo = 1, CodigoHabilidad = 1, CodigoIdioma = 1, Descripcion = "Ataque" },
                    new HabilidadesContenidos { Consecutivo = 2, CodigoHabilidad = 1, CodigoIdioma = 2, Descripcion = "Attack" }
                }
            };


            WrapperSimpleTypesDTO wrapper = await habilidadBusiness.ModificarHabilidad(habilidad);
        }
    }
}
