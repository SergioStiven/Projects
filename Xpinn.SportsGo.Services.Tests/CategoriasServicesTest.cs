using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using FreshMvvm;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;
using System.Collections.Generic;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class CategoriasServicesTest
    {
        [TestMethod]
        public async Task CategoriasServices_CrearCategoriaCandidatos_ShouldCreate()
        {
            CategoriasServices categoriaService = new CategoriasServices();

            CategoriasCandidatosDTO categoriaCandidato = new CategoriasCandidatosDTO
            {
                CodigoCandidato = 5,
                CodigoCategoria = 3,
                HabilidadesCandidatos = new List<HabilidadesCandidatosDTO>
                {
                    new HabilidadesCandidatosDTO {  CodigoHabilidad = 2, NumeroEstrellas = 3}
                }
            };

            WrapperSimpleTypesDTO wrapper = await categoriaService.CrearCategoriaCandidatos(categoriaCandidato);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }
    }
}
