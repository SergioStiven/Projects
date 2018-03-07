using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Repositories.Tests
{
    [TestClass]
    public class HabilidadesRepositoryTest
    {
        [TestMethod]
        public async Task CandidatoRepository_ListarHabilidadesPorIdioma_ShouldList()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                Habilidades habilidad = new Habilidades
                {
                    IdiomaBase = Idioma.Español
                };

                List<HabilidadesDTO> informacionCandidato = await habilidadesRepo.ListarHabilidadesPorIdioma(habilidad);

                 Assert.IsNotNull(informacionCandidato);
            }
        }
    }
}
