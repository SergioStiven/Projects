using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class NoticiasRepository
    {

        SportsGoEntities _context;

        public NoticiasRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos RssFeeds


        public void CrearRssFeed(RssFeeds feedParaCrear)
        {
            _context.RssFeeds.Add(feedParaCrear);
        }

        public async Task<RssFeeds> BuscarRssFeed(RssFeeds feedParaBuscar)
        {
            RssFeeds rssFeedBuscado = await _context.RssFeeds.Where(x => x.Consecutivo == feedParaBuscar.Consecutivo).FirstOrDefaultAsync();

            return rssFeedBuscado;
        }

        public async Task<RssFeeds> BuscarRssFeedPorCodigoIdioma(int codigoIdiomaParaBuscar)
        {
            RssFeeds rssFeedBuscado = await _context.RssFeeds.Where(x => x.CodigoIdioma == codigoIdiomaParaBuscar).FirstOrDefaultAsync();

            return rssFeedBuscado;
        }

        public async Task<List<RssFeeds>> ListarRssFeed()
        {
            List<RssFeeds> listaRssFeedBuscados = await _context.RssFeeds.ToListAsync();

            return listaRssFeedBuscados;
        }

        public async Task<RssFeeds> ModificarRssFeed(RssFeeds rssFeedParaModificar)
        {
            RssFeeds rssFeedExistente = await _context.RssFeeds.Where(x => x.Consecutivo == rssFeedParaModificar.Consecutivo).FirstOrDefaultAsync();

            rssFeedExistente.UrlFeed = rssFeedParaModificar.UrlFeed;

            return rssFeedExistente;
        }

        public void EliminarRssFeed(RssFeeds rssFeedParaEliminar)
        {
            _context.RssFeeds.Attach(rssFeedParaEliminar);
            _context.RssFeeds.Remove(rssFeedParaEliminar);
        }


        #endregion


        #region Metodos Notificaciones


        public void CrearNotificacion(Notificaciones notificacionParaCrear)
        {
            _context.Notificaciones.Add(notificacionParaCrear);
        }

        public async Task<List<NotificacionesDTO>> ListarNotificaciones(BuscadorDTO buscador)
        {
            IQueryable<Notificaciones> queryNotificaciones = _context.Notificaciones.AsQueryable();

            if (buscador.FechaFiltroBase != DateTime.MinValue)
            {
                queryNotificaciones = queryNotificaciones.Where(x => x.Creacion > buscador.FechaFiltroBase);
            }

            List<NotificacionesDTO> listaNotificaciones = await queryNotificaciones.Where(x => x.CodigoPersonaDestinoAccion == buscador.ConsecutivoPersona || x.Planes.CodigoTipoPerfil == buscador.ConsecutivoTipoPerfil)
                .Select(x => new NotificacionesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoPlanNuevo = x.CodigoPlanNuevo,
                    CodigoPersonaDestinoAccion = x.CodigoPersonaDestinoAccion,
                    CodigoPersonaOrigenAccion = x.CodigoPersonaOrigenAccion,
                    CodigoTipoNotificacion = x.CodigoTipoNotificacion,
                    Creacion = x.Creacion,
                    TipoNotificacion = new TipoNotificacionDTO
                    {
                        Consecutivo = x.TipoNotificacion.Consecutivo,
                        Descripcion = x.TipoNotificacion.Descripcion
                    },
                    Planes = _context.Planes.Where(y => y.Consecutivo == x.Planes.Consecutivo)
                    .Select(z => new PlanesDTO
                    {
                        CodigoArchivo = z.CodigoArchivo,
                        DescripcionIdiomaBuscado = z.PlanesContenidos.Where(t => t.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(h => h.Descripcion).FirstOrDefault()
                    }).FirstOrDefault(),
                    PersonasOrigenAccion = _context.Personas.Where(y => y.Consecutivo == x.PersonasOrigenAccion.Consecutivo)
                    .Select(z => new PersonasDTO
                    {
                        Consecutivo = z.Consecutivo,
                        CodigoArchivoImagenPerfil = z.CodigoArchivoImagenPerfil,
                        Nombres = z.Nombres,
                        Apellidos = z.Apellidos
                    }).FirstOrDefault(),
                    GruposEventos = _context.GruposEventos.Where(y => y.Consecutivo == x.CodigoEvento)
                    .Select(z => new GruposEventosDTO
                    {
                        Consecutivo = z.Consecutivo,
                        FechaInicio = z.FechaInicio,
                        FechaTerminacion = z.FechaTerminacion,
                        Titulo = z.Titulo
                    }).FirstOrDefault(),
                })
                .OrderByDescending(x => x.Creacion)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaNotificaciones;
        }

        public async Task<NotificacionesDTO> BuscarNotificacion(Notificaciones notificacionParaBuscar)
        {
            IQueryable<Notificaciones> queryNotificaciones = _context.Notificaciones.Where(x => x.Consecutivo == notificacionParaBuscar.Consecutivo).AsQueryable();

            NotificacionesDTO notificacion = await queryNotificaciones
                .Select(x => new NotificacionesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoPlanNuevo = x.CodigoPlanNuevo,
                    CodigoPersonaDestinoAccion = x.CodigoPersonaDestinoAccion,
                    CodigoPersonaOrigenAccion = x.CodigoPersonaOrigenAccion,
                    CodigoTipoNotificacion = x.CodigoTipoNotificacion,
                    Creacion = x.Creacion,
                    TipoNotificacion = new TipoNotificacionDTO
                    {
                        Consecutivo = x.TipoNotificacion.Consecutivo,
                        Descripcion = x.TipoNotificacion.Descripcion
                    },
                    Planes = _context.Planes.Where(y => y.Consecutivo == x.Planes.Consecutivo)
                    .Select(z => new PlanesDTO
                    {
                        CodigoArchivo = z.CodigoArchivo,
                        DescripcionIdiomaBuscado = z.PlanesContenidos.Where(t => t.CodigoIdioma == notificacionParaBuscar.CodigoIdiomaUsuarioBase).Select(h => h.Descripcion).FirstOrDefault()
                    }).FirstOrDefault(),
                    PersonasOrigenAccion = _context.Personas.Where(y => y.Consecutivo == x.PersonasOrigenAccion.Consecutivo)
                    .Select(z => new PersonasDTO
                    {
                        Consecutivo = z.Consecutivo,
                        CodigoArchivoImagenPerfil = z.CodigoArchivoImagenPerfil,
                        Nombres = z.Nombres,
                        Apellidos = z.Apellidos
                    }).FirstOrDefault(),
                    GruposEventos = _context.GruposEventos.Where(y => y.Consecutivo == x.GruposEventos.Consecutivo)
                    .Select(z => new GruposEventosDTO
                    {
                        Consecutivo = z.Consecutivo,
                        CodigoGrupo = z.CodigoGrupo,
                        Titulo = z.Titulo,
                        Descripcion = z.Descripcion,
                        FechaInicio = z.FechaInicio,
                        FechaTerminacion = z.FechaTerminacion,
                        Grupos = new GruposDTO
                        {
                            Consecutivo = z.Grupos.Consecutivo,
                            CodigoPersona = z.Grupos.CodigoPersona,
                            Personas = new PersonasDTO
                            {
                                Consecutivo = z.Grupos.Personas.Consecutivo,
                                CodigoArchivoImagenPerfil = z.Grupos.Personas.CodigoArchivoImagenPerfil,
                                Nombres = z.Grupos.Personas.Nombres,
                                Apellidos = z.Grupos.Personas.Apellidos
                            }
                        }
                    }).FirstOrDefault()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return notificacion;
        }

        public void EliminarNotificacionesDeUnPlan(Notificaciones notificacionParaEliminar)
        {
            _context.Notificaciones.RemoveRange(_context.Notificaciones.Where(x => x.CodigoPlanNuevo == notificacionParaEliminar.CodigoPlanNuevo));
        }

        public void EliminarNotificacionesDeUnEvento(Notificaciones notificacionParaEliminar)
        {
            _context.Notificaciones.RemoveRange(_context.Notificaciones.Where(x => x.CodigoEvento == notificacionParaEliminar.CodigoEvento));
        }


        #endregion


        #region Metodos Noticias


        public void CrearNoticia(Noticias noticiaParaCrear)
        {
            _context.Noticias.Add(noticiaParaCrear);
        }

        public async Task<Noticias> ModificarCodigoArchivoNoticia(Noticias noticiaParaModificar)
        {
            Noticias noticiaExistente = await _context.Noticias.Where(x => x.Consecutivo == noticiaParaModificar.Consecutivo).FirstOrDefaultAsync();

            noticiaExistente.CodigoArchivo = noticiaParaModificar.CodigoArchivo;

            return noticiaExistente;
        }

        public async Task<Noticias> BuscarNoticia(Noticias noticiaParaBuscar)
        {
            Noticias noticiaBuscada = await (from noticia in _context.Noticias
                                             where noticia.Consecutivo == noticiaParaBuscar.Consecutivo
                                             select noticia).Include(x => x.CategoriasNoticias)
                                                           .Include(x => x.NoticiasContenidos)
                                                           .Include(x => x.NoticiasPaises)
                                                           .Include(x => x.TipoPublicacion)
                                                           .AsNoTracking()
                                                           .FirstOrDefaultAsync();

            if (noticiaBuscada.CodigoArchivo.HasValue)
            {
                noticiaBuscada.CodigoTipoArchivo = await _context.Archivos
                                                                 .Where(x => x.Consecutivo == noticiaBuscada.CodigoArchivo)
                                                                 .Select(x => x.CodigoTipoArchivo)
                                                                 .FirstOrDefaultAsync();
            }

            return noticiaBuscada;
        }

        public async Task<int?> BuscarArchivoDeUnaNoticia(Noticias noticiaParaBuscar)
        {
            int? codigoArchivoExistente = await _context.Noticias.Where(x => x.Consecutivo == noticiaParaBuscar.Consecutivo).Select(x => x.CodigoArchivo).FirstOrDefaultAsync();

            return codigoArchivoExistente;
        }

        public async Task<List<NoticiasDTO>> ListarNoticiasNoNotificaciones(BuscadorDTO buscador, bool esTimeLine = false)
        {
            IQueryable<Noticias> queryNoticias = _context.Noticias.AsQueryable();

            if (buscador.FechaInicio != DateTime.MinValue)
            {
                queryNoticias = queryNoticias.Where(x => x.Creacion >= buscador.FechaInicio);
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                queryNoticias = queryNoticias.Where(x => x.CategoriasNoticias.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryNoticias = queryNoticias.Where(x => x.NoticiasPaises.Any(y => buscador.PaisesParaBuscar.Contains(y.Consecutivo)));
            }

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar))
            {
                queryNoticias = queryNoticias.Where(x => x.NoticiasContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(z => z.Titulo).FirstOrDefault().Trim().ToUpper().Contains(buscador.IdentificadorParaBuscar.Trim().ToUpper()));
            }

            if (esTimeLine)
            {
                int codigoTipoNoticia = (int)TipoPublicacionNoticiasAnuncios.TimeLine;

                List<NoticiasDTO> listaNoticiasTimeLine = await queryNoticias.Where(x => x.CodigoTipoNoticia == codigoTipoNoticia)
                    .Select(x => new NoticiasDTO
                    {
                        Consecutivo = x.Consecutivo,
                        CodigoArchivo = x.CodigoArchivo,
                        UrlPublicidad = x.UrlPublicidad,
                        Creacion = x.Creacion,
                        CodigoTipoArchivo = _context.Archivos.Where(y => y.Consecutivo == x.CodigoArchivo).Select(y => y.CodigoTipoArchivo).FirstOrDefault(),
                        CodigoTipoNoticia = x.CodigoTipoNoticia,
                        CodigoUsuario = x.CodigoUsuario,
                        CodigoArchivoImagenPerfilAdministrador = _context.ImagenesPerfilAdministradores.Where(y => y.CodigoUsuario == x.CodigoUsuario).Select(y => y.CodigoArchivo).FirstOrDefault(),
                        TituloIdiomaBuscado = x.NoticiasContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault(),
                        DescripcionIdiomaBuscado = x.NoticiasContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    })
                    .OrderByDescending(x => x.Creacion)
                    .Skip(() => buscador.SkipIndexBase)
                    .Take(() => buscador.TakeIndexBase)
                    .AsNoTracking()
                    .ToListAsync();

                return listaNoticiasTimeLine;
            }
            else
            {
                int numeroRegistros = await queryNoticias.CountAsync();

                List<NoticiasDTO> listaNoticias = await queryNoticias
                    .Select(x => new NoticiasDTO
                    {
                        Consecutivo = x.Consecutivo,
                        CodigoArchivo = x.CodigoArchivo,
                        UrlPublicidad = x.UrlPublicidad,
                        Creacion = x.Creacion,
                        NumeroRegistrosExistentes = numeroRegistros,
                        CodigoTipoArchivo = _context.Archivos.Where(y => y.Consecutivo == x.CodigoArchivo).Select(y => y.CodigoTipoArchivo).FirstOrDefault(),
                        CodigoTipoNoticia = x.CodigoTipoNoticia,
                        TituloIdiomaBuscado = x.NoticiasContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault(),
                        DescripcionIdiomaBuscado = x.NoticiasContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        NoticiasContenidos = x.NoticiasContenidos
                                              .Select(z => new NoticiasContenidosDTO
                                              {
                                                  Consecutivo = z.Consecutivo,
                                                  CodigoIdioma = z.CodigoIdioma,
                                                  CodigoNoticia = z.CodigoNoticia,
                                                  Titulo = z.Titulo,
                                                  Descripcion = z.Descripcion
                                              })
                                              .ToList(),
                        NoticiasPaises = x.NoticiasPaises.Select(y =>
                                          new NoticiasPaisesDTO
                                          {
                                              Consecutivo = y.Consecutivo,
                                              CodigoPais = y.CodigoPais,
                                              CodigoNoticia = y.CodigoNoticia,
                                              DescripcionIdiomaBuscado = y.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(z => z.Descripcion).FirstOrDefault()
                                          })
                                          .ToList(),
                        CategoriasNoticias = x.CategoriasNoticias.Select(y =>
                                              new CategoriasNoticiasDTO
                                              {
                                                  Consecutivo = y.Consecutivo,
                                                  CodigoNoticia = y.CodigoNoticia,
                                                  CodigoCategoria = y.CodigoCategoria,
                                                  DescripcionIdiomaBuscado = y.Categorias.CategoriasContenidos.Where(z => z.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(z => z.Descripcion).FirstOrDefault()
                                              })
                                              .ToList()
                    })
                    .OrderByDescending(x => x.Creacion)
                    .Skip(() => buscador.SkipIndexBase)
                    .Take(() => buscador.TakeIndexBase)
                    .AsNoTracking()
                    .ToListAsync();

                return listaNoticias;
            }
        }

        public async Task<List<NoticiasDTO>> ListarNoticiasNotificaciones(BuscadorDTO buscador)
        {
            IQueryable<Noticias> queryNoticias = _context.Noticias.AsQueryable();

            if (buscador.FechaFiltroBase != DateTime.MinValue)
            {
                queryNoticias = queryNoticias.Where(x => x.Creacion > buscador.FechaFiltroBase);
            }

            int codigoTipoNoticia = (int)TipoPublicacionNoticiasAnuncios.NotificacionOAnuncioLateral;

            List<NoticiasDTO> listaNoticiasTimeLine = await queryNoticias.Where(x => x.CodigoTipoNoticia == codigoTipoNoticia)
                .Select(x => new NoticiasDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoArchivo = x.CodigoArchivo,
                    UrlPublicidad = x.UrlPublicidad,
                    Creacion = x.Creacion,
                    CodigoTipoArchivo = _context.Archivos.Where(y => y.Consecutivo == x.CodigoArchivo).Select(y => y.CodigoTipoArchivo).FirstOrDefault(),
                    CodigoTipoNoticia = x.CodigoTipoNoticia,
                    CodigoUsuario = x.CodigoUsuario,
                    CodigoArchivoImagenPerfilAdministrador = _context.ImagenesPerfilAdministradores.Where(y => y.CodigoUsuario == x.CodigoUsuario).Select(y => y.CodigoArchivo).FirstOrDefault(),
                    TituloIdiomaBuscado = x.NoticiasContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault(),
                    DescripcionIdiomaBuscado = x.NoticiasContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                })
                .OrderByDescending(x => x.Creacion)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaNoticiasTimeLine;
        }

        public async Task<Noticias> ModificarNoticia(Noticias noticiaParaModificar)
        {
            Noticias noticiaBuscada = await _context.Noticias.Where(x => x.Consecutivo == noticiaParaModificar.Consecutivo).FirstOrDefaultAsync();

            noticiaBuscada.UrlPublicidad = !string.IsNullOrWhiteSpace(noticiaParaModificar.UrlPublicidad) ? noticiaParaModificar.UrlPublicidad.Trim() : string.Empty;
            noticiaBuscada.CodigoTipoNoticia = noticiaParaModificar.CodigoTipoNoticia;

            return noticiaBuscada;
        }

        public void EliminarNoticia(Noticias noticiaParaEliminar)
        {
            _context.Noticias.Attach(noticiaParaEliminar);
            _context.Noticias.Remove(noticiaParaEliminar);
        }

        public async Task<Noticias> DesasignarArchivoNoticia(Noticias noticiaArchivoParaEliminar)
        {
            Noticias noticiaExistente = await _context.Noticias.Where(x => x.Consecutivo == noticiaArchivoParaEliminar.Consecutivo).FirstOrDefaultAsync();

            noticiaExistente.CodigoArchivo = null;

            return noticiaExistente;
        }


        #endregion


        #region Metodos NoticiasContenido


        public void CrearNoticiasContenidos(ICollection<NoticiasContenidos> noticiasContenidoParaCrear)
        {
            noticiasContenidoParaCrear.ForEach(x =>
            {
                x.Titulo.Trim();
                x.Descripcion = !string.IsNullOrWhiteSpace(x.Descripcion) ? x.Descripcion.Trim() : string.Empty;
            });

            _context.NoticiasContenidos.AddRange(noticiasContenidoParaCrear);
        }

        public async Task<NoticiasContenidos> BuscarNoticiaContenidoPorConsecutivo(NoticiasContenidos noticiaContenidoParaBuscar)
        {
            NoticiasContenidos noticiaContenidoBuscado = await (from noticiaContenido in _context.NoticiasContenidos
                                                                where noticiaContenido.Consecutivo == noticiaContenidoParaBuscar.Consecutivo
                                                                select noticiaContenido).Include(x => x.Noticias)
                                                                                        .AsNoTracking()
                                                                                        .FirstOrDefaultAsync();

            return noticiaContenidoBuscado;
        }

        public async Task<List<NoticiasContenidos>> ListarNoticiasContenidosDeUnaNoticia(NoticiasContenidos anuncioContenidoParaListar)
        {
            List<NoticiasContenidos> listaContenidoDeUnaNoticia = await (from noticiaContenido in _context.NoticiasContenidos
                                                                         where noticiaContenido.CodigoNoticia == anuncioContenidoParaListar.CodigoNoticia
                                                                         select noticiaContenido).AsNoTracking()
                                                                                                 .ToListAsync();

            return listaContenidoDeUnaNoticia;
        }

        public async Task<NoticiasContenidos> ModificarNoticiaContenido(NoticiasContenidos noticiaContenidoParaModificar)
        {
            NoticiasContenidos NoticiaContenidoExistente = await _context.NoticiasContenidos.Where(x => x.Consecutivo == noticiaContenidoParaModificar.Consecutivo).FirstOrDefaultAsync();

            NoticiaContenidoExistente.Titulo = noticiaContenidoParaModificar.Titulo.Trim();
            NoticiaContenidoExistente.Descripcion = !string.IsNullOrWhiteSpace(noticiaContenidoParaModificar.Descripcion) ? noticiaContenidoParaModificar.Descripcion.Trim() : string.Empty;

            return NoticiaContenidoExistente;
        }

        public void EliminarNoticiaContenido(NoticiasContenidos noticiaContenidoParaEliminar)
        {
            _context.NoticiasContenidos.Attach(noticiaContenidoParaEliminar);
            _context.NoticiasContenidos.Remove(noticiaContenidoParaEliminar);
        }

        public void EliminarMultiplesNoticiasContenidos(NoticiasContenidos anuncioContenidoParaEliminar)
        {
            _context.NoticiasContenidos.RemoveRange(_context.NoticiasContenidos.Where(x => x.CodigoNoticia == anuncioContenidoParaEliminar.CodigoNoticia));
        }


        #endregion


        #region Metodos NoticiasPaises


        public void CrearNoticiasPaises(ICollection<NoticiasPaises> noticiasPaisParaCrear)
        {
            _context.NoticiasPaises.AddRange(noticiasPaisParaCrear);
        }

        public async Task<NoticiasPaises> BuscarNoticiaPaisPorConsecutivo(NoticiasPaises noticiaPaisParaBuscar)
        {
            NoticiasPaises noticiaPaisBuscado = await (from noticiaPais in _context.NoticiasPaises
                                                       where noticiaPais.Consecutivo == noticiaPaisParaBuscar.Consecutivo
                                                       select noticiaPais).Include(x => x.Noticias)
                                                                          .AsNoTracking()
                                                                          .FirstOrDefaultAsync();

            return noticiaPaisBuscado;
        }

        public async Task<List<NoticiasPaises>> ListarNoticiasPaisesDeUnaNoticia(NoticiasPaises noticiaPaisesParaListar)
        {
            List<NoticiasPaises> listaPaisesDeUnaNoticia = await (from noticiaPais in _context.NoticiasPaises
                                                                  where noticiaPais.CodigoNoticia == noticiaPaisesParaListar.CodigoNoticia
                                                                  select noticiaPais).AsNoTracking()
                                                                                    .ToListAsync();

            return listaPaisesDeUnaNoticia;
        }

        public void EliminarNoticiaPais(NoticiasPaises noticiaPaisParaEliminar)
        {
            _context.NoticiasPaises.Attach(noticiaPaisParaEliminar);
            _context.NoticiasPaises.Remove(noticiaPaisParaEliminar);
        }

        public void EliminarMultiplesNoticiasPaises(NoticiasPaises noticiaPaisParaEliminar)
        {
            _context.NoticiasPaises.RemoveRange(_context.NoticiasPaises.Where(x => x.CodigoNoticia == noticiaPaisParaEliminar.CodigoNoticia));
        }


        #endregion


    }
}
