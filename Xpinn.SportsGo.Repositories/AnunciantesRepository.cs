using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class AnunciantesRepository
    {
        SportsGoEntities _context;

        public AnunciantesRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Anunciantes


        public void CrearAnunciante(Anunciantes anuncianteParaCrear)
        {
            anuncianteParaCrear.Empresa = !string.IsNullOrWhiteSpace(anuncianteParaCrear.Empresa) ? anuncianteParaCrear.Empresa.Trim() : string.Empty;
            anuncianteParaCrear.NumeroIdentificacion = !string.IsNullOrWhiteSpace(anuncianteParaCrear.NumeroIdentificacion) ? anuncianteParaCrear.NumeroIdentificacion.Trim() : string.Empty;

            anuncianteParaCrear.Personas.Nombres = anuncianteParaCrear.Personas.Nombres.Trim();
            anuncianteParaCrear.Personas.CiudadResidencia = anuncianteParaCrear.Personas.CiudadResidencia.Trim();
            anuncianteParaCrear.Personas.Telefono = anuncianteParaCrear.Personas.Telefono.Trim();
            anuncianteParaCrear.Personas.Skype = !string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Skype) ? anuncianteParaCrear.Personas.Skype.Trim() : string.Empty;

            anuncianteParaCrear.Personas.Usuarios.Usuario = anuncianteParaCrear.Personas.Usuarios.Usuario.Trim();
            anuncianteParaCrear.Personas.Usuarios.Clave = anuncianteParaCrear.Personas.Usuarios.Clave.Trim();
            anuncianteParaCrear.Personas.Usuarios.Email = anuncianteParaCrear.Personas.Usuarios.Email.Trim();
            anuncianteParaCrear.Personas.Usuarios.Creacion = DateTime.Now;

            _context.Anunciantes.Add(anuncianteParaCrear);
        }

        public async Task<PlanesDTO> BuscarConfiguracionAnuncioPorPlanAnunciante(int codigoAnunciante)
        {
            PlanesDTO configuracionPlan = await _context.Anunciantes.Where(x => x.Consecutivo == codigoAnunciante)
                                                                    .Select(x => new PlanesDTO
                                                                    {
                                                                        NumeroDiasVigenciaAnuncio = x.Personas.Usuarios.PlanesUsuarios.Planes.NumeroDiasVigenciaAnuncio,
                                                                        NumeroAparicionesAnuncio = x.Personas.Usuarios.PlanesUsuarios.Planes.NumeroAparicionesAnuncio
                                                                    }).FirstOrDefaultAsync();

            return configuracionPlan;
        }

        public async Task<Anunciantes> BuscarAnunciantePorConsecutivo(Anunciantes anuncianteParaBuscar)
        {
            Anunciantes anuncianteBuscado = await (from anunciante in _context.Anunciantes
                                                   where anunciante.Consecutivo == anuncianteParaBuscar.Consecutivo
                                                   select anunciante).Include(x => x.Personas)
                                                                     .AsNoTracking()
                                                                     .FirstOrDefaultAsync();


            return anuncianteBuscado;
        }

        public async Task<Anunciantes> BuscarAnunciantePorCodigoPersona(Anunciantes anuncianteParaBuscar)
        {
            Anunciantes anuncianteBuscado = await (from anunciante in _context.Anunciantes
                                                   where anunciante.CodigoPersona == anuncianteParaBuscar.CodigoPersona
                                                   select anunciante).Include(x => x.Personas)
                                                                     .AsNoTracking()
                                                                     .FirstOrDefaultAsync();


            return anuncianteBuscado;
        }

        public async Task<Anunciantes> ModificarInformacionAnunciante(Anunciantes anuncianteParaModificar)
        {
            Anunciantes anuncianteExistente = await _context.Anunciantes.Where(x => x.Consecutivo == anuncianteParaModificar.Consecutivo).FirstOrDefaultAsync();

            anuncianteExistente.Empresa = !string.IsNullOrWhiteSpace(anuncianteParaModificar.Empresa) ? anuncianteParaModificar.Empresa.Trim() : string.Empty;
            anuncianteExistente.NumeroIdentificacion = !string.IsNullOrWhiteSpace(anuncianteParaModificar.NumeroIdentificacion) ? anuncianteParaModificar.NumeroIdentificacion.Trim() : string.Empty;

            return anuncianteExistente;
        }


        #endregion


        #region Metodos Anuncios


        public void CrearAnuncio(Anuncios anuncioParaCrear)
        {
            anuncioParaCrear.UrlPublicidad = !string.IsNullOrWhiteSpace(anuncioParaCrear.UrlPublicidad) ? anuncioParaCrear.UrlPublicidad.Trim() : string.Empty;

            anuncioParaCrear.Creacion = DateTime.Now;
            _context.Anuncios.Add(anuncioParaCrear);
        }

        public async Task<Anuncios> ModificarCodigoArchivoAnuncio(Anuncios anuncioParaModificar)
        {
            Anuncios anuncioExistente = await (from anuncio in _context.Anuncios
                                               where anuncio.Consecutivo == anuncioParaModificar.Consecutivo
                                               select anuncio).FirstOrDefaultAsync();

            anuncioExistente.CodigoArchivo = anuncioParaModificar.CodigoArchivo;

            return anuncioExistente;
        }

        public async Task<Anuncios> BuscarAnuncioPorConsecutivo(Anuncios anuncioParaBuscar)
        {
            Anuncios anuncioBuscado = await (from anuncio in _context.Anuncios
                                             where anuncio.Consecutivo == anuncioParaBuscar.Consecutivo
                                             select anuncio).Include(x => x.Anunciantes)
                                                            .Include(x => x.AnunciosContenidos)
                                                            .Include(x => x.AnunciosPaises)
                                                            .Include(x => x.CategoriasAnuncios)
                                                            .Include(x => x.TipoPublicacion)
                                                            .AsNoTracking()
                                                            .FirstOrDefaultAsync();

            if (anuncioBuscado.CodigoArchivo.HasValue)
            {
                anuncioBuscado.CodigoTipoArchivo = await _context.Archivos.Where(x => x.Consecutivo == anuncioBuscado.CodigoArchivo)
                                                                          .Select(x => x.CodigoTipoArchivo)
                                                                          .FirstOrDefaultAsync();
            }

            return anuncioBuscado;
        }

        public async Task<int?> BuscarArchivoDeUnAnuncio(Anuncios anuncioParaBuscar)
        {
            int? anuncioBuscado = await (from anuncio in _context.Anuncios
                                         where anuncio.Consecutivo == anuncioParaBuscar.Consecutivo
                                         select anuncio.CodigoArchivo).FirstOrDefaultAsync();

            return anuncioBuscado;
        }

        public async Task<List<AnunciosDTO>> ListarAnunciosDeUnAnunciante(Anuncios anuncioParaListar)
        {
            IQueryable<Anuncios> queryAnuncios = _context.Anuncios.Where(x => x.CodigoAnunciante == anuncioParaListar.CodigoAnunciante).AsQueryable();

            int queryContador = await queryAnuncios.CountAsync();

            List<AnunciosDTO> listaAnuncios = await queryAnuncios
                .Select(x => new AnunciosDTO
                {
                    Consecutivo = x.Consecutivo,
                    Creacion = x.Creacion,
                    Vencimiento = x.Vencimiento,
                    NumeroApariciones = x.NumeroApariciones,
                    NumeroVecesClickeados = x.NumeroVecesClickeados,
                    NumeroVecesVistos = x.NumeroVecesVistos,
                    UltimaVezVisto = x.UltimaVezVisto,
                    CodigoArchivo = x.CodigoArchivo,
                    UrlPublicidad = x.UrlPublicidad,
                    CodigoTipoArchivo = x.Archivos.CodigoTipoArchivo,
                    NumeroRegistrosExistentes = queryContador,
                    CodigoTipoAnuncio = x.CodigoTipoAnuncio,
                    FechaInicio = x.FechaInicio,
                    TituloIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == anuncioParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault(),
                    DescripcionIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == anuncioParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    AnunciosContenidos = x.AnunciosContenidos
                                          .Select(z => new AnunciosContenidosDTO
                                          {
                                              Consecutivo = z.Consecutivo,
                                              CodigoAnuncio = z.CodigoAnuncio,
                                              Titulo = z.Titulo,
                                              Descripcion = z.Descripcion,
                                              CodigoIdioma = z.CodigoIdioma,
                                          }).ToList(),
                    AnunciosPaises = x.AnunciosPaises.Select(y =>
                                          new AnunciosPaisesDTO
                                          {
                                              Consecutivo = y.Consecutivo,
                                              CodigoPais = y.CodigoPais,
                                              CodigoAnuncio = y.CodigoAnuncio,
                                              DescripcionIdiomaBuscado = y.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == anuncioParaListar.CodigoIdiomaUsuarioBase).Select(z => z.Descripcion).FirstOrDefault()
                                          })
                                          .ToList(),
                    CategoriasAnuncios = x.CategoriasAnuncios.Select(y =>
                                          new CategoriasAnunciosDTO
                                          {
                                              Consecutivo = y.Consecutivo,
                                              CodigoAnuncio = y.CodigoAnuncio,
                                              CodigoCategoria = y.CodigoCategoria,
                                              DescripcionIdiomaBuscado = y.Categorias.CategoriasContenidos.Where(z => z.CodigoIdioma == anuncioParaListar.CodigoIdiomaUsuarioBase).Select(z => z.Descripcion).FirstOrDefault()
                                          })
                                          .ToList()
                })
                .OrderByDescending(x => x.Creacion)
                .Skip(() => anuncioParaListar.SkipIndexBase)
                .Take(() => anuncioParaListar.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaAnuncios;
        }

        public async Task<List<AnunciosDTO>> ListarAnuncios(BuscadorDTO buscador, bool esTimeLine = false)
        {
            IQueryable<Anuncios> queryAnuncios = _context.Anuncios.AsQueryable();

            if (buscador.FechaInicio != DateTime.MinValue)
            {
                queryAnuncios = queryAnuncios.Where(x => x.Creacion >= buscador.FechaInicio);
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.CategoriasAnuncios.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosPaises.Any(y => buscador.PaisesParaBuscar.Contains(y.Consecutivo)));
            }

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar))
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault().Trim().ToUpper().Contains(buscador.IdentificadorParaBuscar.Trim().ToUpper()));
            }

            if (buscador.IdiomaBase != Idioma.SinIdioma)
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosContenidos.Any(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase));
            }

            if (esTimeLine)
            {
                int codigoTipoAnuncioLateral = (int)TipoPublicacionNoticiasAnuncios.TimeLine;
                queryAnuncios.Where(x => x.Vencimiento >= DateTime.Now && x.CodigoTipoAnuncio == codigoTipoAnuncioLateral && x.NumeroApariciones > x.NumeroVecesVistos && x.FechaInicio <= DateTime.Now);

                int numeroAnunciosPosibles = await queryAnuncios.CountAsync();

                Random random = new Random();
                int skipIndexRandom = random.Next(0, numeroAnunciosPosibles);

                int diferenciaSkipConTotal = numeroAnunciosPosibles - skipIndexRandom;
                if (diferenciaSkipConTotal < buscador.TakeIndexBase)
                {
                    skipIndexRandom -= buscador.SkipIndexBase - diferenciaSkipConTotal;

                    if (skipIndexRandom < 0)
                    {
                        skipIndexRandom = 0;
                    }
                }

                List<AnunciosDTO> listaAnunciosTimeLine = await queryAnuncios.OrderBy(x => x.UltimaVezVisto)
                    .Select(x => new AnunciosDTO
                    {
                        Consecutivo = x.Consecutivo,
                        Creacion = x.Creacion,
                        CodigoArchivo = x.CodigoArchivo,
                        UrlPublicidad = x.UrlPublicidad,
                        CodigoTipoArchivo = x.Archivos.CodigoTipoArchivo,
                        CodigoTipoAnuncio = x.CodigoTipoAnuncio,
                        TituloIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault(),
                        DescripcionIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        Anunciantes = new AnunciantesDTO
                        {
                            Consecutivo = x.Anunciantes.Consecutivo,
                            Personas = new PersonasDTO
                            {
                                Consecutivo = x.Anunciantes.Personas.Consecutivo,
                                CodigoArchivoImagenPerfil = x.Anunciantes.Personas.CodigoArchivoImagenPerfil,
                                Nombres = x.Anunciantes.Personas.Nombres,
                                Apellidos = x.Anunciantes.Personas.Apellidos
                            }
                        },
                    })
                    .OrderByDescending(x => x.Creacion)
                    .Skip(() => skipIndexRandom)
                    .Take(() => buscador.TakeIndexBase)
                    .AsNoTracking()
                    .ToListAsync();

                return listaAnunciosTimeLine;
            }
            else
            {
                int queryContador = await queryAnuncios.CountAsync();

                List<AnunciosDTO> listaAnuncios = await queryAnuncios
                    .Select(x => new AnunciosDTO
                    {
                        Consecutivo = x.Consecutivo,
                        Creacion = x.Creacion,
                        Vencimiento = x.Vencimiento,
                        NumeroApariciones = x.NumeroApariciones,
                        NumeroVecesClickeados = x.NumeroVecesClickeados,
                        NumeroVecesVistos = x.NumeroVecesVistos,
                        UltimaVezVisto = x.UltimaVezVisto,
                        CodigoArchivo = x.CodigoArchivo,
                        UrlPublicidad = x.UrlPublicidad,
                        CodigoTipoArchivo = x.Archivos.CodigoTipoArchivo,
                        NumeroRegistrosExistentes = queryContador,
                        CodigoTipoAnuncio = x.CodigoTipoAnuncio,
                        FechaInicio = x.FechaInicio,
                        TituloIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault(),
                        DescripcionIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        Anunciantes = new AnunciantesDTO
                        {
                            Consecutivo = x.Anunciantes.Consecutivo,
                            Personas = new PersonasDTO
                            {
                                Consecutivo = x.Anunciantes.Personas.Consecutivo,
                                CodigoArchivoImagenPerfil = x.Anunciantes.Personas.CodigoArchivoImagenPerfil,
                                Nombres = x.Anunciantes.Personas.Nombres,
                                Apellidos = x.Anunciantes.Personas.Apellidos
                            }
                        },
                        AnunciosContenidos = x.AnunciosContenidos
                                              .Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase)
                                              .Select(z => new AnunciosContenidosDTO
                                              {
                                                  Consecutivo = z.Consecutivo,
                                                  CodigoAnuncio = z.CodigoAnuncio,
                                                  Titulo = z.Titulo,
                                                  Descripcion = z.Descripcion,
                                                  CodigoIdioma = z.CodigoIdioma,
                                              }).ToList()
                    })
                    .OrderByDescending(x => x.Creacion)
                    .Skip(() => buscador.SkipIndexBase)
                    .Take(() => buscador.TakeIndexBase)
                    .AsNoTracking()
                    .ToListAsync();

                return listaAnuncios;
            }
        }

        public async Task<int?> ConsultarDuracionVideoParaElPlanDeEsteAnuncio(int codigoAnuncio)
        {
            int? duracionVideoPermtida = await _context.Anuncios.Where(x => x.Consecutivo == codigoAnuncio).Select(x => x.Anunciantes.Personas.Usuarios.PlanesUsuarios.Planes.TiempoPermitidoVideo).FirstOrDefaultAsync();

            return duracionVideoPermtida;
        }

        public async Task<List<AnunciosDTO>> ListarAnunciosLaterales(BuscadorDTO buscador)
        {
            IQueryable<Anuncios> queryAnuncios = _context.Anuncios.AsQueryable();

            if (buscador.FechaInicio != DateTime.MinValue)
            {
                queryAnuncios = queryAnuncios.Where(x => x.Creacion >= buscador.FechaInicio);
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.CategoriasAnuncios.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosPaises.Any(y => buscador.PaisesParaBuscar.Contains(y.Consecutivo)));
            }

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar))
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault().Trim().ToUpper().Contains(buscador.IdentificadorParaBuscar.Trim().ToUpper()));
            }

            if (buscador.IdiomaBase != Idioma.SinIdioma)
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosContenidos.Any(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase));
            }

            int codigoTipoAnuncioLateral = (int)TipoPublicacionNoticiasAnuncios.NotificacionOAnuncioLateral;

            List<AnunciosDTO> listaAnunciosTimeLine = await queryAnuncios.Where(x => x.Vencimiento >= DateTime.Now && x.CodigoTipoAnuncio == codigoTipoAnuncioLateral && x.FechaInicio <= DateTime.Now)
                .Select(x => new AnunciosDTO
                {
                    Consecutivo = x.Consecutivo,
                    Creacion = x.Creacion,
                    CodigoArchivo = x.CodigoArchivo,
                    UrlPublicidad = x.UrlPublicidad,
                    CodigoTipoArchivo = x.Archivos.CodigoTipoArchivo,
                    CodigoTipoAnuncio = x.CodigoTipoAnuncio,
                    NoticiaLateral = true,
                    TituloIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Titulo).FirstOrDefault(),
                    DescripcionIdiomaBuscado = x.AnunciosContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    Anunciantes = new AnunciantesDTO
                    {
                        Consecutivo = x.Anunciantes.Consecutivo,
                        Personas = new PersonasDTO
                        {
                            Consecutivo = x.Anunciantes.Personas.Consecutivo,
                            CodigoArchivoImagenPerfil = x.Anunciantes.Personas.CodigoArchivoImagenPerfil,
                            Nombres = x.Anunciantes.Personas.Nombres,
                            Apellidos = x.Anunciantes.Personas.Apellidos
                        }
                    },
                })
                .OrderByDescending(x => x.Creacion)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaAnunciosTimeLine;

        }

        public async Task<Anuncios> ModificarAnuncio(Anuncios anuncioParaModificar)
        {
            Anuncios anuncioExistente = await _context.Anuncios.Where(x => x.Consecutivo == anuncioParaModificar.Consecutivo).FirstOrDefaultAsync();

            anuncioExistente.FechaInicio = anuncioParaModificar.FechaInicio;
            anuncioExistente.UrlPublicidad = !string.IsNullOrWhiteSpace(anuncioParaModificar.UrlPublicidad) ? anuncioParaModificar.UrlPublicidad.Trim() : string.Empty;
            anuncioExistente.CodigoTipoAnuncio = anuncioParaModificar.CodigoTipoAnuncio;

            return anuncioExistente;
        }

        public async Task ModificarControlAnuncio(List<int> listaCodigoAnuncios)
        {
            string query = @"UPDATE a set Ultimavezvisto = getdate(), NumeroVecesVistos = (ISNULL((Select numerovecesvistos from anuncios b where b.Consecutivo = a.consecutivo), 0) + 1) from Anuncios a where Consecutivo IN (" + ExtensionMethodsHelper.ConvertToCommaSeparatedString(listaCodigoAnuncios) + ")";

            await _context.Database.ExecuteSqlCommandAsync(query);
        }

        public async Task<Anuncios> AumentarContadorClickDeUnAnuncio(Anuncios anuncioParaAumentar)
        {
            Anuncios anuncioExistente = await _context.Anuncios.Where(x => x.Consecutivo == anuncioParaAumentar.Consecutivo).FirstOrDefaultAsync();

            anuncioExistente.NumeroVecesClickeados = anuncioExistente.NumeroVecesClickeados.HasValue ? anuncioExistente.NumeroVecesClickeados + 1 : 1;

            return anuncioExistente;
        }

        public void EliminarAnuncio(Anuncios anuncioParaEliminar)
        {
            _context.Anuncios.Attach(anuncioParaEliminar);
            _context.Anuncios.Remove(anuncioParaEliminar);
        }

        public async Task<Anuncios> DesasignarArchivoAnuncio(Anuncios anuncioArchivoParaEliminar)
        {
            Anuncios anuncioExistente = await _context.Anuncios.Where(x => x.Consecutivo == anuncioArchivoParaEliminar.Consecutivo).FirstOrDefaultAsync();

            anuncioExistente.CodigoArchivo = null;

            return anuncioExistente;
        }


        #endregion


        #region Metodos AnunciosContenidos


        public void CrearAnunciosContenidos(ICollection<AnunciosContenidos> anuncioContenidoParaCrear)
        {
            anuncioContenidoParaCrear.ForEach(x =>
            {
                x.Titulo.Trim();
                x.Descripcion = !string.IsNullOrWhiteSpace(x.Descripcion) ? x.Descripcion.Trim() : string.Empty;
            });

            _context.AnunciosContenidos.AddRange(anuncioContenidoParaCrear);
        }

        public async Task<AnunciosContenidos> BuscarAnuncioContenidoPorConsecutivo(AnunciosContenidos anuncioContenidoParaBuscar)
        {
            AnunciosContenidos anuncioContenidoBuscado = await (from anuncioContenido in _context.AnunciosContenidos
                                                                where anuncioContenido.Consecutivo == anuncioContenidoParaBuscar.Consecutivo
                                                                select anuncioContenido).Include(x => x.Anuncios)
                                                                                        .AsNoTracking()
                                                                                        .FirstOrDefaultAsync();

            return anuncioContenidoBuscado;
        }

        public async Task<List<AnunciosContenidos>> ListarAnunciosContenidosDeUnAnuncio(AnunciosContenidos anuncioContenidoParaListar)
        {
            List<AnunciosContenidos> listaContenidoDeUnAnuncio = await (from anuncioContenido in _context.AnunciosContenidos
                                                                        where anuncioContenido.CodigoAnuncio == anuncioContenidoParaListar.CodigoAnuncio
                                                                        select anuncioContenido).AsNoTracking()
                                                                                                .ToListAsync();

            return listaContenidoDeUnAnuncio;
        }

        public async Task<AnunciosContenidos> ModificarAnuncioContenido(AnunciosContenidos anuncioContenidoParaModificar)
        {
            AnunciosContenidos anuncioContenidoExistente = await _context.AnunciosContenidos.Where(x => x.Consecutivo == anuncioContenidoParaModificar.Consecutivo).FirstOrDefaultAsync();

            anuncioContenidoExistente.Titulo = anuncioContenidoParaModificar.Titulo.Trim();
            anuncioContenidoExistente.Descripcion = !string.IsNullOrWhiteSpace(anuncioContenidoParaModificar.Descripcion) ? anuncioContenidoParaModificar.Descripcion.Trim() : string.Empty;

            return anuncioContenidoExistente;
        }

        public void EliminarAnuncioContenido(AnunciosContenidos anuncioContenidoParaEliminar)
        {
            _context.AnunciosContenidos.Attach(anuncioContenidoParaEliminar);
            _context.AnunciosContenidos.Remove(anuncioContenidoParaEliminar);
        }

        public void EliminarMultiplesContenidosAnuncios(AnunciosContenidos anuncioContenidoParaEliminar)
        {
            _context.AnunciosContenidos.RemoveRange(_context.AnunciosContenidos.Where(x => x.CodigoAnuncio == anuncioContenidoParaEliminar.CodigoAnuncio));
        }


        #endregion


        #region Metodos AnunciosPaises


        public void CrearAnunciosPaises(ICollection<AnunciosPaises> anuncioPaisParaCrear)
        {
            _context.AnunciosPaises.AddRange(anuncioPaisParaCrear);
        }

        public async Task<AnunciosPaises> BuscaAnuncioPaisPorConsecutivo(AnunciosPaises anuncioPaisParaBuscar)
        {
            AnunciosPaises anuncioPaisBuscado = await (from anuncioPais in _context.AnunciosPaises
                                                       where anuncioPais.Consecutivo == anuncioPaisParaBuscar.Consecutivo
                                                       select anuncioPais).Include(x => x.Anuncios)
                                                                          .AsNoTracking()
                                                                          .FirstOrDefaultAsync();

            return anuncioPaisBuscado;
        }

        public async Task<List<AnunciosPaises>> ListarAnunciosPaisesDeUnAnuncio(AnunciosPaises anuncioPaisesParaListar)
        {
            List<AnunciosPaises> listaPaisesDeUnAnuncio = await (from anuncioPais in _context.AnunciosPaises
                                                                 where anuncioPais.CodigoAnuncio == anuncioPaisesParaListar.CodigoAnuncio
                                                                 select anuncioPais).AsNoTracking()
                                                                                    .ToListAsync();

            return listaPaisesDeUnAnuncio;
        }

        public void EliminarAnuncioPais(AnunciosPaises anuncioPaisParaEliminar)
        {
            _context.AnunciosPaises.Attach(anuncioPaisParaEliminar);
            _context.AnunciosPaises.Remove(anuncioPaisParaEliminar);
        }

        public void EliminarMultiplesAnuncioPais(AnunciosPaises anuncioPaisParaEliminar)
        {
            _context.AnunciosPaises.RemoveRange(_context.AnunciosPaises.Where(x => x.CodigoAnuncio == anuncioPaisParaEliminar.CodigoAnuncio));
        }


        #endregion


    }
}
