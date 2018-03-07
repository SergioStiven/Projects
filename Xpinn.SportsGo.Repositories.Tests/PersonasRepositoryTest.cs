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
    public class PersonasRepositoryTest
    {
        [TestMethod]
        public async Task PersonasRepository_BuscarPersona_ShouldSearch()
        {
            using (SportsGoEntities context = new SportsGoEntities())
            {
                PersonasRepository personaService = new PersonasRepository(context);

                Personas persona = new Personas
                {
                    Consecutivo = 3,
                    IdiomaBase = Idioma.Español
                };

                PersonasDTO personaBuscada = await personaService.BuscarPersona(persona);

                Assert.IsNotNull(personaBuscada);
                Assert.IsNotNull(personaBuscada.Nombres);
                Assert.AreNotEqual(persona.Consecutivo, 0);
            }
        }
    }
}
