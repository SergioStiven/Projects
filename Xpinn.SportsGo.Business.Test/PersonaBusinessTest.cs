using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class PersonaBusinessTest
    {
        [TestMethod]
        public async Task PersonasBusiness_ModificarPersona_ShouldModify()
        {
            PersonasBusiness personaBusiness = new PersonasBusiness();

            Personas persona = new Personas
            {
                Consecutivo = 3,
                Nombres = "Bryan",
                Apellidos = "Sanchez",
                CodigoIdioma = 1,
                CodigoPais = 1,
                CiudadResidencia = "Bogota",
                Telefono = "3144781828",
                Skype = "miskype"
            };

            WrapperSimpleTypesDTO wrapper = await personaBusiness.ModificarPersona(persona);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }

        [TestMethod]
        public async Task PersonasBusiness_BuscarPersona_ShouldSearch()
        {
            PersonasBusiness personaBusiness = new PersonasBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            Personas persona = await secure.DecryptMessageToEntity<Personas>(@"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");

            PersonasDTO personaBuscada = await personaBusiness.BuscarPersona(persona);

            Assert.IsNotNull(personaBuscada);
        }
    }
}
