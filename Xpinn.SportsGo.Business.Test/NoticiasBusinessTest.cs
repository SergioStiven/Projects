using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util;
using Xpinn.SportsGo.DomainEntities;
using System.Linq;
using System.Xml;
using System.ServiceModel.Syndication;
using SimpleFeedReader;
using System.IO;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class NoticiasBusinessTest
    {
        [TestMethod]
        public async Task NoticiasBusiness_ListarTimeLine_ShouldList()
        {
            NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

            BuscadorDTO buscadorDTO = new BuscadorDTO
            {
                IdiomaBase = Idioma.Español,
                SkipIndexBase = 0,
                TakeIndexBase = 8,
            };

            List<TimeLineNoticias> listaTimeLine = await noticiasBusiness.ListarTimeLine(buscadorDTO);

            Assert.IsNotNull(listaTimeLine);
        }

        [TestMethod]
        public void PruebaFeed()
        {
            string url = "http://rss.cnn.com/rss/cnn_topstories.rss";
            //string url = "https://www.nasa.gov/rss/dyn/breaking_news.rss";
            //string url = "http://www.codeguru.com/rss/csharp/";

            for (int i = 0; i < 5; i++)
            {
                var reader = new FeedReader(true);
                var items = reader.RetrieveFeed(url);
            }

            //foreach (SyndicationItem item in feed.Items)
            ////foreach (SyndicationItem item in rssFormater.Feed.Items)
            //{
            //    String subject = item.Title.Text;
            //    String summary = item.Summary.Text;
            //}
        }

        [TestMethod]
        public async Task NoticiasBusiness_ListarNoticias_ShouldList()
        {
            NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

            BuscadorDTO buscadorDTO = new BuscadorDTO
            {
                IdiomaBase = Idioma.Español,
                SkipIndexBase = 0,
                TakeIndexBase = 100,
            };

            List<NoticiasDTO> listaTimeLine = await noticiasBusiness.ListarNoticias(buscadorDTO);

            var hola = listaTimeLine.Where(x => x.Consecutivo == 2005).FirstOrDefault();

            Assert.IsNotNull(listaTimeLine);
        }

        [TestMethod]
        public async Task NoticiasBusiness_ListaTimeLineNotificacionese_ShouldList()
        {
            NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

            BuscadorDTO buscador = new BuscadorDTO
            {
                IdiomaBase = Idioma.Español,
                ConsecutivoPersona = 2,
                TipoDePerfil = TipoPerfil.Candidato,
                CodigoPlanUsuario = 6,
                SkipIndexBase = 0,
                TakeIndexBase = 5
            };

            List<TimeLineNotificaciones> listaTimeLine = await noticiasBusiness.ListaTimeLineNotificaciones(buscador);

            Assert.IsNotNull(listaTimeLine);
        }

        [TestMethod]
        public async Task NoticiasBusiness_CrearNoticia_ShouldCreate()
        {
            NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            Noticias noticias = await secure.DecryptMessageToEntity<Noticias>(@"ZbE1EJwSi4NW0GdgBaQaolFpmtlt/W7CyJlWnO6+H073Dx0LyH2oxsjZ/Jt4t6JNl0ZU5nZ6k0u9dK9/GOxz7ozqBByBcvc2vCZ47uJ+2IOa8KkE4T0vSalKqCZl2c7OgMrUTTILQmjbhb/YSTAwd0xoqdFDqTaxj4Ik23RG31LAYRiEON8wx8g1cQNam1ERBJa7qeQ9mMKTImc96UcPOtAzzhz5bo6o/Y7FRkPqLz12f3Q9vUm3i+8Ifo9p5XfkiHVO2KqJjeG2SBgz4zXF3OZPr/KMmmDRQjIQojEBUKW3inEXvYTn49tFFOo5A2jaSDT1vsbTkqjZRRGEojh9yt2fafItLHoV1wHGScQoOrrqNBLpiRFqWel89qi52RRLjWinFfUxgMO7JKPBuizp3nU1rcmM6+/O4/DvW7YOO6xxN8ilcgBzNdRklnCCLXkB1GpV/xS2juwuCOZ+XYtI5haEcGL7Xj7baLTzkfHJKV/gQm7NKmVRHLerqW5U/S/0ilWJpN77J0eUaAmASusoSYXXAcnl0+h5Yc7iYnNYV+NKOw80tKApP7BTxmVGXEivbqW/Tm4R2JrZwFlHR3JobM9BemW2vqZDonk6/rOtwgsSMVvnNPqm6BRE0INYApEm3SkDg75NhbUVg1X2Risaslt1xdRt5+HNtT7rDLRDsvKnQnIHLss7mjAGy6p7LAqZs/+/kn5MFId19trzvinmMcL4jnPhYO0DRqeAROM5cVS5pXNPorP4HSFDC+/ecPLQVMkRjdMgEQTNbuSIDFdtExRo03oOuJgbaWukNHLXC+BPowJQtk70cPovnIzM/IUq8/SS8WdEfvWyFfAauONE9U92qMb/6b/b6CDhutf7rnMsuZoPdAwQSCbnjOqPdLpsaq/FV7B+v36tbSWU1Qn4xdIzQGgSQgpq6x9iqqtl+sx3gGuXItuhqkNOM18czjeJT5Jfk9DJ3w9ldc/7ioe7dDDSxZ+msolPuzHPcImMaDGe2kc4hGPkmRWyq7ztJAjBggqOfLRYHrX00q62s6TvolcwMWMfRFOwEO4I6Dxv2wJFPIENIt26T8IpM1vUsIW51LHgUST/HrAuNDN1/qaMZNfZ35ZKCn9gjSnMCo8oEL6E1lJXhKHxVaLr+GdRZ4T5n3oaZwinPnJJ6w70j8jLmt7A7xoKKyolu69eYqDeLqMx1c9tpG2e3/Hnr5jWntsuxLczocUSgqx863imMYdlJsINtmeTEMGWOTIBE8rIdm+CESe8sN0voCjqiQPo1JKz2e+cTK0Dxe/K+gSGLLJbseyMu0uJwGe5g9kEM7oV9Vmql9+/xnDEiEt/P9G35iZUYAhiJdR1nkC4KVdK3xSCLLokHmSZZiCou0XJ7P3+sogSbybWPzoIsYNrukKm+qAACG9rQV14hCFuvlJmC3QlNtn0vhspxdCDLF7cHSCeuPkY+TKDqqF3kiY7Xpx22ISXwcj+SM9Mg+C2W3BjGw4NLvC5PsnxTAqLrk0LL0MApgtBMf0ytAaq06CyZAbjTuX15ACAFGZsUq/QtLNiSfiE5jjjNsCBqinw4bOiRy4hyrnIRrXtJCjlku2RUy53jz6VzSP0xtqcxGr8nteQmo9yQlzctsod4qLXmcbFA2hcmHLEuwXxbKs8oTXHQfPOvcC7ynpGPH9z3SKncz6TiX6igrOjo7XKr4Vf/ZGL5fysK4fGiqcx/Fben2tMICWf51eUvegJpI9+tXKn3CdqcRen8n/89w3ZX+rst7GaYQNpzpsiFT4rh5pBznLanoxGkJ1FNap6KTefrFUNrIiaEu+eH9VgFNrILPVK93zQIiALS1M=");

            WrapperSimpleTypesDTO wrapper = await noticiasBusiness.CrearNoticia(noticias);
            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task NoticiasBusiness_ModificarNoticia_ShouldModify()
        {
            NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            Noticias noticias = await secure.DecryptMessageToEntity<Noticias>(@"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");

            WrapperSimpleTypesDTO wrapper = await noticiasBusiness.ModificarNoticia(noticias);

            Assert.IsNotNull(wrapper);
        }


        [TestMethod]
        public async Task NoticiasBusiness_EliminarNoticia_ShouldDelete()
        {
            NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            Noticias noticia = new Noticias
            {
                Consecutivo = 5
            };

            WrapperSimpleTypesDTO wrapper = await noticiasBusiness.EliminarNoticia(noticia);

            Assert.IsNotNull(wrapper);
        }
    }
}
