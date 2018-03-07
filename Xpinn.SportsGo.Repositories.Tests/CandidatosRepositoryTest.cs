using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using System.Linq;

namespace Xpinn.SportsGo.Repositories.Tests
{
    [TestClass]
    public class CandidatosRepositoryTest
    {
        [TestMethod]
        public async Task CandidatoRepository_BuscarInformacionCandidato_ShouldVerify()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatoRepository = new CandidatosRepository(context);
                Candidatos candidatoABuscar = new Candidatos();
                candidatoABuscar.Personas = new Personas();
                candidatoABuscar.Personas.Consecutivo = 8;

                Candidatos informacionCandidato = await candidatoRepository.BuscarCandidatoPorCodigoPersona(candidatoABuscar);

                Assert.IsNotNull(informacionCandidato);
                Assert.AreNotEqual(informacionCandidato.Consecutivo, 0);
                Assert.AreNotEqual(informacionCandidato.CodigoPersona, 0);
                Assert.AreNotEqual(informacionCandidato.CodigoGenero, 0);
                Assert.AreNotEqual(informacionCandidato.Estatura, 0);
                Assert.AreNotEqual(informacionCandidato.Peso, 0);
                Assert.AreNotEqual(informacionCandidato.FechaNacimiento, DateTime.MinValue);
            }
        }

        [TestMethod]
        public async Task CandidatosServices_ListarCandidatos_ShouldList()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);

                BuscadorDTO buscador = new BuscadorDTO
                {
                    SkipIndexBase = 0,
                    TakeIndexBase = 20,
                    EstaturaInicial = 1,
                    EstaturaFinal = 200
                };

                List<CandidatosDTO> listaCandidatos = await candidatosRepo.ListarCandidatos(buscador);

                Assert.IsNotNull(listaCandidatos);
                Assert.IsTrue(listaCandidatos.TrueForAll(x => x.CandidatosVideos.Count == 0));
                Assert.IsTrue(listaCandidatos.TrueForAll(x => x.CategoriasCandidatos.All(y => y.Candidatos == null && y.Categorias == null && y.HabilidadesCandidatos.Count == 0)));
                Assert.IsTrue(listaCandidatos.TrueForAll(x => x.Generos == null || x.Generos.Candidatos.Count == 0));
            }
        }

        [TestMethod]
        public void CandidatoRepository_CrearCandidato_ShouldCreate()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction dbContextTransaction = context.Database.BeginTransaction())
            {
                CandidatosRepository candidatoRepository = new CandidatosRepository(context);
                Candidatos candidatoParaCrear = new Candidatos
                {
                    CodigoPersona = 8,
                    CodigoGenero = 2,
                    Estatura = 170,
                    Peso = 60,
                    Biografia = "Yo ser Niño MVC Angular y no leer comentarios",
                    FechaNacimiento = new DateTime(1995, 11, 9)
                };

                candidatoRepository.CrearCandidato(candidatoParaCrear);

                //Assert.IsNotNull(wrapperCrearCandidato);
                //Assert.IsTrue(wrapperCrearCandidato.Exitoso);
                //Assert.AreNotEqual(wrapperCrearCandidato.NumeroRegistrosAfectados, 0);
            }
        }

        [TestMethod]
        public async Task CandidatoRepository_ModificarCandidato_ShouldModify()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction dbContextTransaction = context.Database.BeginTransaction())
            {
                CandidatosRepository candidatoRepository = new CandidatosRepository(context);
                Candidatos candidatoParaCrear = new Candidatos
                {
                    Consecutivo = 4,
                    CodigoPersona = 8,
                    CodigoGenero = 1,
                    Estatura = 180,
                    Peso = 70,
                    Biografia = "Yo ser Niño MVC Angular y no leer comentarios trollo yo ser",
                    FechaNacimiento = new DateTime(1995, 11, 9),
                };

                var hola = context.Personas.Find(8);
                candidatoParaCrear.Personas = hola;
                candidatoParaCrear.Personas.Nombres = "Sergioo";

                Candidatos candidatoExistente = await candidatoRepository.ModificarInformacionCandidato(candidatoParaCrear);

                Assert.IsNotNull(candidatoExistente);
            }
        }
    }
}
