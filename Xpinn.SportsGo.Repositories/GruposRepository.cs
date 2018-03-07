using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Repositories
{
    public class GruposRepository
    {
        SportsGoEntities _context;

        public GruposRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Grupos


        public void CrearGrupo(Grupos grupoParaCrear)
        {
            grupoParaCrear.NombreContacto = !string.IsNullOrWhiteSpace(grupoParaCrear.NombreContacto) ? grupoParaCrear.NombreContacto.Trim() : string.Empty;

            grupoParaCrear.Personas.Nombres = grupoParaCrear.Personas.Nombres.Trim();
            grupoParaCrear.Personas.CiudadResidencia = grupoParaCrear.Personas.CiudadResidencia.Trim();
            grupoParaCrear.Personas.Telefono = grupoParaCrear.Personas.Telefono.Trim();
            grupoParaCrear.Personas.Skype = !string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Skype) ? grupoParaCrear.Personas.Skype.Trim() : string.Empty;

            grupoParaCrear.Personas.Usuarios.Usuario = grupoParaCrear.Personas.Usuarios.Usuario.Trim();
            grupoParaCrear.Personas.Usuarios.Clave = grupoParaCrear.Personas.Usuarios.Clave.Trim();
            grupoParaCrear.Personas.Usuarios.Email = grupoParaCrear.Personas.Usuarios.Email.Trim();
            grupoParaCrear.Personas.Usuarios.Creacion = DateTime.Now;

            _context.Grupos.Add(grupoParaCrear);
        }

        public async Task<Grupos> BuscarGrupoPorCodigoPersona(Grupos grupoParaBuscar)
        {
            Grupos informacionGrupo = await (from grupo in _context.Grupos
                                             where grupo.CodigoPersona == grupoParaBuscar.Personas.Consecutivo
                                             select grupo).Include(x => x.CategoriasGrupos)
                                                          .Include(x => x.Personas)
                                                          .Include(x => x.GruposEventos)
                                                          .AsNoTracking()
                                                          .FirstOrDefaultAsync();

            return informacionGrupo;
        }

        public async Task<Grupos> BuscarGrupoPorCodigoGrupo(Grupos candidatoParaBuscar)
        {
            Grupos informacionGrupo = await (from grupo in _context.Grupos
                                             where grupo.Consecutivo == candidatoParaBuscar.Consecutivo
                                             select grupo).Include(x => x.CategoriasGrupos)
                                                          .Include(x => x.Personas)
                                                          .Include(x => x.GruposEventos)
                                                          .AsNoTracking()
                                                          .FirstOrDefaultAsync();

            return informacionGrupo;
        }

        public async Task<List<GruposDTO>> ListarGrupos(BuscadorDTO buscador)
        {
            IQueryable<Grupos> queryGrupos = _context.Grupos;

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar))
            {
                buscador.IdentificadorParaBuscar = buscador.IdentificadorParaBuscar.Trim();
                string[] arrayIdentificador = buscador.IdentificadorParaBuscar.Split(new char[] { ' ' }, 2);

                string nombre = arrayIdentificador.ElementAtOrDefault(0);
                string apellido = arrayIdentificador.ElementAtOrDefault(1);

                if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(apellido))
                {
                    queryGrupos = queryGrupos.Where(x => x.Personas.Nombres.Contains(nombre) || x.Personas.Apellidos.Contains(apellido));
                }
                else
                {
                    queryGrupos = queryGrupos.Where(x => x.Personas.Nombres.Contains(nombre));
                }
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                queryGrupos = queryGrupos.Where(x => x.CategoriasGrupos.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryGrupos = queryGrupos.Where(x => buscador.PaisesParaBuscar.Contains(x.Personas.CodigoPais));
            }

            int queryContador = await queryGrupos.CountAsync();

            List<GruposDTO> listaGrupos = await queryGrupos
                .Select(x => new GruposDTO
                {
                    Consecutivo = x.Consecutivo,
                    NombreContacto = x.NombreContacto,
                    NumeroRegistrosExistentes = queryContador,
                    Personas = new PersonasDTO
                    {
                        Consecutivo = x.Personas.Consecutivo,
                        Nombres = x.Personas.Nombres,
                        Apellidos = x.Personas.Apellidos,
                        CodigoPais = x.Personas.CodigoPais,
                        CodigoIdioma = x.Personas.CodigoIdioma,
                        CodigoArchivoImagenPerfil = x.Personas.CodigoArchivoImagenPerfil,
                        Skype = x.Personas.Skype,
                        Telefono = x.Personas.Telefono,
                        CiudadResidencia = x.Personas.CiudadResidencia,
                        Paises = new PaisesDTO
                        {
                            Consecutivo = x.Personas.Paises.Consecutivo,
                            CodigoArchivo = x.Personas.Paises.CodigoArchivo,
                            DescripcionIdiomaBuscado = x.Personas.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                        }
                    }
                })
                .OrderBy(x => x.Personas.Nombres)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaGrupos;
        }

        public async Task<Grupos> ModificarInformacionGrupo(Grupos grupoParaModificar)
        {
            Grupos grupoExistente = await _context.Grupos.Where(x => x.Consecutivo == grupoParaModificar.Consecutivo).FirstOrDefaultAsync();

            grupoExistente.NombreContacto = !string.IsNullOrWhiteSpace(grupoParaModificar.NombreContacto) ? grupoParaModificar.NombreContacto.Trim() : string.Empty;

            return grupoExistente;
        }


        #endregion


        #region Metodos GruposEventos


        public void CrearGrupoEvento(GruposEventos grupoEventoParaCrear)
        {
            grupoEventoParaCrear.Creacion = DateTime.Now;
            grupoEventoParaCrear.Titulo = grupoEventoParaCrear.Titulo.Trim();
            grupoEventoParaCrear.Descripcion = !string.IsNullOrWhiteSpace(grupoEventoParaCrear.Descripcion) ? grupoEventoParaCrear.Descripcion.Trim() : string.Empty;
            grupoEventoParaCrear.Ubicacion = !string.IsNullOrWhiteSpace(grupoEventoParaCrear.Ubicacion) ? grupoEventoParaCrear.Ubicacion.Trim() : string.Empty;

            _context.GruposEventos.Add(grupoEventoParaCrear);
        }

        public async Task<GruposEventos> ModificarCodigoArchivoGrupoEvento(GruposEventos grupoEventoParaModificar)
        {
            GruposEventos grupoEventoBuscado = await (from grupoEvento in _context.GruposEventos
                                                      where grupoEvento.Consecutivo == grupoEventoParaModificar.Consecutivo
                                                      select grupoEvento).FirstOrDefaultAsync();

            grupoEventoBuscado.CodigoArchivo = grupoEventoParaModificar.CodigoArchivo;

            return grupoEventoBuscado;
        }

        public async Task<int?> BuscarCodigoPersonaDeUnGrupo(int codigoEvento)
        {
            int? codigoPersonaGrupo = await _context.GruposEventos.Where(x => x.Consecutivo == codigoEvento).Select(x => x.Grupos.Personas.Consecutivo).FirstOrDefaultAsync();

            return codigoPersonaGrupo;
        }

        public async Task<GruposEventosDTO> BuscarGrupoEventoPorConsecutivo(GruposEventos grupoEventoParaBuscar)
        {
            GruposEventosDTO grupoEventoBuscado = await _context.GruposEventos
                                                                .Where(x => x.Consecutivo == grupoEventoParaBuscar.Consecutivo)
                                                                .Select(x => new GruposEventosDTO
                                                                {
                                                                    Consecutivo = x.Consecutivo,
                                                                    CodigoGrupo = x.CodigoGrupo,
                                                                    Titulo = x.Titulo,
                                                                    Descripcion = x.Descripcion,
                                                                    Creacion = x.Creacion,
                                                                    CodigoIdioma = x.CodigoIdioma,
                                                                    CodigoPais = x.CodigoPais,
                                                                    CodigoArchivo = x.CodigoArchivo,
                                                                    FechaInicio = x.FechaInicio,
                                                                    FechaTerminacion = x.FechaTerminacion,
                                                                    CodigoTipoArchivo = _context.Archivos.Where(y => y.Consecutivo == x.CodigoArchivo).Select(y => y.CodigoTipoArchivo).FirstOrDefault(),
                                                                    Ubicacion = x.Ubicacion,
                                                                    NumeroEventosAsistentes = x.GruposEventosAsistentes.Where(y => y.CodigoEvento == x.Consecutivo).Count(),
                                                                    Grupos = new GruposDTO
                                                                    {
                                                                        Consecutivo = x.Grupos.Consecutivo,
                                                                        Personas = new PersonasDTO
                                                                        {
                                                                            Consecutivo = x.Grupos.Personas.Consecutivo,
                                                                            CodigoArchivoImagenPerfil = x.Grupos.Personas.CodigoArchivoImagenPerfil,
                                                                            Nombres = x.Grupos.Personas.Nombres,
                                                                            Apellidos = x.Grupos.Personas.Apellidos
                                                                        }
                                                                    },
                                                                    Idiomas = new IdiomasDTO
                                                                    {
                                                                        Consecutivo = x.Idiomas.Consecutivo
                                                                    },
                                                                    Paises = new PaisesDTO
                                                                    {
                                                                        Consecutivo = x.Paises.Consecutivo,
                                                                        CodigoArchivo = x.Paises.CodigoArchivo,
                                                                        DescripcionIdiomaBuscado = x.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == grupoEventoParaBuscar.CodigoIdioma).Select(y => y.Descripcion).FirstOrDefault()
                                                                    },
                                                                    CategoriasEventos = x.CategoriasEventos.Where(z => z.CodigoEvento == grupoEventoParaBuscar.Consecutivo)
                                                                                            .Select(z => new CategoriasEventosDTO
                                                                                            {
                                                                                                Consecutivo = z.Consecutivo,
                                                                                                CodigoEvento = z.CodigoEvento,
                                                                                                CodigoCategoria = z.CodigoCategoria,
                                                                                                Categorias = new CategoriasDTO
                                                                                                {
                                                                                                    Consecutivo = z.Categorias.Consecutivo,
                                                                                                    CodigoArchivo = z.Categorias.CodigoArchivo,
                                                                                                    DescripcionIdiomaBuscado = z.Categorias.CategoriasContenidos
                                                                                                                                .Where(h => h.CodigoIdioma == grupoEventoParaBuscar.CodigoIdioma)
                                                                                                                                .Select(h => h.Descripcion).FirstOrDefault()
                                                                                                }
                                                                                            }).ToList()
                                                                })
                                                                .AsNoTracking()
                                                                .FirstOrDefaultAsync();

            return grupoEventoBuscado;
        }

        public async Task<int?> BuscarArchivoGrupoEvento(GruposEventos grupoEventoParaBuscar)
        {
            int? grupoEventoBuscado = await (from grupoEvento in _context.GruposEventos
                                             where grupoEvento.Consecutivo == grupoEventoParaBuscar.Consecutivo
                                             select grupoEvento.CodigoArchivo).FirstOrDefaultAsync();

            return grupoEventoBuscado;
        }

        public async Task<List<GruposEventosDTO>> ListarEventosDeUnGrupo(BuscadorDTO buscador)
        {
            IQueryable<GruposEventos> queryEventosDeUnGrupo = _context.GruposEventos.Where(x => x.CodigoGrupo == buscador.ConsecutivoPerfil).AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar))
            {
                buscador.IdentificadorParaBuscar = buscador.IdentificadorParaBuscar.Trim();
                queryEventosDeUnGrupo = queryEventosDeUnGrupo.Where(x => x.Titulo.Trim().ToUpper().Contains(buscador.IdentificadorParaBuscar.Trim().ToUpper()));
            }

            if (buscador.FechaFiltroBase != DateTime.MinValue)
            {
                queryEventosDeUnGrupo = queryEventosDeUnGrupo.Where(x => x.Creacion >= buscador.FechaFiltroBase);
            }

            if (buscador.FechaInicio != DateTime.MinValue)
            {
                queryEventosDeUnGrupo = queryEventosDeUnGrupo.Where(x => x.FechaInicio >= buscador.FechaInicio);
            }

            if (buscador.FechaFinal != DateTime.MinValue)
            {
                queryEventosDeUnGrupo = queryEventosDeUnGrupo.Where(x => x.FechaTerminacion <= buscador.FechaFinal);
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                queryEventosDeUnGrupo = queryEventosDeUnGrupo.Where(x => x.CategoriasEventos.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryEventosDeUnGrupo = queryEventosDeUnGrupo.Where(x => buscador.PaisesParaBuscar.Contains(x.CodigoPais));
            }

            if (buscador.IdiomaBase != Idioma.SinIdioma)
            {
                queryEventosDeUnGrupo = queryEventosDeUnGrupo.Where(x => x.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase);
            }

            int queryContador = await queryEventosDeUnGrupo.CountAsync();

            List<GruposEventosDTO> listaEventosDeUnGrupo = await queryEventosDeUnGrupo
                .Select(x => new GruposEventosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoGrupo = x.CodigoGrupo,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion,
                    Creacion = x.Creacion,
                    CodigoIdioma = x.CodigoIdioma,
                    CodigoPais = x.CodigoPais,
                    CodigoArchivo = x.CodigoArchivo,
                    CodigoTipoArchivo = _context.Archivos.Where(y => y.Consecutivo == x.CodigoArchivo).Select(y => y.CodigoTipoArchivo).FirstOrDefault(),
                    FechaInicio = x.FechaInicio,
                    FechaTerminacion = x.FechaTerminacion,
                    Ubicacion = x.Ubicacion,
                    NumeroRegistrosExistentes = queryContador,
                    NumeroEventosAsistentes = x.GruposEventosAsistentes.Where(y => y.CodigoEvento == x.Consecutivo).Count(),
                    CategoriasEventos = x.CategoriasEventos.Where(z => z.CodigoEvento == x.Consecutivo)
                                                           .Select(z => new CategoriasEventosDTO
                                                           {
                                                               Consecutivo = z.Consecutivo,
                                                               CodigoEvento = z.CodigoEvento,
                                                               CodigoCategoria = z.CodigoCategoria
                                                           }).ToList(),
                    Grupos = new GruposDTO
                    {
                        Consecutivo = x.Grupos.Consecutivo,
                        CodigoPersona = x.Grupos.CodigoPersona,
                        NombreContacto = x.Grupos.NombreContacto,
                        Personas = new PersonasDTO
                        {
                            Consecutivo = x.Grupos.Personas.Consecutivo,
                            CodigoArchivoImagenPerfil = x.Grupos.Personas.CodigoArchivoImagenPerfil,
                            CodigoIdioma = x.Grupos.Personas.CodigoIdioma,
                            CodigoPais = x.Grupos.Personas.CodigoPais
                        }
                    }
                })
                .OrderByDescending(x => x.Creacion)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaEventosDeUnGrupo;
        }

        public async Task<List<GruposEventosDTO>> ListarEventos(BuscadorDTO buscador, bool isTimeLineQuery = false)
        {
            IQueryable<GruposEventos> grupoEventos = _context.GruposEventos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar))
            {
                buscador.IdentificadorParaBuscar = buscador.IdentificadorParaBuscar.Trim();
                grupoEventos = grupoEventos.Where(x => x.Titulo.Trim().ToUpper().Contains(buscador.IdentificadorParaBuscar.Trim().ToUpper()));
            }

            if (buscador.FechaInicio != DateTime.MinValue && isTimeLineQuery)
            {
                grupoEventos = grupoEventos.Where(x => x.Creacion >= buscador.FechaInicio);
            }
            else if(buscador.FechaInicio != DateTime.MinValue)
            {
                grupoEventos = grupoEventos.Where(x => x.FechaInicio >= buscador.FechaInicio);
            }

            if (buscador.FechaFinal != DateTime.MinValue)
            {
                grupoEventos = grupoEventos.Where(x => x.FechaTerminacion <= buscador.FechaFinal);
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                grupoEventos = grupoEventos.Where(x => x.CategoriasEventos.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                grupoEventos = grupoEventos.Where(x => buscador.PaisesParaBuscar.Contains(x.CodigoPais));
            }

            if (buscador.IdiomaBase != Idioma.SinIdioma)
            {
                grupoEventos = grupoEventos.Where(x => x.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase);
            }

            if (isTimeLineQuery)
            {
                List<GruposEventosDTO> listaEventosTimeLine = await grupoEventos
                    .Select(x => new GruposEventosDTO
                    {
                        Consecutivo = x.Consecutivo,
                        CodigoGrupo = x.CodigoGrupo,
                        Titulo = x.Titulo,
                        Descripcion = x.Descripcion,
                        Creacion = x.Creacion,
                        CodigoIdioma = x.CodigoIdioma,
                        CodigoPais = x.CodigoPais,
                        CodigoArchivo = x.CodigoArchivo,
                        CodigoTipoArchivo = _context.Archivos.Where(y => y.Consecutivo == x.CodigoArchivo).Select(y => y.CodigoTipoArchivo).FirstOrDefault(),
                        FechaInicio = x.FechaInicio,
                        FechaTerminacion = x.FechaTerminacion,
                        Grupos = new GruposDTO
                        {
                            Consecutivo = x.Grupos.Consecutivo,
                            CodigoPersona = x.Grupos.CodigoPersona,
                            Personas = new PersonasDTO
                            {
                                Consecutivo = x.Grupos.Personas.Consecutivo,
                                CodigoArchivoImagenPerfil = x.Grupos.Personas.CodigoArchivoImagenPerfil,
                                Nombres = x.Grupos.Personas.Nombres,
                                Apellidos = x.Grupos.Personas.Apellidos
                            }
                        },
                    })
                    .OrderByDescending(x => x.Creacion)
                    .Skip(() => buscador.SkipIndexBase)
                    .Take(() => buscador.TakeIndexBase)
                    .AsNoTracking()
                    .ToListAsync();

                return listaEventosTimeLine;
            }
            else
            {
                int queryContador = await grupoEventos.CountAsync();

                List<GruposEventosDTO> listaEventos = await grupoEventos
                    .Select(x => new GruposEventosDTO
                    {
                        Consecutivo = x.Consecutivo,
                        CodigoGrupo = x.CodigoGrupo,
                        Titulo = x.Titulo,
                        Descripcion = x.Descripcion,
                        Creacion = x.Creacion,
                        CodigoIdioma = x.CodigoIdioma,
                        CodigoPais = x.CodigoPais,
                        CodigoArchivo = x.CodigoArchivo,
                        CodigoTipoArchivo = _context.Archivos.Where(y => y.Consecutivo == x.CodigoArchivo).Select(y => y.CodigoTipoArchivo).FirstOrDefault(),
                        FechaInicio = x.FechaInicio,
                        FechaTerminacion = x.FechaTerminacion,
                        Ubicacion = x.Ubicacion,
                        NumeroRegistrosExistentes = queryContador,
                        NumeroEventosAsistentes = x.GruposEventosAsistentes.Where(y => y.CodigoEvento == x.Consecutivo).Count(),
                        Grupos = new GruposDTO
                        {
                            Consecutivo = x.Grupos.Consecutivo,
                            Personas = new PersonasDTO
                            {
                                Consecutivo = x.Grupos.Personas.Consecutivo,
                                CodigoArchivoImagenPerfil = x.Grupos.Personas.CodigoArchivoImagenPerfil,
                                Nombres = x.Grupos.Personas.Nombres,
                                Apellidos = x.Grupos.Personas.Apellidos
                            }
                        },
                        Paises = new PaisesDTO
                        {
                            Consecutivo = x.Paises.Consecutivo,
                            CodigoArchivo = x.Paises.CodigoArchivo,
                            DescripcionIdiomaBuscado = x.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                        }
                    })
                    .OrderByDescending(x => x.Creacion)
                    .Skip(() => buscador.SkipIndexBase)
                    .Take(() => buscador.TakeIndexBase)
                    .AsNoTracking()
                    .ToListAsync();

                return listaEventos;
            }
        }

        public async Task<int?> ConsultarDuracionVideoParaElPlanDeEsteEvento(int codigoEvento)
        {
            int? duracionVideoPermitida = await _context.GruposEventos.Where(x => x.Consecutivo == codigoEvento).Select(x => x.Grupos.Personas.Usuarios.PlanesUsuarios.Planes.TiempoPermitidoVideo).FirstOrDefaultAsync();

            return duracionVideoPermitida;
        }

        public async Task<GruposEventos> ModificarInformacionGrupoEvento(GruposEventos grupoEventoParaModificar)
        {
            GruposEventos grupoEventoExistente = await _context.GruposEventos.Where(x => x.Consecutivo == grupoEventoParaModificar.Consecutivo).FirstOrDefaultAsync();

            grupoEventoExistente.Titulo = grupoEventoParaModificar.Titulo.Trim();
            grupoEventoExistente.Descripcion = !string.IsNullOrWhiteSpace(grupoEventoParaModificar.Descripcion) ? grupoEventoParaModificar.Descripcion.Trim() : string.Empty;
            grupoEventoExistente.Ubicacion = !string.IsNullOrWhiteSpace(grupoEventoParaModificar.Ubicacion) ? grupoEventoParaModificar.Ubicacion.Trim() : string.Empty;
            grupoEventoExistente.FechaInicio = grupoEventoParaModificar.FechaInicio;
            grupoEventoExistente.FechaTerminacion = grupoEventoParaModificar.FechaTerminacion;
            grupoEventoExistente.CodigoPais = grupoEventoParaModificar.CodigoPais;
            grupoEventoExistente.CodigoIdioma = grupoEventoParaModificar.CodigoIdioma;
            grupoEventoExistente.CodigoArchivo = grupoEventoParaModificar.CodigoArchivo;

            return grupoEventoExistente;
        }

        public void EliminarGrupoEvento(GruposEventos grupoEventoParaEliminar)
        {
            _context.GruposEventos.Attach(grupoEventoParaEliminar);
            _context.GruposEventos.Remove(grupoEventoParaEliminar);
        }

        public async Task<GruposEventos> DesasignarArchivoGrupoEvento(GruposEventos grupoEventoArchivoParaBorrar)
        {
            GruposEventos grupoEventoExistente = await _context.GruposEventos.Where(x => x.Consecutivo == grupoEventoArchivoParaBorrar.Consecutivo).FirstOrDefaultAsync();

            grupoEventoExistente.CodigoArchivo = null;

            return grupoEventoExistente;
        }


        #endregion


        #region Metodos GruposEventosAsistentes


        public void CrearGruposEventosAsistentes(GruposEventosAsistentes grupoEventoAsistentesParaCrear)
        {
            _context.GruposEventosAsistentes.Add(grupoEventoAsistentesParaCrear);
        }

        public async Task<WrapperSimpleTypesDTO> BuscarNumeroAsistentesGruposEventos(GruposEventosAsistentes grupoEventoAsistenteParaBuscar)
        {
            WrapperSimpleTypesDTO grupoEventoBuscado = new WrapperSimpleTypesDTO();

            grupoEventoBuscado.NumeroRegistrosExistentes = await (from grupoEventoAsistentes in _context.GruposEventosAsistentes
                                                                  where grupoEventoAsistentes.CodigoEvento == grupoEventoAsistenteParaBuscar.CodigoEvento
                                                                  select grupoEventoAsistentes).CountAsync();

            return grupoEventoBuscado;
        }

        public async Task<WrapperSimpleTypesDTO> BuscarSiPersonaAsisteAGrupoEvento(GruposEventosAsistentes grupoEventoAsistenteParaBuscar)
        {
            WrapperSimpleTypesDTO grupoEventoBuscado = new WrapperSimpleTypesDTO();

            grupoEventoBuscado.Existe = await (from grupoEventoAsistentes in _context.GruposEventosAsistentes
                                               where grupoEventoAsistentes.CodigoEvento == grupoEventoAsistenteParaBuscar.CodigoEvento
                                               && grupoEventoAsistentes.CodigoPersona == grupoEventoAsistenteParaBuscar.CodigoPersona
                                               select grupoEventoAsistentes).AnyAsync();

            return grupoEventoBuscado;
        }

        public async Task<List<GruposEventosAsistentesDTO>> ListarEventosAsistentesDeUnEvento(GruposEventosAsistentes grupoEventoAsistenteParaListar)
        {
            IQueryable<GruposEventosAsistentes> queryEventosAsistentesDeUnGrupo = _context.GruposEventosAsistentes.Where(x => x.CodigoEvento == grupoEventoAsistenteParaListar.CodigoEvento).AsQueryable();

            if (!string.IsNullOrWhiteSpace(grupoEventoAsistenteParaListar.IdentificadorParaBuscar))
            {
                grupoEventoAsistenteParaListar.IdentificadorParaBuscar = grupoEventoAsistenteParaListar.IdentificadorParaBuscar.Trim();
                string[] arrayIdentificador = grupoEventoAsistenteParaListar.IdentificadorParaBuscar.Split(new char[] { ' ' }, 2);

                string nombre = arrayIdentificador.ElementAtOrDefault(0);
                string apellido = arrayIdentificador.ElementAtOrDefault(1);

                if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(apellido))
                {
                    queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.Personas.Nombres.Trim().ToUpper().Contains(nombre.Trim().ToUpper()) || x.Personas.Apellidos.Trim().ToUpper().Contains(apellido.Trim().ToUpper()));
                }
                else
                {
                    queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.Personas.Nombres.Trim().ToUpper().Contains(nombre.Trim().ToUpper()));
                }
            }

            int queryContador = await queryEventosAsistentesDeUnGrupo.CountAsync();

            List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnEvento = await queryEventosAsistentesDeUnGrupo
                .Select(x => new GruposEventosAsistentesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoEvento = x.CodigoEvento,
                    CodigoPersona = x.CodigoPersona,
                    NumeroRegistrosExistentes = queryContador,
                    Personas = new PersonasDTO
                    {
                        Consecutivo = x.Personas.Consecutivo,
                        Nombres = x.Personas.Nombres,
                        Apellidos = x.Personas.Apellidos,
                        CodigoIdioma = x.Personas.CodigoIdioma,
                        CodigoPais = x.Personas.CodigoPais,
                        CodigoArchivoImagenPerfil = x.Personas.CodigoArchivoImagenPerfil,
                        Paises = new PaisesDTO
                        {
                            Consecutivo = x.Personas.Paises.Consecutivo,
                            CodigoArchivo = x.Personas.Paises.CodigoArchivo,
                            DescripcionIdiomaBuscado = x.Personas.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == grupoEventoAsistenteParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                        }
                    }
                })
                .OrderBy(x => x.Personas.Nombres)
                .Skip(() => grupoEventoAsistenteParaListar.SkipIndexBase)
                .Take(() => grupoEventoAsistenteParaListar.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaEventosAsistentesDeUnEvento;
        }

        public async Task<List<GruposEventosAsistentesDTO>> ListarEventosAsistentesDeUnaPersona(BuscadorDTO buscador)
        {
            IQueryable<GruposEventosAsistentes> queryEventosAsistentesDeUnGrupo = _context.GruposEventosAsistentes.Where(x => x.CodigoPersona == buscador.ConsecutivoPersona).AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar)) 
            {
                buscador.IdentificadorParaBuscar = buscador.IdentificadorParaBuscar.Trim();
                queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.GruposEventos.Titulo.Trim().ToUpper().Contains(buscador.IdentificadorParaBuscar.Trim().ToUpper()));
            }

            if (buscador.FechaFiltroBase != DateTime.MinValue)
            {
                queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.GruposEventos.Creacion >= buscador.FechaFiltroBase);
            }

            if (buscador.FechaInicio != DateTime.MinValue)
            {
                queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.GruposEventos.FechaInicio >= buscador.FechaInicio);
            }

            if (buscador.FechaFinal != DateTime.MinValue)
            {
                queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.GruposEventos.FechaTerminacion <= buscador.FechaFinal);
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.GruposEventos.CategoriasEventos.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => buscador.PaisesParaBuscar.Contains(x.GruposEventos.CodigoPais));
            }

            if (buscador.IdiomaBase != Idioma.SinIdioma)
            {
                queryEventosAsistentesDeUnGrupo = queryEventosAsistentesDeUnGrupo.Where(x => x.GruposEventos.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase);
            }

            int queryContador = await queryEventosAsistentesDeUnGrupo.CountAsync();

            List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnaPersona = await queryEventosAsistentesDeUnGrupo
                .Select(x => new GruposEventosAsistentesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoEvento = x.CodigoEvento,
                    CodigoPersona = x.CodigoPersona,
                    NumeroRegistrosExistentes = queryContador,
                    GruposEventos = new GruposEventosDTO
                    {
                        Consecutivo = x.GruposEventos.Consecutivo,
                        CodigoGrupo = x.GruposEventos.CodigoGrupo,
                        Titulo = x.GruposEventos.Titulo,
                        Descripcion = x.GruposEventos.Descripcion,
                        FechaInicio = x.GruposEventos.FechaInicio,
                        FechaTerminacion = x.GruposEventos.FechaTerminacion,
                        CodigoPais = x.GruposEventos.CodigoPais,
                        CodigoIdioma = x.GruposEventos.CodigoIdioma,
                        Grupos = new GruposDTO
                        {
                            Consecutivo = x.GruposEventos.Grupos.Consecutivo,
                            Personas = new PersonasDTO
                            {
                                Consecutivo = x.GruposEventos.Grupos.Personas.Consecutivo,
                                CodigoArchivoImagenPerfil = x.GruposEventos.Grupos.Personas.CodigoArchivoImagenPerfil,
                                Nombres = x.GruposEventos.Grupos.Personas.Nombres,
                                Apellidos = x.GruposEventos.Grupos.Personas.Apellidos
                            }
                        },
                        Paises = new PaisesDTO
                        {
                            Consecutivo = x.GruposEventos.Paises.Consecutivo,
                            CodigoArchivo = x.GruposEventos.Paises.CodigoArchivo,
                            DescripcionIdiomaBuscado = x.GruposEventos.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                        },
                    },
                })
                .OrderBy(x => x.Consecutivo)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaEventosAsistentesDeUnaPersona;
        }

        public async Task<GruposEventosAsistentes> EliminarGrupoEventoAsistente(GruposEventosAsistentes grupoEventoAsistenteParaEliminar)
        {
            GruposEventosAsistentes grupoEventoAsistente = await _context.GruposEventosAsistentes.Where(x => x.CodigoEvento == grupoEventoAsistenteParaEliminar.CodigoEvento && x.CodigoPersona == grupoEventoAsistenteParaEliminar.CodigoPersona).FirstOrDefaultAsync();

            _context.GruposEventosAsistentes.Remove(grupoEventoAsistente);

            return grupoEventoAsistente;
        }

        public void EliminarMultiplesGrupoEventoAsistente(GruposEventosAsistentes grupoEventoAsistenteParaEliminar)
        {
            _context.GruposEventosAsistentes.RemoveRange(_context.GruposEventosAsistentes.Where(x => x.CodigoEvento == grupoEventoAsistenteParaEliminar.CodigoEvento));
        }


        #endregion


    }
}