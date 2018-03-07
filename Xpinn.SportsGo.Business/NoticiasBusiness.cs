using SimpleFeedReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Business
{
    public class NoticiasBusiness
    {

        #region Metodos RssFeeds


        public async Task<WrapperSimpleTypesDTO> InteractuarRssFeed(List<RssFeeds> rssFeedParaInteractuar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);

                foreach (var feed in rssFeedParaInteractuar)
                {
                    if (feed.Consecutivo <= 0)
                    {
                        noticiaRepo.CrearRssFeed(feed);
                    }
                    else
                    {
                        RssFeeds feedExistente = await noticiaRepo.ModificarRssFeed(feed);
                    }
                }

                WrapperSimpleTypesDTO wrapperInteractuarRssFeed = new WrapperSimpleTypesDTO();

                wrapperInteractuarRssFeed.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperInteractuarRssFeed.NumeroRegistrosAfectados > 0)
                {
                    wrapperInteractuarRssFeed.Exitoso = true;
                }

                return wrapperInteractuarRssFeed;
            }
        }

        public async Task<RssFeeds> BuscarRssFeed(RssFeeds rssParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);
                RssFeeds rssBuscado = await noticiaRepo.BuscarRssFeed(rssParaBuscar);

                return rssBuscado;
            }
        }

        public async Task<List<RssFeeds>> ListarRssFeed()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);

                List<RssFeeds> listaRssFeeds = await noticiaRepo.ListarRssFeed();

                return listaRssFeeds;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarRssFeed(RssFeeds rssFeedParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);

                noticiaRepo.EliminarRssFeed(rssFeedParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarRssFeed = new WrapperSimpleTypesDTO();

                wrapperEliminarRssFeed.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarRssFeed.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarRssFeed.Exitoso = true;
                }

                return wrapperEliminarRssFeed;
            }
        }

        #endregion


        #region Metodos Notificaciones


        public async Task<List<TimeLineNotificaciones>> ListaTimeLineNotificaciones(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (buscador.FechaFiltroBase != DateTime.MinValue)
                {
                    buscador.FechaFiltroBase = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaFiltroBase);
                }

                List<NoticiasDTO> listaNoticias = await noticiaRepo.ListarNoticiasNotificaciones(buscador);
                List<NotificacionesDTO> listarNotificaciones = await noticiaRepo.ListarNotificaciones(buscador);

                List<TimeLineNotificaciones> listaTimeLineNotificacion = TimeLineNotificaciones.CrearListaTimeLineNotificaciones(listaNoticias, listarNotificaciones);

                // Si no es minValue significa que estoy refrescando y no busco feed
                if (buscador.FechaFiltroBase == DateTime.MinValue)
                {
                    RssFeeds rssBuscado = await noticiaRepo.BuscarRssFeedPorCodigoIdioma(buscador.CodigoIdiomaUsuarioBase);

                    if (rssBuscado != null && !string.IsNullOrWhiteSpace(rssBuscado.UrlFeed))
                    {
                        FeedReader reader = new FeedReader();
                        IEnumerable<FeedItem> items = reader.RetrieveFeed(rssBuscado.UrlFeed);

                        if (items != null && items.Count() > 0)
                        {
                            IEnumerable<FeedItem> itemsFiltrados = items.Skip(buscador.SkipIndexBase).Take(buscador.TakeIndexBase);

                            if (itemsFiltrados != null && itemsFiltrados.Count() > 0)
                            {
                                foreach (FeedItem item in itemsFiltrados)
                                {
                                    string url = item.Uri != null ? item.Uri.AbsoluteUri : string.Empty;

                                    TimeLineNotificaciones timeLineRss = new TimeLineNotificaciones(item.Title, item.Summary, url, string.Empty, TipoNotificacionEnum.RssFeed);

                                    listaTimeLineNotificacion.Insert(0, timeLineRss);
                                }
                            }
                        }
                    }
                }
                // Cuando Skip este en 0 significa que es la primera consulta, asi no repito su cosa de validar el plan
                // si "FechaFiltroBase" tiene un valor significa que es pull to refresh y no debo agregar las validaciones del plan
                if (buscador.SkipIndexBase == 0 && buscador.FechaFiltroBase == DateTime.MinValue)
                {
                    PlanesRepository planRepo = new PlanesRepository(context);
                    AdministracionRepository adminRepo = new AdministracionRepository(context);

                    PlanesUsuarios planUsuario = new PlanesUsuarios
                    {
                        Consecutivo = buscador.CodigoPlanUsuario,
                        IdiomaBase = buscador.IdiomaBase
                    };
                    PlanesUsuariosDTO planDelUsuarioValidado = await planRepo.BuscarPlanUsuario(planUsuario);
                    ImagenesPerfilAdministradores imagenperfil = await adminRepo.BuscarPrimeraImagenPerfilAdministrador();

                    long mesesFaltantesParaQueSeVenza = DateTimeHelper.DiferenciaEntreDosFechas(planDelUsuarioValidado.Vencimiento, DateTime.Now);

                    if (mesesFaltantesParaQueSeVenza == 1 && planDelUsuarioValidado.Planes.PlanDefault != 1)
                    {
                        TimeLineNotificaciones timeLineCasiVence = new TimeLineNotificaciones
                        {
                            CodigoArchivo = imagenperfil.CodigoArchivo,
                            UrlArchivo = imagenperfil.UrlImagenPerfil,
                            DescripcionPlan = planDelUsuarioValidado.Planes.DescripcionIdiomaBuscado,
                            FechaVencimientoPlan = planDelUsuarioValidado.Vencimiento,
                            TipoDeLaNotificacion = TipoNotificacionEnum.EstaPorVencersePlan,
                            CreacionNotificacion = DateTime.Now
                        };

                        listaTimeLineNotificacion.Insert(0, timeLineCasiVence);
                    }
                    else if (mesesFaltantesParaQueSeVenza < 0 && planDelUsuarioValidado.Planes.PlanDefault != 1)
                    {
                        TimeLineNotificaciones timeLineVencio = new TimeLineNotificaciones
                        {
                            CodigoArchivo = imagenperfil.CodigoArchivo,
                            UrlArchivo = imagenperfil.UrlImagenPerfil,
                            DescripcionPlan = planDelUsuarioValidado.Planes.DescripcionIdiomaBuscado,
                            FechaVencimientoPlan = planDelUsuarioValidado.Vencimiento,
                            TipoDeLaNotificacion = TipoNotificacionEnum.SeVencioPlan,
                            CreacionNotificacion = DateTime.Now
                        };

                        listaTimeLineNotificacion.Insert(0, timeLineVencio);
                    }
                }

                if (listaTimeLineNotificacion != null && listaTimeLineNotificacion.Count > 0)
                {
                    foreach (var notificacion in listaTimeLineNotificacion)
                    {
                        notificacion.CreacionNotificacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, notificacion.CreacionNotificacion);
                        notificacion.FechaInicioEvento = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, notificacion.FechaInicioEvento);
                        notificacion.FechaTerminacionEvento = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, notificacion.FechaTerminacionEvento);
                        notificacion.FechaVencimientoPlan = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, notificacion.FechaVencimientoPlan);
                    }
                }

                return listaTimeLineNotificacion;
            }
        }


        #endregion


        #region Metodos Noticias


        public async Task<WrapperSimpleTypesDTO> CrearNoticia(Noticias crearNoticia)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);

                crearNoticia.Creacion = DateTime.Now;
                noticiaRepo.CrearNoticia(crearNoticia);

                WrapperSimpleTypesDTO wrapperCrearNoticia = new WrapperSimpleTypesDTO();

                wrapperCrearNoticia.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearNoticia.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearNoticia.Exitoso = true;
                    wrapperCrearNoticia.ConsecutivoCreado = crearNoticia.Consecutivo;
                }

                return wrapperCrearNoticia;
            }
        }

        public async Task<Noticias> BuscarNoticia(Noticias noticiaParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);
                Noticias noticiaBuscada = await noticiaRepo.BuscarNoticia(noticiaParaBuscar);

                if (noticiaBuscada != null)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    noticiaBuscada.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(noticiaParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, noticiaBuscada.Creacion);
                }

                return noticiaBuscada;
            }
        }

        public async Task<List<TimeLineNoticias>> ListarTimeLine(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);
                GruposRepository grupoRepo = new GruposRepository(context);
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (buscador.FechaInicio != DateTime.MinValue)
                {
                    buscador.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaInicio);
                }

                List<GruposEventosDTO> listaEventos = await grupoRepo.ListarEventos(buscador, true);
                List<AnunciosDTO> listaAnuncios = await anuncianteRepo.ListarAnuncios(buscador, true);
                List<NoticiasDTO> listaNoticias = await noticiaRepo.ListarNoticiasNoNotificaciones(buscador, true);

                List<TimeLineNoticias> listaTimeLine = TimeLineNoticias.CrearListaTimeLine(listaEventos, listaAnuncios, listaNoticias);

                List<int> listaCodigoAnunciosConsultados = listaAnuncios.Select(x => x.Consecutivo).ToList();

                if (!buscador.EsConsultaEnLaApp && buscador.SkipIndexBase == 0)
                {
                    List<AnunciosDTO> listaAnunciosLaterales = await anuncianteRepo.ListarAnunciosLaterales(buscador);
                    List<TimeLineNoticias> listaAnunciosLateralesParaAdicionar = TimeLineNoticias.CrearListaTimeLine(null, listaAnunciosLaterales, null);

                    listaCodigoAnunciosConsultados.AddRange(listaAnunciosLaterales.Select(x => x.Consecutivo));

                    if (listaAnunciosLateralesParaAdicionar != null && listaAnunciosLateralesParaAdicionar.Count > 0)
                    {
                        listaTimeLine.AddRange(listaAnunciosLateralesParaAdicionar);
                    }
                }

                if (listaCodigoAnunciosConsultados.Count > 0)
                {
                    await anuncianteRepo.ModificarControlAnuncio(listaCodigoAnunciosConsultados);
                }

                if (listaTimeLine != null && listaTimeLine.Count > 0)
                {
                    foreach (var timeLine in listaTimeLine)
                    {
                        timeLine.FechaPublicacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, timeLine.FechaPublicacion);
                    }
                }

                return listaTimeLine;
            }
        }

        public async Task<List<NoticiasDTO>> ListarNoticias(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (buscador.FechaInicio != DateTime.MinValue)
                {
                    buscador.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaInicio);
                }

                List<NoticiasDTO> listaNoticias = await noticiaRepo.ListarNoticiasNoNotificaciones(buscador);

                if (listaNoticias != null && listaNoticias.Count > 0)
                {
                    foreach (var noticia in listaNoticias)
                    {
                        noticia.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, noticia.Creacion);
                    }
                }

                return listaNoticias;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarNoticia(Noticias noticiaParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                Noticias noticiaExistente = await noticiasRepo.ModificarNoticia(noticiaParaModificar);

                if (noticiaParaModificar.NoticiasContenidos != null && noticiaParaModificar.NoticiasContenidos.Count > 0)
                {
                    foreach (NoticiasContenidos noticiaContenido in noticiaParaModificar.NoticiasContenidos)
                    {
                        NoticiasContenidos noticiaContenidoExistente = await noticiasRepo.ModificarNoticiaContenido(noticiaContenido);
                    }
                }

                if (noticiaParaModificar.NoticiasPaises != null && noticiaParaModificar.NoticiasPaises.Count > 0)
                {
                    NoticiasPaises noticiaPaisParaBorrar = new NoticiasPaises
                    {
                        CodigoNoticia = noticiaParaModificar.Consecutivo
                    };

                    noticiasRepo.EliminarMultiplesNoticiasPaises(noticiaPaisParaBorrar);

                    foreach (var noticiaPais in noticiaParaModificar.NoticiasPaises)
                    {
                        noticiaPais.CodigoNoticia = noticiaParaModificar.Consecutivo;
                    }

                    noticiasRepo.CrearNoticiasPaises(noticiaParaModificar.NoticiasPaises);
                }

                if (noticiaParaModificar.CategoriasNoticias != null && noticiaParaModificar.CategoriasNoticias.Count > 0)
                {
                    CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                    CategoriasNoticias categoriaNoticiaParaBorrar = new CategoriasNoticias
                    {
                        CodigoNoticia = noticiaParaModificar.Consecutivo
                    };

                    categoriasRepo.EliminarMultiplesCategoriasNoticias(categoriaNoticiaParaBorrar);

                    foreach (CategoriasNoticias noticiaCategoria in noticiaParaModificar.CategoriasNoticias)
                    {
                        noticiaCategoria.CodigoNoticia = noticiaParaModificar.Consecutivo;
                    }

                    categoriasRepo.CrearListaCategoriaNoticias(noticiaParaModificar.CategoriasNoticias);
                }

                WrapperSimpleTypesDTO wrapperModificarNoticia = new WrapperSimpleTypesDTO();

                wrapperModificarNoticia.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarNoticia.NumeroRegistrosAfectados > 0) wrapperModificarNoticia.Exitoso = true;

                return wrapperModificarNoticia;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarNoticia(Noticias noticiaParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                NoticiasContenidos noticiaContenidoParaBorrar = new NoticiasContenidos
                {
                    CodigoNoticia = noticiaParaEliminar.Consecutivo
                };
                noticiasRepo.EliminarMultiplesNoticiasContenidos(noticiaContenidoParaBorrar);

                NoticiasPaises noticiaPaisesParaBorrar = new NoticiasPaises
                {
                    CodigoNoticia = noticiaParaEliminar.Consecutivo
                };
                noticiasRepo.EliminarMultiplesNoticiasPaises(noticiaPaisesParaBorrar);

                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasNoticias categoriasNoticiaParaBorrar = new CategoriasNoticias
                {
                    CodigoNoticia = noticiaParaEliminar.Consecutivo
                };
                categoriasRepo.EliminarMultiplesCategoriasNoticias(categoriasNoticiaParaBorrar);

                int? codigoArchivoDeAnuncio = await noticiasRepo.BuscarArchivoDeUnaNoticia(noticiaParaEliminar);
                noticiasRepo.EliminarNoticia(noticiaParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarNoticia = new WrapperSimpleTypesDTO();

                wrapperEliminarNoticia.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (codigoArchivoDeAnuncio.HasValue)
                {
                    ArchivosRepository archivoRepo = new ArchivosRepository(context);
                    Archivos archivoParaEliminar = new Archivos
                    {
                        Consecutivo = codigoArchivoDeAnuncio.Value,
                    };
                    archivoRepo.EliminarArchivo(archivoParaEliminar);
                }

                wrapperEliminarNoticia.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                if (wrapperEliminarNoticia.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarNoticia.Exitoso = true;
                }

                return wrapperEliminarNoticia;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarArchivoNoticia(Noticias noticiaArchivoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                Archivos archivoParaEliminar = new Archivos
                {
                    Consecutivo = noticiaArchivoParaEliminar.CodigoArchivo.Value,
                };

                NoticiasRepository noticiaRepo = new NoticiasRepository(context);
                Noticias noticiaExistente = await noticiaRepo.DesasignarArchivoNoticia(noticiaArchivoParaEliminar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                archivoRepo.EliminarArchivo(archivoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarArchivoNoticia = new WrapperSimpleTypesDTO();

                wrapperEliminarArchivoNoticia.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarArchivoNoticia.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarArchivoNoticia.Exitoso = true;
                }

                return wrapperEliminarArchivoNoticia;
            }
        }


        #endregion


        #region Metodos NoticiasContenidos


        public async Task<WrapperSimpleTypesDTO> CrearNoticiasContenidos(List<NoticiasContenidos> noticiaContenidoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiaRepo = new NoticiasRepository(context);
                noticiaRepo.CrearNoticiasContenidos(noticiaContenidoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearNoticiasContenidos = new WrapperSimpleTypesDTO();

                wrapperCrearNoticiasContenidos.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearNoticiasContenidos.NumeroRegistrosAfectados > 0) wrapperCrearNoticiasContenidos.Exitoso = true;

                return wrapperCrearNoticiasContenidos;
            }
        }

        public async Task<NoticiasContenidos> BuscarNoticiaContenidoPorConsecutivo(NoticiasContenidos noticiaContenidoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                NoticiasContenidos noticiaContenidoBuscado = await noticiasRepo.BuscarNoticiaContenidoPorConsecutivo(noticiaContenidoParaBuscar);

                return noticiaContenidoBuscado;
            }
        }

        public async Task<List<NoticiasContenidos>> ListarNoticiasContenidosDeUnaNoticia(NoticiasContenidos noticiaContenidoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                List<NoticiasContenidos> listaContenidoDeUnaNoticia = await noticiasRepo.ListarNoticiasContenidosDeUnaNoticia(noticiaContenidoParaListar);

                return listaContenidoDeUnaNoticia;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarNoticiaContenido(NoticiasContenidos noticiaContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                NoticiasContenidos noticiaContenidoExistente = await noticiasRepo.ModificarNoticiaContenido(noticiaContenidoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarNoticiaContenido = new WrapperSimpleTypesDTO();

                wrapperModificarNoticiaContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarNoticiaContenido.NumeroRegistrosAfectados > 0) wrapperModificarNoticiaContenido.Exitoso = true;

                return wrapperModificarNoticiaContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarMultiplesNoticiaContenido(List<NoticiasContenidos> noticiaContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);

                foreach (var noticiaContenido in noticiaContenidoParaModificar)
                {
                    NoticiasContenidos noticiaContenidoExistente = await noticiasRepo.ModificarNoticiaContenido(noticiaContenido);
                }

                WrapperSimpleTypesDTO wrapperModificarNoticiaContenido = new WrapperSimpleTypesDTO();

                wrapperModificarNoticiaContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarNoticiaContenido.NumeroRegistrosAfectados > 0) wrapperModificarNoticiaContenido.Exitoso = true;

                return wrapperModificarNoticiaContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarNoticiaContenido(NoticiasContenidos noticiaContenidoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                noticiasRepo.EliminarNoticiaContenido(noticiaContenidoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarNoticiaContenido = new WrapperSimpleTypesDTO();

                wrapperEliminarNoticiaContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarNoticiaContenido.NumeroRegistrosAfectados > 0) wrapperEliminarNoticiaContenido.Exitoso = true;

                return wrapperEliminarNoticiaContenido;
            }
        }


        #endregion


        #region Metodos NoticiasPaises


        public async Task<WrapperSimpleTypesDTO> CrearNoticiasPaises(List<NoticiasPaises> noticiaPaisParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                noticiasRepo.CrearNoticiasPaises(noticiaPaisParaCrear);

                WrapperSimpleTypesDTO wrapperCrearNoticiasPaises = new WrapperSimpleTypesDTO();

                wrapperCrearNoticiasPaises.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearNoticiasPaises.NumeroRegistrosAfectados > 0) wrapperCrearNoticiasPaises.Exitoso = true;

                return wrapperCrearNoticiasPaises;
            }
        }

        public async Task<NoticiasPaises> BuscarNoticiaPaisPorConsecutivo(NoticiasPaises noticiaPaisParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                NoticiasPaises noticiaPaisBuscado = await noticiasRepo.BuscarNoticiaPaisPorConsecutivo(noticiaPaisParaBuscar);

                return noticiaPaisBuscado;
            }
        }

        public async Task<List<NoticiasPaises>> ListarNoticiasPaisesDeUnaNoticia(NoticiasPaises noticiaPaisesParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                List<NoticiasPaises> listaPaisesDeUnaNoticia = await noticiasRepo.ListarNoticiasPaisesDeUnaNoticia(noticiaPaisesParaListar);

                return listaPaisesDeUnaNoticia;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarNoticiaPais(NoticiasPaises noticiaPaisParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                noticiasRepo.EliminarNoticiaPais(noticiaPaisParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarNoticiaPais = new WrapperSimpleTypesDTO();

                wrapperEliminarNoticiaPais.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarNoticiaPais.NumeroRegistrosAfectados > 0) wrapperEliminarNoticiaPais.Exitoso = true;

                return wrapperEliminarNoticiaPais;
            }
        }


        #endregion


    }
}
