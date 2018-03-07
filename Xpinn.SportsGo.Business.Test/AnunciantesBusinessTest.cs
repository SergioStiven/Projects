using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class AnunciantesBusinessTest
    {
        [TestMethod]
        public async Task AnunciantesBusiness_CrearAnuncio_ShouldCreate()
        {
            AnunciantesBusiness anuncianteBusiness = new AnunciantesBusiness();

            Anuncios anuncios = new Anuncios
            {
                CodigoAnunciante = 1,
                Vencimiento = new DateTime(2020, 1, 1),
                NumeroApariciones = 1000,
                CodigoArchivo = 9,
                UrlPublicidad = "hollaaaa",
                AnunciosContenidos = new List<AnunciosContenidos>
                {
                    new AnunciosContenidos { Titulo = "holaa", CodigoIdioma = 1 },
                    new AnunciosContenidos { Titulo = "hi", CodigoIdioma = 2 }
                },
                AnunciosPaises = new List<AnunciosPaises>
                {
                    new AnunciosPaises { CodigoPais = 1 }
                },
                CategoriasAnuncios = new List<CategoriasAnuncios>
                {
                    new CategoriasAnuncios { CodigoCategoria = 3 }
                }
            };

            WrapperSimpleTypesDTO wrapper = await anuncianteBusiness.CrearAnuncio(anuncios);

            Assert.IsNotNull(wrapper);

        }

        [TestMethod]
        public async Task AnuncianteBusiness_EliminarAnuncio_ShouldDelete()
        {
            AnunciantesBusiness anuncianteBusiness = new AnunciantesBusiness();

            Anuncios anuncioParaBorrar = new Anuncios
            {
                Consecutivo = 3
            };

            WrapperSimpleTypesDTO wrapper = await anuncianteBusiness.EliminarAnuncio(anuncioParaBorrar);

            Assert.IsNotNull(wrapper);
            Assert.IsTrue(wrapper.Exitoso);
        }

        [TestMethod]
        public async Task AnunciantesBusiness_CrearAnuncios_ShouldCreate()
        {
            AnunciantesBusiness anuncianteBusiness = new AnunciantesBusiness();

            Anuncios anuncio = new Anuncios
            {
                CodigoAnunciante = 1,
                FechaInicio = DateTime.Now,
                Vencimiento = new DateTime(2017, 09, 15),
                CodigoTipoAnuncio = 1,
                UrlPublicidad = "https://www.twitch.tv/directory/game/League%20of%20Legends",
                AnunciosContenidos = new List<AnunciosContenidos>
                {
                    new AnunciosContenidos { Titulo = "Prueba de un anuncio para League of Legends :D", Descripcion = "La descripcion de una prueba de un anuncio que quizas funcione o quizas no", CodigoIdioma = 1 },
                    new AnunciosContenidos { Titulo = "Test for a advertising for League of Legends :D", Descripcion = "The description of an advertising test that could work or dont", CodigoIdioma = 2 }
                },
                AnunciosPaises = new List<AnunciosPaises>
                {
                    new AnunciosPaises { CodigoPais = 1 },
                    new AnunciosPaises { CodigoPais = 2 },
                    new AnunciosPaises { CodigoPais = 3 },
                    new AnunciosPaises { CodigoPais = 9 }
                }
            };

            WrapperSimpleTypesDTO wrapper = await anuncianteBusiness.CrearAnuncio(anuncio);

            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task AnunciantesBusiness_EliminarAnuncio_ShouldEliminate()
        {
            AnunciantesBusiness anuncianteBusiness = new AnunciantesBusiness();

            Anuncios anuncios = new Anuncios
            {
                Consecutivo = 4,
            };

            await anuncianteBusiness.EliminarAnuncio(anuncios);
        }

        [TestMethod]
        public async Task AnunciantesBusiness_CrearAnunciante_ShouldCreate()
        {
            AnunciantesBusiness anuncianteBusiness = new AnunciantesBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            // No funca ese json encriptado cambiar
            Anunciantes anuncios = await secure.DecryptMessageToEntity<Anunciantes>(@"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");

            WrapperSimpleTypesDTO wrapper = await anuncianteBusiness.CrearAnunciante(anuncios, null, null);
            Assert.IsNotNull(wrapper);
        }
    }
}
