using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class AnunciantesServicesTest
    {
        [TestMethod]
        public async Task AnunciantesServices_CrearAnuncio_ShouldCreate()
        {
            AnunciantesServices anuncianteServices = new AnunciantesServices();

            AnunciosDTO anuncios = new AnunciosDTO
            {
                CodigoAnunciante = 1,
                Vencimiento = new DateTime(2020,1,1),
                NumeroApariciones = 1000,
                CodigoArchivo = 11,
                UrlPublicidad = "hollaaaa",
                AnunciosContenidos = new List<AnunciosContenidosDTO>
                {
                    new AnunciosContenidosDTO { Titulo = "holaa", CodigoIdioma = 1 },
                    new AnunciosContenidosDTO { Titulo = "hi", CodigoIdioma = 2 }
                },
                AnunciosPaises = new List<AnunciosPaisesDTO>
                {
                    new AnunciosPaisesDTO { CodigoPais = 1 }
                },
                CategoriasAnuncios = new List<CategoriasAnunciosDTO>
                {
                    new CategoriasAnunciosDTO { CodigoCategoria = 3 }
                }
            };

            WrapperSimpleTypesDTO wrapper = await anuncianteServices.CrearAnuncio(anuncios);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }

        [TestMethod]
        public async Task AnuncianteServices_EliminarAnuncio_ShouldDelete()
        {
            AnunciantesServices anuncianteServices = new AnunciantesServices();

            AnunciosDTO anuncioParaBorrar = new AnunciosDTO
            {
                Consecutivo = 7
            };

            WrapperSimpleTypesDTO wrapper = await anuncianteServices.EliminarAnuncio(anuncioParaBorrar);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }

        [TestMethod]
        public async Task AnunciantesServices_BuscaAnuncioPaisPorConsecutivo_ShouldList()
        {
            AnunciantesServices anuncianteServices = new AnunciantesServices();

            AnunciosDTO anuncio = new AnunciosDTO
            {
                Consecutivo = 3
            };

            AnunciosDTO anuncioBuscado = await anuncianteServices.BuscarAnuncioPorConsecutivo(anuncio);

            Assert.IsNotNull(anuncioBuscado);
        }

        [TestMethod]
        public async Task AnunciantesServices_ListarAnunciosDeUnAnunciante_ShouldList()
        {
            AnunciantesServices anuncianteServices = new AnunciantesServices();

            AnunciosDTO anuncio = new AnunciosDTO
            {
                SkipIndexBase = 0,
                TakeIndexBase = 20,
                CodigoAnunciante = 1,
                CodigoIdiomaUsuarioBase = 1
            };

            List<AnunciosDTO> lstAnuncios = await anuncianteServices.ListarAnunciosDeUnAnunciante(anuncio);

            Assert.IsNotNull(lstAnuncios);
            Assert.IsTrue(lstAnuncios.Count > 0);
            Assert.IsTrue(lstAnuncios.TrueForAll(x => x.Anunciantes == null));
            Assert.IsTrue(lstAnuncios.TrueForAll(x => x.Archivos == null));
        }

        [TestMethod]
        public async Task AnunciantesServices_ListarAnunciosContenidosDeUnAnuncio_ShouldList()
        {
            AnunciantesServices anuncianteServices = new AnunciantesServices();

            AnunciosContenidosDTO anuncio = new AnunciosContenidosDTO
            {
                CodigoAnuncio = 2
            };

            List<AnunciosContenidosDTO> lstAnunciosContenidos = await anuncianteServices.ListarAnunciosContenidosDeUnAnuncio(anuncio);

            Assert.IsNotNull(lstAnunciosContenidos);
            Assert.IsTrue(lstAnunciosContenidos.Count > 0);
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Anuncios == null));
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Idiomas.AnunciosContenidos.Count == 0));
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Idiomas.CategoriasContenidos.Count == 0));
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Idiomas.GruposEventos.Count == 0));
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Idiomas.HabilidadesContenidos.Count == 0));
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Idiomas.Paises.Count == 0));
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Idiomas.Personas.Count == 0));
            Assert.IsTrue(lstAnunciosContenidos.TrueForAll(x => x.Idiomas.PlanesContenidos.Count == 0));
        }

        [TestMethod]
        public async Task AnunciantesServices_ListarAnunciosPaisesDeUnAnuncio_ShouldList()
        {
            AnunciantesServices anuncianteServices = new AnunciantesServices();

            AnunciosPaisesDTO anuncio = new AnunciosPaisesDTO
            {
                CodigoAnuncio = 2
            };

            List<AnunciosPaisesDTO> lstAnunciosPaises = await anuncianteServices.ListarAnunciosPaisesDeUnAnuncio(anuncio);

            Assert.IsNotNull(lstAnunciosPaises);
            Assert.IsTrue(lstAnunciosPaises.Count > 0);
            Assert.IsTrue(lstAnunciosPaises.TrueForAll(x => x.Anuncios == null));
            Assert.IsTrue(lstAnunciosPaises.TrueForAll(x => x.Paises.Idiomas == null));
            Assert.IsTrue(lstAnunciosPaises.TrueForAll(x => x.Paises.Monedas == null));
            Assert.IsTrue(lstAnunciosPaises.TrueForAll(x => x.Paises.AnunciosPaises.Count == 0));
            Assert.IsTrue(lstAnunciosPaises.TrueForAll(x => x.Paises.GruposEventos.Count == 0));
            Assert.IsTrue(lstAnunciosPaises.TrueForAll(x => x.Paises.Personas.Count == 0));
        }
    }
}
