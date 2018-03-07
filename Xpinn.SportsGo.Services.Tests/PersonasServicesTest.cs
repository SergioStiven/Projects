using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using FreshMvvm;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class PersonasServicesTest
    {
        [TestMethod]
        public async Task PersonasServices_BuscarPersona_ShouldSearch()
        {
            PersonasServices personaService = new PersonasServices();

            PersonasDTO persona = new PersonasDTO
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
