using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services
{
    public class NoticiasServices : BaseService
    {

        #region Metodos RssFeeds


        public async Task<WrapperSimpleTypesDTO> InteractuarRssFeed(List<RssFeedsDTO> rssFeedParaInteractuar)
        {
            if (rssFeedParaInteractuar == null)
            {
                throw new ArgumentNullException("rssFeedParaInteractuar vacio y/o invalido!.");
            }
            if (rssFeedParaInteractuar.Count <= 0 || !rssFeedParaInteractuar.TrueForAll( x=> x.CodigoIdioma > 0 && !string.IsNullOrWhiteSpace(x.UrlFeed)))
            {
                throw new ArgumentException("rssFeedParaInteractuar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperInteractuarRssFeed = await client.PostAsync<List<RssFeedsDTO>, WrapperSimpleTypesDTO>("Noticias/InteractuarRssFeed", rssFeedParaInteractuar);

            return wrapperInteractuarRssFeed;
        }

        public async Task<RssFeedsDTO> BuscarRssFeed(RssFeedsDTO rssFeedParaBuscar)
        {
            if (rssFeedParaBuscar == null) throw new ArgumentNullException("No puedes buscar un rssFeed si rssFeedParaBuscar es nulo!.");
            if (rssFeedParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("rssFeedParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            RssFeedsDTO rssFeedBuscado = await client.PostAsync("Noticias/BuscarRssFeed", rssFeedParaBuscar);

            return rssFeedBuscado;
        }

        public async Task<List<RssFeedsDTO>> ListarRssFeed()
        {
            IHttpClient client = ConfigurarHttpClient();

            List<RssFeedsDTO> listaRssFeeds = await client.PostAsync<List<RssFeedsDTO>>("Noticias/ListarRssFeed");

            return listaRssFeeds;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarRssFeed(RssFeedsDTO rssFeedParaEliminar)
        {
            if (rssFeedParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un rssFeed si rssFeedParaEliminar es nulo!.");
            if (rssFeedParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("rssFeedParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarRssFeed = await client.PostAsync<RssFeedsDTO, WrapperSimpleTypesDTO>("Noticias/EliminarRssFeed", rssFeedParaEliminar);

            return wrapperEliminarRssFeed;
        }

        #endregion


        #region Metodos Noticias


        public async Task<WrapperSimpleTypesDTO> CrearNoticia(NoticiasDTO noticiaParaCrear)
        {
            if (noticiaParaCrear == null)
            {
                throw new ArgumentNullException("noticiaParaCrear vacio y/o invalido!.");
            }
            if (noticiaParaCrear.CodigoTipoNoticia <= 0)
            {
                throw new ArgumentException("noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.NoticiasContenidos == null || noticiaParaCrear.NoticiasContenidos.Count <= 0
                  || !noticiaParaCrear.NoticiasContenidos.All(x => x.CodigoIdioma > 0 && !string.IsNullOrWhiteSpace(x.Titulo)))
            {
                throw new ArgumentException("NoticiasContenidos del noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.NoticiasPaises == null || noticiaParaCrear.NoticiasPaises.Count <= 0
                  || !noticiaParaCrear.NoticiasPaises.All(x => x.CodigoPais > 0))
            {
                throw new ArgumentException("NoticiasPaises del noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.CategoriasNoticias == null || noticiaParaCrear.CategoriasNoticias.Count <= 0
                  || !noticiaParaCrear.CategoriasNoticias.All(x => x.CodigoCategoria > 0))
            {
                throw new ArgumentException("CategoriasNoticias del noticiaParaCrear vacio y/o invalido!.");
            }
            else if (noticiaParaCrear.Archivos != null)
            {
                throw new ArgumentException("Usa CrearArchivoStream en ArchivosService para crear el archivo o mataras la memoria del servidor!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearNoticia = await client.PostAsync<NoticiasDTO, WrapperSimpleTypesDTO>("Noticias/CrearNoticia", noticiaParaCrear);

            return wrapperCrearNoticia;
        }

        public async Task<NoticiasDTO> BuscarNoticia(NoticiasDTO noticiaParaBuscar)
        {
            if (noticiaParaBuscar == null) throw new ArgumentNullException("No puedes buscar una noticia si noticiaParaBuscar es nulo!.");
            if (noticiaParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("noticiaParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            NoticiasDTO noticiaBuscada = await client.PostAsync("Noticias/BuscarNoticia", noticiaParaBuscar);

            return noticiaBuscada;
        }

        public async Task<List<NoticiasDTO>> ListarNoticias(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar las noticias si buscador es nulo!.");
            if (buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<NoticiasDTO> listarNoticias = await client.PostAsync<BuscadorDTO, List<NoticiasDTO>>("Noticias/ListarNoticias", buscador);

            return listarNoticias;
        }

        public async Task<List<TimeLineNoticias>> ListarTimeLine(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar el timeline si buscador es nulo!.");
            if (buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<TimeLineNoticias> listaTimeLine = await client.PostAsync<BuscadorDTO, List<TimeLineNoticias>>("Noticias/ListarTimeLine", buscador);

            return listaTimeLine;
        }

        public async Task<List<TimeLineNotificaciones>> ListaTimeLineNotificaciones(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar el timeline de notificacion si buscador es nulo!.");
            if (buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma
                || buscador.ConsecutivoPersona <= 0 || buscador.TipoDePerfil == TipoPerfil.SinTipoPerfil || buscador.CodigoPlanUsuario <= 0)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<TimeLineNotificaciones> listaTimeLineNotificacion = await client.PostAsync<BuscadorDTO, List<TimeLineNotificaciones>>("Noticias/ListaTimeLineNotificaciones", buscador);

            return listaTimeLineNotificacion;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarNoticia(NoticiasDTO noticiaParaModificar)
        {
            if (noticiaParaModificar == null) throw new ArgumentNullException("No puedes modificar una noticia si noticiaParaModificar es nulo!.");
            if (noticiaParaModificar.Consecutivo <= 0)
            {
                throw new ArgumentException("noticiaParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarNoticia = await client.PostAsync<NoticiasDTO, WrapperSimpleTypesDTO>("Noticias/ModificarNoticia", noticiaParaModificar);

            return wrapperModificarNoticia;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarNoticia(NoticiasDTO noticiaParaEliminar)
        {
            if (noticiaParaEliminar == null) throw new ArgumentNullException("No puedes eliminar una noticia si noticiaParaEliminar es nulo!.");
            if (noticiaParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("noticiaParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarNoticia = await client.PostAsync<NoticiasDTO, WrapperSimpleTypesDTO>("Noticias/EliminarNoticia", noticiaParaEliminar);

            return wrapperEliminarNoticia;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarArchivoNoticia(NoticiasDTO noticiaArchivoParaEliminar)
        {
            if (noticiaArchivoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un archivoNoticia si noticiaArchivoParaEliminar es nulo!.");
            if (noticiaArchivoParaEliminar.Consecutivo <= 0 || noticiaArchivoParaEliminar.CodigoArchivo <= 0)
            {
                throw new ArgumentException("noticiaArchivoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarArchivoNoticia = await client.PostAsync<NoticiasDTO, WrapperSimpleTypesDTO>("Noticias/EliminarArchivoNoticia", noticiaArchivoParaEliminar);

            return wrapperEliminarArchivoNoticia;
        }


        #endregion


        #region Metodos NoticiasContenidos


        public async Task<WrapperSimpleTypesDTO> CrearNoticiasContenidos(List<NoticiasContenidosDTO> noticiaContenidoParaCrear)
        {
            if (noticiaContenidoParaCrear == null) throw new ArgumentNullException("No puedes crear un noticiaContenido si noticiaContenidoParaCrear es nulo!.");
            if (noticiaContenidoParaCrear.Count <= 0
                || !noticiaContenidoParaCrear.All(x => x.CodigoNoticia > 0 && !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0))
            {
                throw new ArgumentException("noticiaContenidoParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearNoticiasContenidos = await client.PostAsync<List<NoticiasContenidosDTO>, WrapperSimpleTypesDTO>("Noticias/CrearNoticiasContenidos", noticiaContenidoParaCrear);

            return wrapperCrearNoticiasContenidos;
        }

        public async Task<NoticiasContenidosDTO> BuscarNoticiaContenidoPorConsecutivo(NoticiasContenidosDTO noticiaContenidoParaBuscar)
        {
            if (noticiaContenidoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un noticiaContenido si noticiaContenidoParaBuscar es nulo!.");
            if (noticiaContenidoParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("noticiaContenidoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            NoticiasContenidosDTO noticiaContenidoBuscado = await client.PostAsync("Noticias/BuscarNoticiaContenidoPorConsecutivo", noticiaContenidoParaBuscar);

            return noticiaContenidoBuscado;
        }

        public async Task<List<NoticiasContenidosDTO>> ListarNoticiasContenidosDeUnaNoticia(NoticiasContenidosDTO noticiaContenidoParaListar)
        {
            if (noticiaContenidoParaListar == null) throw new ArgumentNullException("No puedes listar los noticiasContenido si noticiaContenidoParaListar es nulo!.");
            if (noticiaContenidoParaListar.CodigoNoticia <= 0)
            {
                throw new ArgumentException("noticiaContenidoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<NoticiasContenidosDTO> listaContenidoDeUnaNoticia = await client.PostAsync<NoticiasContenidosDTO, List<NoticiasContenidosDTO>>("Noticias/ListarNoticiasContenidosDeUnaNoticia", noticiaContenidoParaListar);

            return listaContenidoDeUnaNoticia;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarNoticiaContenido(NoticiasContenidosDTO noticiaContenidoParaModificar)
        {
            if (noticiaContenidoParaModificar == null) throw new ArgumentNullException("No puedes modificar un noticiaContenido si noticiaContenidoParaModificar es nulo!.");
            if (noticiaContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(noticiaContenidoParaModificar.Titulo))
            {
                throw new ArgumentException("noticiaContenidoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarNoticiaContenido = await client.PostAsync<NoticiasContenidosDTO, WrapperSimpleTypesDTO>("Noticias/ModificarNoticiaContenido", noticiaContenidoParaModificar);

            return wrapperModificarNoticiaContenido;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarMultiplesNoticiaContenido(List<NoticiasContenidosDTO> noticiaContenidoParaModificar)
        {
            if (noticiaContenidoParaModificar == null) throw new ArgumentNullException("No puedes modificar las noticiaContenido si noticiaContenidoParaModificar es nulo!.");
            if (noticiaContenidoParaModificar.Count > 0 && noticiaContenidoParaModificar.All(x => x.Consecutivo > 0 && !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0 ))
            {
                throw new ArgumentException("noticiaContenidoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarMultiplesNoticiaContenido = await client.PostAsync<List<NoticiasContenidosDTO>, WrapperSimpleTypesDTO>("Noticias/ModificarMultiplesNoticiaContenido", noticiaContenidoParaModificar);

            return wrapperModificarMultiplesNoticiaContenido;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarNoticiaContenido(NoticiasContenidosDTO noticiaContenidoParaEliminar)
        {
            if (noticiaContenidoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un noticiaContenido si noticiaContenidoParaEliminar es nulo!.");
            if (noticiaContenidoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("noticiaContenidoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarNoticiaContenido = await client.PostAsync<NoticiasContenidosDTO, WrapperSimpleTypesDTO>("Noticias/EliminarNoticiaContenido", noticiaContenidoParaEliminar);

            return wrapperEliminarNoticiaContenido;
        }


        #endregion


        #region Metodos NoticiasPaises


        public async Task<WrapperSimpleTypesDTO> CrearNoticiasPaises(List<NoticiasPaisesDTO> noticiaPaisParaCrear)
        {
            if (noticiaPaisParaCrear == null) throw new ArgumentNullException("No puedes crear un noticiaPais si noticiaPaisParaCrear es nulo!.");
            if (noticiaPaisParaCrear.Count <= 0
                || !noticiaPaisParaCrear.All(x => x.CodigoNoticia > 0 && x.CodigoPais > 0))
            {
                throw new ArgumentException("noticiaPaisParaCrear vacio y/o invalido!.");
            }
            
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearNoticiasPaises = await client.PostAsync<List<NoticiasPaisesDTO>, WrapperSimpleTypesDTO>("Noticias/CrearNoticiasPaises", noticiaPaisParaCrear);

            return wrapperCrearNoticiasPaises;
        }

        public async Task<NoticiasPaisesDTO> BuscarNoticiaPaisPorConsecutivo(NoticiasPaisesDTO noticiaPaisParaBuscar)
        {
            if (noticiaPaisParaBuscar == null) throw new ArgumentNullException("No puedes buscar un noticiaPais si noticiaPaisParaBuscar es nulo!.");
            if (noticiaPaisParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("noticiaPaisParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            NoticiasPaisesDTO noticiaPaisBuscada = await client.PostAsync("Noticias/BuscarNoticiaPaisPorConsecutivo", noticiaPaisParaBuscar);

            return noticiaPaisBuscada;
        }

        public async Task<List<NoticiasPaisesDTO>> ListarNoticiasPaisesDeUnaNoticia(NoticiasPaisesDTO noticiaPaisesParaListar)
        {
            if (noticiaPaisesParaListar == null) throw new ArgumentNullException("No puedes listar los noticiaPais si noticiaPaisesParaListar es nulo!.");
            if (noticiaPaisesParaListar.CodigoNoticia <= 0)
            {
                throw new ArgumentException("noticiaPaisesParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<NoticiasPaisesDTO> listaPaisesDeUnaNoticia = await client.PostAsync<NoticiasPaisesDTO, List<NoticiasPaisesDTO>>("Noticias/ListarNoticiasPaisesDeUnaNoticia", noticiaPaisesParaListar);

            return listaPaisesDeUnaNoticia;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarNoticiaPais(NoticiasPaisesDTO noticiaPaisParaEliminar)
        {
            if (noticiaPaisParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un noticiaPais si noticiaPaisParaEliminar es nulo!.");
            if (noticiaPaisParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("noticiaPaisParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarNoticiaPais = await client.PostAsync<NoticiasPaisesDTO, WrapperSimpleTypesDTO>("Noticias/EliminarNoticiaPais", noticiaPaisParaEliminar);

            return wrapperEliminarNoticiaPais;
        }


        #endregion


    }
}
