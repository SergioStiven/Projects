using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Repositories
{
    public class CandidatosRepository
    {
        SportsGoEntities _context;

        public CandidatosRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Candidatos


        public void CrearCandidato(Candidatos candidatoParaCrear)
        {
            candidatoParaCrear.Biografia = !string.IsNullOrWhiteSpace(candidatoParaCrear.Biografia) ? candidatoParaCrear.Biografia.Trim() : string.Empty;
            candidatoParaCrear.Alias = !string.IsNullOrWhiteSpace(candidatoParaCrear.Alias) ? candidatoParaCrear.Alias.Trim() : string.Empty;

            candidatoParaCrear.Personas.Nombres = candidatoParaCrear.Personas.Nombres.Trim();
            candidatoParaCrear.Personas.CiudadResidencia = candidatoParaCrear.Personas.CiudadResidencia.Trim();
            candidatoParaCrear.Personas.Telefono = candidatoParaCrear.Personas.Telefono.Trim();
            candidatoParaCrear.Personas.Skype = !string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Skype) ? candidatoParaCrear.Personas.Skype.Trim() : string.Empty;

            candidatoParaCrear.Personas.Usuarios.Usuario = candidatoParaCrear.Personas.Usuarios.Usuario.Trim();
            candidatoParaCrear.Personas.Usuarios.Clave = candidatoParaCrear.Personas.Usuarios.Clave.Trim();
            candidatoParaCrear.Personas.Usuarios.Email = candidatoParaCrear.Personas.Usuarios.Email.Trim();
            candidatoParaCrear.Personas.Usuarios.Creacion = DateTime.Now;

            _context.Candidatos.Add(candidatoParaCrear);
        }

        public async Task<Candidatos> BuscarCandidatoPorCodigoPersona(Candidatos candidatoParaBuscar)
        {
            Candidatos informacionCandidato = await (from candidato in _context.Candidatos
                                                     where candidato.CodigoPersona == candidatoParaBuscar.Personas.Consecutivo
                                                     select candidato).Include(x => x.CategoriasCandidatos)
                                                                      .Include(x => x.CandidatosResponsables)
                                                                      .Include(x => x.Personas)
                                                                      .AsNoTracking()
                                                                      .FirstOrDefaultAsync();

            return informacionCandidato;
        }


        public async Task<Candidatos> BuscarCandidatoPorCodigoCandidato(Candidatos candidatoParaBuscar)
        {
            Candidatos informacionCandidato = await (from candidato in _context.Candidatos
                                                     where candidato.CodigoPersona == candidatoParaBuscar.Consecutivo
                                                     select candidato).Include(x => x.CategoriasCandidatos)
                                                                      .Include(x => x.CandidatosResponsables)
                                                                      .Include(x => x.Personas)
                                                                      .AsNoTracking()
                                                                      .FirstOrDefaultAsync();

            return informacionCandidato;
        }

        public async Task<List<CandidatosDTO>> ListarCandidatos(BuscadorDTO buscador)
        {
            IQueryable<Candidatos> queryCandidatos = _context.Candidatos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscador.IdentificadorParaBuscar))
            {
                buscador.IdentificadorParaBuscar = buscador.IdentificadorParaBuscar.Trim();
                string[] arrayIdentificador = buscador.IdentificadorParaBuscar.Split(new char[] { ' ' }, 2);

                string nombre = arrayIdentificador.ElementAtOrDefault(0);
                string apellido = arrayIdentificador.ElementAtOrDefault(1);

                if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(apellido))
                {
                    queryCandidatos = queryCandidatos.Where(x => x.Personas.Nombres.Trim().ToUpper().Contains(nombre.Trim().ToUpper()) || x.Personas.Apellidos.Trim().ToUpper().Contains(apellido.Trim().ToUpper()));
                }
                else
                {
                    queryCandidatos = queryCandidatos.Where(x => x.Personas.Nombres.Trim().ToUpper().Contains(nombre.Trim().ToUpper()));
                }
            }

            if (buscador.EstaturaInicial > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => x.Estatura >= buscador.EstaturaInicial);
            }

            if (buscador.EstaturaFinal > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => x.Estatura <= buscador.EstaturaFinal);
            }

            if (buscador.PesoInicial > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => x.Peso >= buscador.PesoInicial);
            }

            if (buscador.PesoFinal > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => x.Peso <= buscador.PesoFinal);
            }

            if (buscador.EdadInicio > 0)
            {
                int anioActual = DateTime.Now.Year;
                queryCandidatos = queryCandidatos.Where(x => (anioActual - x.FechaNacimiento.Year) >= buscador.EdadInicio);
            }

            if (buscador.EdadFinal > 0)
            {
                int anioActual = DateTime.Now.Year;
                queryCandidatos = queryCandidatos.Where(x => (anioActual - x.FechaNacimiento.Year) <= buscador.EdadFinal);
            }

            if (buscador.CategoriasParaBuscar != null && buscador.CategoriasParaBuscar.Count > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => x.CategoriasCandidatos.Any(y => buscador.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => buscador.PaisesParaBuscar.Contains(x.Personas.CodigoPais));
            }

            int queryContador = await queryCandidatos.CountAsync();

            List<CandidatosDTO> listaCandidatos = await queryCandidatos
                .Select(x => new CandidatosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoPersona = x.CodigoPersona,
                    CodigoGenero = x.CodigoGenero,
                    Estatura = x.Estatura,
                    Peso = x.Peso,
                    FechaNacimiento = x.FechaNacimiento,
                    NumeroRegistrosExistentes = queryContador,
                    Personas = new PersonasDTO
                    {
                        Consecutivo = x.Personas.Consecutivo,
                        Nombres = x.Personas.Nombres,
                        Apellidos = x.Personas.Apellidos,
                        CodigoPais = x.Personas.CodigoPais,
                        CodigoIdioma = x.Personas.CodigoIdioma,
                        CodigoArchivoImagenPerfil = x.Personas.CodigoArchivoImagenPerfil,
                        CiudadResidencia = x.Personas.CiudadResidencia,
                        Telefono = x.Personas.Telefono,
                        Skype = x.Personas.Skype,
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

            return listaCandidatos;
        }

        public async Task<Candidatos> ModificarInformacionCandidato(Candidatos candidatoParaModificar)
        {
            Candidatos candidatoExistente = await _context.Candidatos.Where(x => x.Consecutivo == candidatoParaModificar.Consecutivo).FirstOrDefaultAsync();

            candidatoExistente.Biografia = !string.IsNullOrWhiteSpace(candidatoParaModificar.Biografia) ? candidatoParaModificar.Biografia.Trim() : string.Empty;
            candidatoExistente.Alias = !string.IsNullOrWhiteSpace(candidatoParaModificar.Alias) ? candidatoParaModificar.Alias.Trim() : string.Empty;

            candidatoExistente.CodigoGenero = candidatoParaModificar.CodigoGenero;
            candidatoExistente.Estatura = candidatoParaModificar.Estatura;
            candidatoExistente.Peso = candidatoParaModificar.Peso;
            candidatoExistente.FechaNacimiento = candidatoParaModificar.FechaNacimiento;

            return candidatoExistente;
        }

        public async Task<Candidatos> DesasignarResponsableDeUnCandidato(Candidatos candidatoParaDesasignarResponsable)
        {
            Candidatos candidatoExistente = await _context.Candidatos.Where(x => x.Consecutivo == candidatoParaDesasignarResponsable.Consecutivo).FirstOrDefaultAsync();

            candidatoExistente.CodigoResponsable = null;

            return candidatoExistente;
        }


        #endregion


        #region Metodos CandidatosVideos


        public void CrearCandidatoVideo(CandidatosVideos candidatoVideoParaCrear)
        {
            candidatoVideoParaCrear.Creacion = DateTime.Now;
            candidatoVideoParaCrear.Titulo = candidatoVideoParaCrear.Titulo.Trim();
            candidatoVideoParaCrear.Descripcion = !string.IsNullOrWhiteSpace(candidatoVideoParaCrear.Descripcion) ? candidatoVideoParaCrear.Descripcion.Trim() : string.Empty;

            _context.CandidatosVideos.Add(candidatoVideoParaCrear);
        }

        public async Task<int?> ConsultarDuracionVideoParaElPlanDeEstaPublicacionCandidato(int codigoCandidatoVideo)
        {
            int? duracionVideoPermitida = await _context.CandidatosVideos.Where(x => x.Consecutivo == codigoCandidatoVideo).Select(x => x.Candidatos.Personas.Usuarios.PlanesUsuarios.Planes.TiempoPermitidoVideo).FirstOrDefaultAsync();

            return duracionVideoPermitida;
        }

        public async Task<CandidatosVideos> ModificarCodigoArchivoCandidatoVideo(CandidatosVideos candidatoVideoParaModificar)
        {
            CandidatosVideos candidatoVideoExistente = await (from candidatoVideo in _context.CandidatosVideos
                                                              where candidatoVideo.Consecutivo == candidatoVideoParaModificar.Consecutivo
                                                              select candidatoVideo).FirstOrDefaultAsync();

            candidatoVideoExistente.CodigoArchivo = candidatoVideoParaModificar.CodigoArchivo;

            return candidatoVideoExistente;
        }

        public async Task<CandidatosVideos> BuscarCandidatoVideo(CandidatosVideos candidatoVideoParaBuscar)
        {
            CandidatosVideos candidatoVideoBuscado = await (from candidatoVideo in _context.CandidatosVideos
                                                            where candidatoVideo.Consecutivo == candidatoVideoParaBuscar.Consecutivo
                                                            select candidatoVideo).Include(x => x.Candidatos)
                                                                                  .AsNoTracking()
                                                                                  .FirstOrDefaultAsync();

            return candidatoVideoBuscado;
        }

        public async Task<List<CandidatosVideosDTO>> ListarCandidatosVideosDeUnCandidato(CandidatosVideos candidatoVideoParaBuscar)
        {
            IQueryable<CandidatosVideos> queryVideosCandidato = _context.CandidatosVideos.Where(x => x.CodigoCandidato == candidatoVideoParaBuscar.CodigoCandidato).AsQueryable();

            if (!string.IsNullOrWhiteSpace(candidatoVideoParaBuscar.IdentificadorParaBuscar))
            {
                candidatoVideoParaBuscar.IdentificadorParaBuscar = candidatoVideoParaBuscar.IdentificadorParaBuscar.Trim();
                queryVideosCandidato = queryVideosCandidato.Where(x => x.Titulo.Contains(candidatoVideoParaBuscar.IdentificadorParaBuscar));
            }

            if (candidatoVideoParaBuscar.FechaFiltroBase != DateTime.MinValue)
            {
                queryVideosCandidato = queryVideosCandidato.Where(x => x.Creacion >= candidatoVideoParaBuscar.FechaFiltroBase);
            }

            int queryContador = await queryVideosCandidato.CountAsync();

            List<CandidatosVideosDTO> listaVideosCandidato = await queryVideosCandidato
                .Select(x => new CandidatosVideosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCandidato = x.CodigoCandidato,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion,
                    Creacion = x.Creacion,
                    CodigoArchivo = x.CodigoArchivo,
                    NumeroRegistrosExistentes = queryContador
                })
                .OrderByDescending(x => x.Creacion)
                .Skip(() => candidatoVideoParaBuscar.SkipIndexBase)
                .Take(() => candidatoVideoParaBuscar.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaVideosCandidato;
        }

        public async Task<CandidatosVideos> ModificarCandidatoVideo(CandidatosVideos candidatoVideoParaModificar)
        {
            CandidatosVideos candidatoVideoExistente = await _context.CandidatosVideos.Where(x => x.Consecutivo == candidatoVideoParaModificar.Consecutivo).FirstOrDefaultAsync();

            candidatoVideoExistente.Titulo = candidatoVideoParaModificar.Titulo.Trim();
            candidatoVideoExistente.Descripcion = !string.IsNullOrWhiteSpace(candidatoVideoParaModificar.Descripcion) ? candidatoVideoParaModificar.Descripcion.Trim() : string.Empty;

            return candidatoVideoExistente;
        }

        public void EliminarCandidatoVideo(CandidatosVideos candidatoVideosParaEliminar)
        {
            _context.CandidatosVideos.Attach(candidatoVideosParaEliminar);
            _context.CandidatosVideos.Remove(candidatoVideosParaEliminar);
        }


        #endregion


        #region Metodos Responsables


        public void CrearCandidatoResponsable(CandidatosResponsables candidatoResponsableParaCrear)
        {
            candidatoResponsableParaCrear.TelefonoMovil = candidatoResponsableParaCrear.TelefonoMovil.Trim();
            candidatoResponsableParaCrear.TelefonoFijo = !string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.TelefonoFijo) ? candidatoResponsableParaCrear.TelefonoFijo.Trim() : string.Empty;
            candidatoResponsableParaCrear.Skype = !string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.Skype) ? candidatoResponsableParaCrear.Skype.Trim() : string.Empty;
            candidatoResponsableParaCrear.Email = candidatoResponsableParaCrear.Email.Trim();
            candidatoResponsableParaCrear.Nombres = candidatoResponsableParaCrear.Nombres.Trim();
            candidatoResponsableParaCrear.Apellidos = !string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.Apellidos) ? candidatoResponsableParaCrear.Apellidos.Trim() : string.Empty;

            _context.CandidatosResponsables.Add(candidatoResponsableParaCrear);
        }

        public async Task<CandidatosResponsables> BuscarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaBuscar)
        {
            CandidatosResponsables candidatoResponsableBuscado = await (from candidatoResponsable in _context.CandidatosResponsables
                                                                        where candidatoResponsable.Consecutivo == candidatoResponsableParaBuscar.Consecutivo
                                                                        select candidatoResponsable).Include(x => x.Candidatos)
                                                                                                    .AsNoTracking()
                                                                                                    .FirstOrDefaultAsync();

            return candidatoResponsableBuscado;
        }

        public async Task<Candidatos> BuscarCandidatoParaAsignar(CandidatosResponsables candidatoResponsableParaBuscar)
        {
            Candidatos candidatoExistente = await _context.Candidatos.Where(x => x.Consecutivo == candidatoResponsableParaBuscar.CodigoCandidato).Include(x => x.CandidatosResponsables).FirstOrDefaultAsync();

            return candidatoExistente;
        }

        public async Task<CandidatosResponsablesDTO> BuscarSoloCandidatoResponsableDTO(CandidatosResponsables candidatoResponsableParaBuscar)
        {
            CandidatosResponsablesDTO candidatoResponsableBuscado = await (from candidatoResponsable in _context.CandidatosResponsables
                                                                        where candidatoResponsable.Consecutivo == candidatoResponsableParaBuscar.Consecutivo
                                                                        select new CandidatosResponsablesDTO
                                                                        {
                                                                            Consecutivo = candidatoResponsable.Consecutivo,
                                                                            Nombres = candidatoResponsable.Nombres,
                                                                            Apellidos = candidatoResponsable.Apellidos,
                                                                            TelefonoFijo = candidatoResponsable.TelefonoFijo,
                                                                            TelefonoMovil = candidatoResponsable.TelefonoMovil,
                                                                            Email = candidatoResponsable.Email,
                                                                            Skype = candidatoResponsable.Skype
                                                                        })
                                                                        .AsNoTracking()
                                                                        .FirstOrDefaultAsync();

            return candidatoResponsableBuscado;
        }

        public async Task<CandidatosResponsables> ModificarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaModificar)
        {
            CandidatosResponsables candidatoResponsableExistente = await _context.CandidatosResponsables.Where(x => x.Consecutivo == candidatoResponsableParaModificar.Consecutivo).FirstOrDefaultAsync();

            candidatoResponsableExistente.TelefonoMovil = candidatoResponsableParaModificar.TelefonoMovil.Trim();
            candidatoResponsableExistente.TelefonoFijo = !string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.TelefonoFijo) ? candidatoResponsableParaModificar.TelefonoFijo.Trim() : string.Empty;
            candidatoResponsableExistente.Skype = !string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.Skype) ? candidatoResponsableParaModificar.Skype.Trim() : string.Empty;
            candidatoResponsableExistente.Email = candidatoResponsableParaModificar.Email.Trim();
            candidatoResponsableExistente.Nombres = candidatoResponsableParaModificar.Nombres.Trim();
            candidatoResponsableExistente.Apellidos = !string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.Apellidos) ? candidatoResponsableParaModificar.Apellidos.Trim() : string.Empty;

            return candidatoResponsableExistente;
        }

        public void EliminarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaBorrar)
        {
            _context.CandidatosResponsables.Attach(candidatoResponsableParaBorrar);
            _context.CandidatosResponsables.Remove(candidatoResponsableParaBorrar);
        }


        #endregion


    }
}