using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class MetricasRepository
    {
        SportsGoEntities _context;

        public MetricasRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Metricas Usuarios


        public async Task<WrapperSimpleTypesDTO> NumeroUsuariosRegistrados()
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            wrapper.NumeroRegistrosExistentes = await _context.Usuarios.CountAsync();

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroUsuariosRegistradosUltimoMes()
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            DateTime fechaInicio = DateTimeHelper.PrimerDiaDelMes(DateTime.Today.AddMonths(-1));
            DateTime fechaFinal = DateTimeHelper.UltimoDiaDelMes(DateTime.Today.AddMonths(-1));

            wrapper.NumeroRegistrosExistentes = await _context.Usuarios.Where(x => x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).CountAsync();

            return wrapper;
        }

        public async Task<int> MetricasCandidatos(MetricasDTO metricasParaBuscar)
        {
            IQueryable<Candidatos> queryCandidatos = _context.Candidatos.AsQueryable();

            if (metricasParaBuscar.CategoriasParaBuscar != null && metricasParaBuscar.CategoriasParaBuscar.Count > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => x.CategoriasCandidatos.Any(y => metricasParaBuscar.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (metricasParaBuscar.PaisesParaBuscar != null && metricasParaBuscar.PaisesParaBuscar.Count > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => metricasParaBuscar.PaisesParaBuscar.Contains(x.Personas.CodigoPais));
            }

            if (metricasParaBuscar.IdiomasParaBuscar != null && metricasParaBuscar.IdiomasParaBuscar.Count > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => metricasParaBuscar.IdiomasParaBuscar.Contains(x.Personas.CodigoIdioma));
            }

            if (metricasParaBuscar.PlanesParaBuscar != null && metricasParaBuscar.PlanesParaBuscar.Count > 0)
            {
                queryCandidatos = queryCandidatos.Where(x => metricasParaBuscar.PlanesParaBuscar.Contains(x.Personas.Usuarios.PlanesUsuarios.CodigoPlan));
            }

            return await queryCandidatos.CountAsync();
        }

        public Task<int> NumeroVentasUltimoMes()
        {
            int estadoDelpago = (int)EstadoDeLosPagos.Aprobado;
            DateTime fechaInicio = DateTimeHelper.PrimerDiaDelMes(DateTime.Today.AddMonths(-1));
            DateTime fechaFinal = DateTimeHelper.UltimoDiaDelMes(DateTime.Today.AddMonths(-1));

            return _context.HistorialPagosPersonas.Where(x => x.CodigoEstado == estadoDelpago && x.FechaPago >= fechaInicio && x.FechaPago <= fechaFinal).CountAsync();
        }

        public async Task<int> MetricasGrupos(MetricasDTO metricasParaBuscar)
        {
            IQueryable<Grupos> queryGrupos = _context.Grupos.AsQueryable();

            if (metricasParaBuscar.CategoriasParaBuscar != null && metricasParaBuscar.CategoriasParaBuscar.Count > 0)
            {
                queryGrupos = queryGrupos.Where(x => x.CategoriasGrupos.Any(y => metricasParaBuscar.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (metricasParaBuscar.PaisesParaBuscar != null && metricasParaBuscar.PaisesParaBuscar.Count > 0)
            {
                queryGrupos = queryGrupos.Where(x => metricasParaBuscar.PaisesParaBuscar.Contains(x.Personas.CodigoPais));
            }

            if (metricasParaBuscar.IdiomasParaBuscar != null && metricasParaBuscar.IdiomasParaBuscar.Count > 0)
            {
                queryGrupos = queryGrupos.Where(x => metricasParaBuscar.IdiomasParaBuscar.Contains(x.Personas.CodigoIdioma));
            }

            if (metricasParaBuscar.PlanesParaBuscar != null && metricasParaBuscar.PlanesParaBuscar.Count > 0)
            {
                queryGrupos = queryGrupos.Where(x => metricasParaBuscar.PlanesParaBuscar.Contains(x.Personas.Usuarios.PlanesUsuarios.CodigoPlan));
            }

            return await queryGrupos.CountAsync();
        }

        public async Task<int> MetricasRepresentantes(MetricasDTO metricasParaBuscar)
        {
            IQueryable<Representantes> queryRepresentantes = _context.Representantes.AsQueryable();

            if (metricasParaBuscar.CategoriasParaBuscar != null && metricasParaBuscar.CategoriasParaBuscar.Count > 0)
            {
                queryRepresentantes = queryRepresentantes.Where(x => x.CategoriasRepresentantes.Any(y => metricasParaBuscar.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (metricasParaBuscar.PaisesParaBuscar != null && metricasParaBuscar.PaisesParaBuscar.Count > 0)
            {
                queryRepresentantes = queryRepresentantes.Where(x => metricasParaBuscar.PaisesParaBuscar.Contains(x.Personas.CodigoPais));
            }

            if (metricasParaBuscar.IdiomasParaBuscar != null && metricasParaBuscar.IdiomasParaBuscar.Count > 0)
            {
                queryRepresentantes = queryRepresentantes.Where(x => metricasParaBuscar.IdiomasParaBuscar.Contains(x.Personas.CodigoIdioma));
            }

            if (metricasParaBuscar.PlanesParaBuscar != null && metricasParaBuscar.PlanesParaBuscar.Count > 0)
            {
                queryRepresentantes = queryRepresentantes.Where(x => metricasParaBuscar.PlanesParaBuscar.Contains(x.Personas.Usuarios.PlanesUsuarios.CodigoPlan));
            }

            return await queryRepresentantes.CountAsync();
        }

        public async Task<List<PersonasDTO>> ListarUsuariosMetricas(MetricasDTO metricasParaBuscar)
        {
            IQueryable<Personas> listaUsuarioMetricas = (from persona in _context.Personas
                                                         select persona).AsQueryable();

            if (metricasParaBuscar.PaisesParaBuscar != null && metricasParaBuscar.PaisesParaBuscar.Count > 0)
            {
                listaUsuarioMetricas = listaUsuarioMetricas.Where(x => metricasParaBuscar.PaisesParaBuscar.Contains(x.CodigoPais));
            }

            if (metricasParaBuscar.IdiomasParaBuscar != null && metricasParaBuscar.IdiomasParaBuscar.Count > 0)
            {
                listaUsuarioMetricas = listaUsuarioMetricas.Where(x => metricasParaBuscar.IdiomasParaBuscar.Contains(x.CodigoIdioma));
            }

            if (metricasParaBuscar.PerfilesParaBuscar != null && metricasParaBuscar.PerfilesParaBuscar.Count > 0)
            {
                listaUsuarioMetricas = listaUsuarioMetricas.Where(x => metricasParaBuscar.PerfilesParaBuscar.Contains(x.CodigoTipoPerfil));
            }

            if (!string.IsNullOrWhiteSpace(metricasParaBuscar.UsuarioParaBuscar))
            {
                listaUsuarioMetricas = listaUsuarioMetricas.Where(x => x.Usuarios.Usuario.Trim().ToUpper().Contains(metricasParaBuscar.UsuarioParaBuscar.Trim().ToUpper()));
            }

            if (!string.IsNullOrWhiteSpace(metricasParaBuscar.NombrePersonaParaBuscar))
            {
                listaUsuarioMetricas = listaUsuarioMetricas.Where(x => x.Nombres.Trim().ToUpper().Contains(metricasParaBuscar.NombrePersonaParaBuscar.Trim().ToUpper()));
            }

            if (!string.IsNullOrWhiteSpace(metricasParaBuscar.EmailParaBuscar))
            {
                listaUsuarioMetricas = listaUsuarioMetricas.Where(x => x.Usuarios.Email.Trim().ToUpper().Contains(metricasParaBuscar.EmailParaBuscar.Trim().ToUpper()));
            }

            int numeroRegistrosExistentes = await listaUsuarioMetricas.CountAsync();

            return await listaUsuarioMetricas.Select(x =>
            new PersonasDTO
            {
                Consecutivo = x.Consecutivo,
                Nombres = x.Nombres,
                Apellidos = x.Apellidos,
                CodigoIdioma = x.CodigoIdioma,
                CodigoPais = x.CodigoPais,
                CodigoTipoPerfil = x.CodigoTipoPerfil,
                CodigoArchivoImagenPerfil = x.CodigoArchivoImagenPerfil,
                CiudadResidencia = x.CiudadResidencia,
                Telefono = x.Telefono,
                NumeroRegistrosExistentes = numeroRegistrosExistentes,
                CodigoUsuario = x.CodigoUsuario,
                Paises = new PaisesDTO
                {
                    Consecutivo = x.Paises.Consecutivo,
                    CodigoArchivo = x.Paises.CodigoArchivo,
                    DescripcionIdiomaBuscado = x.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == metricasParaBuscar.CodigoIdiomaUsuarioBase).Select(z => z.Descripcion).FirstOrDefault()
                },
                Usuarios = new UsuariosDTO
                {
                    Consecutivo = x.Usuarios.Consecutivo,
                    Usuario = x.Usuarios.Usuario,
                    Email = x.Usuarios.Email,
                    CodigoTipoPerfil = x.Usuarios.CodigoTipoPerfil,
                    CodigoPlanUsuario = x.Usuarios.CodigoPlanUsuario,
                    Creacion = x.Usuarios.Creacion,
                    CuentaActiva = x.Usuarios.CuentaActiva,
                    PlanesUsuarios = new PlanesUsuariosDTO
                    {
                        Consecutivo = x.Usuarios.PlanesUsuarios.Consecutivo,
                        CodigoPlan = x.Usuarios.PlanesUsuarios.CodigoPlan,
                        NumeroCategoriasUsadas = x.Usuarios.PlanesUsuarios.NumeroCategoriasUsadas
                    }
                }
            })
            .OrderByDescending(x => x.Usuarios.Creacion)
            .Skip(() => metricasParaBuscar.SkipIndexBase)
            .Take(() => metricasParaBuscar.TakeIndexBase)
            .AsNoTracking()
            .ToListAsync();
        }


        #endregion


        #region Metodos Metricas Anunciantes


        public async Task<WrapperSimpleTypesDTO> NumeroAnunciantesRegistrados()
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            wrapper.NumeroRegistrosExistentes = await _context.Anunciantes.CountAsync();

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroAnunciantesRegistradosUltimoMes()
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            DateTime fechaInicio = DateTimeHelper.PrimerDiaDelMes(DateTime.Today.AddMonths(-1));
            DateTime fechaFinal = DateTimeHelper.UltimoDiaDelMes(DateTime.Today.AddMonths(-1));

            wrapper.NumeroRegistrosExistentes = await _context.Anunciantes.Where(x => x.Personas.Usuarios.Creacion >= fechaInicio && x.Personas.Usuarios.Creacion <= fechaFinal).CountAsync();

            return wrapper;
        }


        #endregion


        #region Metodos Metricas Anuncios


        public async Task<WrapperSimpleTypesDTO> NumeroAnunciosRegistrados(MetricasDTO metricasParaBuscar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            if (metricasParaBuscar != null && metricasParaBuscar.CodigoAnunciante > 0)
            {
                wrapper.NumeroRegistrosExistentes = await _context.Anuncios.Where(x => x.CodigoAnunciante == metricasParaBuscar.CodigoAnunciante).CountAsync();
            }
            else
            {
                wrapper.NumeroRegistrosExistentes = await _context.Anuncios.CountAsync();
            }

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroAnunciosRegistradosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            DateTime fechaInicio = DateTimeHelper.PrimerDiaDelMes(DateTime.Today.AddMonths(-1));
            DateTime fechaFinal = DateTimeHelper.UltimoDiaDelMes(DateTime.Today.AddMonths(-1));

            if (metricasParaBuscar != null && metricasParaBuscar.CodigoAnunciante > 0)
            {
                wrapper.NumeroRegistrosExistentes = await _context.Anuncios.Where(x => x.CodigoAnunciante == metricasParaBuscar.CodigoAnunciante && x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).CountAsync();
            }
            else
            {
                wrapper.NumeroRegistrosExistentes = await _context.Anuncios.Where(x => x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).CountAsync();
            }

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesClickeados(MetricasDTO metricasParaBuscar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            if (metricasParaBuscar != null && metricasParaBuscar.CodigoAnunciante > 0)
            {
                int? numeroVecesClickeado = await _context.Anuncios.Where(x => x.CodigoAnunciante == metricasParaBuscar.CodigoAnunciante).Select(x => x.NumeroVecesClickeados).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesClickeado.HasValue ? numeroVecesClickeado.Value : 0;
            }
            else
            {
                int? numeroVecesClickeado = await _context.Anuncios.Select(x => x.NumeroVecesClickeados).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesClickeado.HasValue ? numeroVecesClickeado.Value : 0;
            }

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesClickeadosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            DateTime fechaInicio = DateTimeHelper.PrimerDiaDelMes(DateTime.Today.AddMonths(-1));
            DateTime fechaFinal = DateTimeHelper.UltimoDiaDelMes(DateTime.Today.AddMonths(-1));

            if (metricasParaBuscar != null && metricasParaBuscar.CodigoAnunciante > 0)
            {
                int? numeroVecesClickeado = await _context.Anuncios.Where(x => x.CodigoAnunciante == metricasParaBuscar.CodigoAnunciante && x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).Select(x => x.NumeroVecesClickeados).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesClickeado.HasValue ? numeroVecesClickeado.Value : 0;
            }
            else
            {
                int? numeroVecesClickeado = await _context.Anuncios.Where(x => x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).Select(x => x.NumeroVecesClickeados).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesClickeado.HasValue ? numeroVecesClickeado.Value : 0;
            }

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesVistos(MetricasDTO metricasParaBuscar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            if (metricasParaBuscar != null && metricasParaBuscar.CodigoAnunciante > 0)
            {
                int? numeroVecesVistos = await _context.Anuncios.Where(x => x.CodigoAnunciante == metricasParaBuscar.CodigoAnunciante).Select(x => x.NumeroVecesVistos).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesVistos.HasValue ? numeroVecesVistos.Value : 0;
            }
            else
            {
                int? numeroVecesVistos = await _context.Anuncios.Select(x => x.NumeroVecesVistos).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesVistos.HasValue ? numeroVecesVistos.Value : 0;
            }

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesVistosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            DateTime fechaInicio = DateTimeHelper.PrimerDiaDelMes(DateTime.Today.AddMonths(-1));
            DateTime fechaFinal = DateTimeHelper.UltimoDiaDelMes(DateTime.Today.AddMonths(-1));

            if (metricasParaBuscar != null && metricasParaBuscar.CodigoAnunciante > 0)
            {
                int? numeroVecesVistos = await _context.Anuncios.Where(x => x.CodigoAnunciante == metricasParaBuscar.CodigoAnunciante && x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).Select(x => x.NumeroVecesVistos).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesVistos.HasValue ? numeroVecesVistos.Value : 0;
            }
            else
            {
                int? numeroVecesVistos = await _context.Anuncios.Where(x => x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).Select(x => x.NumeroVecesVistos).SumAsync();
                wrapper.NumeroRegistrosExistentes = numeroVecesVistos.HasValue ? numeroVecesVistos.Value : 0;
            }

            return wrapper;
        }

        public async Task<int> MetricasAnuncios(MetricasDTO metricasParaBuscar)
        {
            IQueryable<Anuncios> queryAnuncios = _context.Anuncios.AsQueryable();

            if (metricasParaBuscar.CodigoAnunciante > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.CodigoAnunciante == metricasParaBuscar.CodigoAnunciante);
            }

            if (metricasParaBuscar.CategoriasParaBuscar != null && metricasParaBuscar.CategoriasParaBuscar.Count > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.CategoriasAnuncios.Any(y => metricasParaBuscar.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (metricasParaBuscar.PaisesParaBuscar != null && metricasParaBuscar.PaisesParaBuscar.Count > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosPaises.Any(y => metricasParaBuscar.PaisesParaBuscar.Contains(y.CodigoPais)));
            }

            if (metricasParaBuscar.IdiomasParaBuscar != null && metricasParaBuscar.IdiomasParaBuscar.Count > 0)
            {
                queryAnuncios = queryAnuncios.Where(x => x.AnunciosContenidos.Any(y => metricasParaBuscar.IdiomasParaBuscar.Contains(y.CodigoIdioma)));
            }

            return await queryAnuncios.CountAsync();
        }


        #endregion


        #region Metodos Metricas Eventos


        public async Task<WrapperSimpleTypesDTO> NumeroEventosRegistrados()
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            wrapper.NumeroRegistrosExistentes = await _context.GruposEventos.CountAsync();

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroEventosRegistradosUltimoMes()
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            DateTime fechaInicio = DateTimeHelper.PrimerDiaDelMes(DateTime.Today.AddMonths(-1));
            DateTime fechaFinal = DateTimeHelper.UltimoDiaDelMes(DateTime.Today.AddMonths(-1));

            wrapper.NumeroRegistrosExistentes = await _context.GruposEventos.Where(x => x.Creacion >= fechaInicio && x.Creacion <= fechaFinal).CountAsync();

            return wrapper;
        }

        public async Task<int> MetricasEventos(MetricasDTO metricasParaBuscar)
        {
            IQueryable<GruposEventos> queryEventos = _context.GruposEventos.AsQueryable();

            if (metricasParaBuscar.CategoriasParaBuscar != null && metricasParaBuscar.CategoriasParaBuscar.Count > 0)
            {
                queryEventos = queryEventos.Where(x => x.CategoriasEventos.Any(y => metricasParaBuscar.CategoriasParaBuscar.Contains(y.CodigoCategoria)));
            }

            if (metricasParaBuscar.PaisesParaBuscar != null && metricasParaBuscar.PaisesParaBuscar.Count > 0)
            {
                queryEventos = queryEventos.Where(x => metricasParaBuscar.PaisesParaBuscar.Contains(x.CodigoPais));
            }

            if (metricasParaBuscar.IdiomasParaBuscar != null && metricasParaBuscar.IdiomasParaBuscar.Count > 0)
            {
                queryEventos = queryEventos.Where(x => metricasParaBuscar.IdiomasParaBuscar.Contains(x.CodigoIdioma));
            }

            return await queryEventos.CountAsync();
        }


        #endregion


    }
}
