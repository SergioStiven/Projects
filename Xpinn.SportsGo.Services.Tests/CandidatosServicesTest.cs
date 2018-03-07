using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class CandidatosServicesTest
    {
        [TestMethod]
        public async Task CandidatosServices_BuscarCandidatoPorCodigoCandidato_ShouldSearch()
        {
            CandidatosServices candidatoService = new CandidatosServices();

            CandidatosDTO candidato = new CandidatosDTO
            {
                Consecutivo = 1
            };

            CandidatosDTO candidatoBuscado = await candidatoService.BuscarCandidatoPorCodigoCandidato(candidato);

            Assert.IsNotNull(candidatoBuscado);
        }

        [TestMethod]
        public async Task CandidatosServices_ListarCandidatos_ShouldList()
        {
            CandidatosServices candidatoService = new CandidatosServices();

            BuscadorDTO buscador = new BuscadorDTO
            {
                SkipIndexBase = 0,
                TakeIndexBase = 20,
                EstaturaInicial = 1,
                EstaturaFinal = 200
            };

            List<CandidatosDTO> listaCandidatos = await candidatoService.ListarCandidatos(buscador);

            Assert.IsNotNull(listaCandidatos);
            Assert.IsTrue(listaCandidatos.TrueForAll(x => x.CandidatosVideos.Count == 0));
            Assert.IsTrue(listaCandidatos.TrueForAll(x => x.CategoriasCandidatos.All(y => y.Candidatos == null && y.Categorias == null && y.HabilidadesCandidatos.Count == 0)));
            Assert.IsTrue(listaCandidatos.TrueForAll(x => x.Generos.Candidatos.Count == 0));
        }

        [TestMethod]
        public async Task CandidatosServices_ListarCandidatosVideos_ShouldList()
        {
            CandidatosServices candidatoService = new CandidatosServices();

            CandidatosVideosDTO buscador = new CandidatosVideosDTO
            {
                SkipIndexBase = 0,
                TakeIndexBase = 20,
                IdentificadorParaBuscar = "Bryan",
                CodigoCandidato = 5
            };

            List<CandidatosVideosDTO> listaCandidatos = await candidatoService.ListarCandidatosVideosDeUnCandidato(buscador);

            Assert.IsNotNull(listaCandidatos);
        }
    }
}