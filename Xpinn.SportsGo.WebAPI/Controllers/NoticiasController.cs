using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class NoticiasController : ApiController
    {
        NoticiasBusiness _noticiasBusiness;

        public NoticiasController()
        {
            _noticiasBusiness = new NoticiasBusiness();
        }


        #region Metodos RssFeeds


        public async Task<IHttpActionResult> InteractuarRssFeed(List<RssFeeds> rssFeedParaInteractuar)
        {
            if (rssFeedParaInteractuar == null || rssFeedParaInteractuar.Count <= 0 || !rssFeedParaInteractuar.TrueForAll(x => x.CodigoIdioma > 0 && !string.IsNullOrWhiteSpace(x.UrlFeed)))
            {
                return BadRequest("rssFeedParaInteractuar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperInteractuarRssFeed = await _noticiasBusiness.InteractuarRssFeed(rssFeedParaInteractuar);

                return Ok(wrapperInteractuarRssFeed);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarRssFeed(RssFeeds rssFeedParaBuscar)
        {
            if (rssFeedParaBuscar == null || rssFeedParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("rssFeedParaBuscar vacio y/o invalido!.");
            }

            try
            {
                RssFeeds rssFeedBuscado = await _noticiasBusiness.BuscarRssFeed(rssFeedParaBuscar);

                return Ok(rssFeedBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarRssFeed()
        {
            try
            {
                List<RssFeeds> listaRssFeeds = await _noticiasBusiness.ListarRssFeed();

                return Ok(listaRssFeeds);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarRssFeed(RssFeeds rssFeedParaEliminar)
        {
            if (rssFeedParaEliminar == null || rssFeedParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("rssFeedParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarRssFeed = await _noticiasBusiness.EliminarRssFeed(rssFeedParaEliminar);

                return Ok(wrapperEliminarRssFeed);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos Noticias


        public async Task<IHttpActionResult> CrearNoticia(Noticias noticiaParaCrear)
        {
            if (noticiaParaCrear == null || noticiaParaCrear.CodigoTipoNoticia <= 0)
            {
                return BadRequest("noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.NoticiasContenidos == null || noticiaParaCrear.NoticiasContenidos.Count <= 0
                  || !noticiaParaCrear.NoticiasContenidos.All(x => x.CodigoIdioma > 0 && !string.IsNullOrWhiteSpace(x.Titulo)))
            {
                return BadRequest("NoticiasContenidos del noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.NoticiasPaises == null || noticiaParaCrear.NoticiasPaises.Count <= 0
                  || !noticiaParaCrear.NoticiasPaises.All(x => x.CodigoPais > 0))
            {
                return BadRequest("NoticiasPaises del noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.CategoriasNoticias == null || noticiaParaCrear.CategoriasNoticias.Count <= 0
                  || !noticiaParaCrear.CategoriasNoticias.All(x => x.CodigoCategoria > 0))
            {
                return BadRequest("CategoriasNoticias del noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.Archivos != null)
            {
                return BadRequest("Usa CrearArchivoStream en ArchivosService para crear el archivo o mataras la memoria del servidor!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearNoticia = await _noticiasBusiness.CrearNoticia(noticiaParaCrear);

                return Ok(wrapperCrearNoticia);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarNoticia(Noticias noticiaParaBuscar)
        {
            if (noticiaParaBuscar == null || noticiaParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("noticiaParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Noticias noticiaBuscada = await _noticiasBusiness.BuscarNoticia(noticiaParaBuscar);

                return Ok(noticiaBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarTimeLine(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<TimeLineNoticias> listaTimeLine = await _noticiasBusiness.ListarTimeLine(buscador);

                return Ok(listaTimeLine);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListaTimeLineNotificaciones(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma
                || buscador.ConsecutivoPersona <= 0 || buscador.TipoDePerfil == TipoPerfil.SinTipoPerfil || buscador.CodigoPlanUsuario <= 0)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<TimeLineNotificaciones> listaTimeLineNotificacion = await _noticiasBusiness.ListaTimeLineNotificaciones(buscador);

                return Ok(listaTimeLineNotificacion);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarNoticias(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<NoticiasDTO> listaNoticias = await _noticiasBusiness.ListarNoticias(buscador);

                return Ok(listaNoticias);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarNoticia(Noticias noticiaParaModificar)
        {
            if (noticiaParaModificar == null || noticiaParaModificar.Consecutivo <= 0)
            {
                return BadRequest("noticiaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarNoticia = await _noticiasBusiness.ModificarNoticia(noticiaParaModificar);

                return Ok(wrapperModificarNoticia);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarNoticia(Noticias noticiaParaEliminar)
        {
            if (noticiaParaEliminar == null || noticiaParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("noticiaParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarNoticia = await _noticiasBusiness.EliminarNoticia(noticiaParaEliminar);

                return Ok(wrapperEliminarNoticia);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarArchivoNoticia(Noticias noticiaArchivoParaEliminar)
        {
            if (noticiaArchivoParaEliminar == null || noticiaArchivoParaEliminar.Consecutivo <= 0 || noticiaArchivoParaEliminar.CodigoArchivo <= 0)
            {
                return BadRequest("noticiaArchivoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarArchivoNoticia = await _noticiasBusiness.EliminarArchivoNoticia(noticiaArchivoParaEliminar);

                return Ok(wrapperEliminarArchivoNoticia);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos NoticiasContenidos


        public async Task<IHttpActionResult> CrearNoticiasContenidos(List<NoticiasContenidos> noticiasContenidoParaCrear)
        {
            if (noticiasContenidoParaCrear == null || noticiasContenidoParaCrear.Count <= 0
                || !noticiasContenidoParaCrear.All(x => x.CodigoNoticia > 0 && !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0))
            {
                return BadRequest("noticiasContenidoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearNoticiasContenidos = await _noticiasBusiness.CrearNoticiasContenidos(noticiasContenidoParaCrear);

                return Ok(wrapperCrearNoticiasContenidos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarNoticiaContenidoPorConsecutivo(NoticiasContenidos noticiaContenidoParaBuscar)
        {
            if (noticiaContenidoParaBuscar == null || noticiaContenidoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("noticiaContenidoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                NoticiasContenidos noticiaContenidoBuscado = await _noticiasBusiness.BuscarNoticiaContenidoPorConsecutivo(noticiaContenidoParaBuscar);

                return Ok(noticiaContenidoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarNoticiasContenidosDeUnaNoticia(NoticiasContenidos noticiaContenidoParaListar)
        {
            if (noticiaContenidoParaListar == null || noticiaContenidoParaListar.CodigoNoticia <= 0)
            {
                return BadRequest("noticiaContenidoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<NoticiasContenidos> listaContenidoDeUnaNoticia = await _noticiasBusiness.ListarNoticiasContenidosDeUnaNoticia(noticiaContenidoParaListar);

                return Ok(listaContenidoDeUnaNoticia);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarNoticiaContenido(NoticiasContenidos noticiaContenidoParaModificar)
        {
            if (noticiaContenidoParaModificar == null || noticiaContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(noticiaContenidoParaModificar.Titulo))
            {
                return BadRequest("noticiaContenidoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarNoticiaContenido = await _noticiasBusiness.ModificarNoticiaContenido(noticiaContenidoParaModificar);

                return Ok(wrapperModificarNoticiaContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarMultiplesNoticiaContenido(List<NoticiasContenidos> noticiaContenidoParaModificar)
        {
            if (noticiaContenidoParaModificar == null || noticiaContenidoParaModificar.All(x => x.Consecutivo > 0 || !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0))
            {
                return BadRequest("noticiaContenidoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarMultiplesNoticiaContenido = await _noticiasBusiness.ModificarMultiplesNoticiaContenido(noticiaContenidoParaModificar);

                return Ok(wrapperModificarMultiplesNoticiaContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarNoticiaContenido(NoticiasContenidos noticiaContenidoParaEliminar)
        {
            if (noticiaContenidoParaEliminar == null || noticiaContenidoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("noticiaContenidoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarNoticiaContenido = await _noticiasBusiness.EliminarNoticiaContenido(noticiaContenidoParaEliminar);

                return Ok(wrapperEliminarNoticiaContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos NoticiasPaises


        public async Task<IHttpActionResult> CrearNoticiasPaises(List<NoticiasPaises> noticiaPaisParaCrear)
        {
            if (noticiaPaisParaCrear == null || noticiaPaisParaCrear.Count <= 0
                || !noticiaPaisParaCrear.All(x => x.CodigoNoticia > 0 && x.CodigoPais > 0))
            {
                return BadRequest("noticiaPaisParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearNoticiasPaises = await _noticiasBusiness.CrearNoticiasPaises(noticiaPaisParaCrear);

                return Ok(wrapperCrearNoticiasPaises);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarNoticiaPaisPorConsecutivo(NoticiasPaises noticiaPaisParaBuscar)
        {
            if (noticiaPaisParaBuscar == null || noticiaPaisParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("noticiaPaisParaBuscar vacio y/o invalido!.");
            }

            try
            {
                NoticiasPaises noticiaPaisBuscado = await _noticiasBusiness.BuscarNoticiaPaisPorConsecutivo(noticiaPaisParaBuscar);

                return Ok(noticiaPaisBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarNoticiasPaisesDeUnaNoticia(NoticiasPaises noticiaPaisesParaListar)
        {
            if (noticiaPaisesParaListar == null || noticiaPaisesParaListar.CodigoNoticia <= 0)
            {
                return BadRequest("noticiaPaisesParaListar vacio y/o invalido!.");
            }

            try
            {
                List<NoticiasPaises> listaPaisesDeUnaNoticia = await _noticiasBusiness.ListarNoticiasPaisesDeUnaNoticia(noticiaPaisesParaListar);

                return Ok(listaPaisesDeUnaNoticia);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarNoticiaPais(NoticiasPaises noticiaPaisParaEliminar)
        {
            if (noticiaPaisParaEliminar == null || noticiaPaisParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("noticiaPaisParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarNoticiaPais = await _noticiasBusiness.EliminarNoticiaPais(noticiaPaisParaEliminar);

                return Ok(wrapperEliminarNoticiaPais);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}