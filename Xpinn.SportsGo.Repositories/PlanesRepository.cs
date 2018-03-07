using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class PlanesRepository
    {
        SportsGoEntities _context;

        public PlanesRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Planes


        public void CrearPlan(Planes planParaCrear)
        {
            planParaCrear.Creacion = DateTime.Today;

            _context.Planes.Add(planParaCrear);
        }

        public async Task<Planes> BuscarPlan(Planes planParaBuscar)
        {
            Planes planBuscado = await (from plan in _context.Planes
                                        where plan.Consecutivo == planParaBuscar.Consecutivo
                                        select plan).Include(x => x.PlanesContenidos)
                                                    .Include(x => x.Periodicidades)
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();

            return planBuscado;
        }

        public async Task<Planes> BuscarPlanDefaultDeUnPerfil(Planes planParaBuscar)
        {
            Planes planBuscado = await (from plan in _context.Planes
                                        where plan.CodigoTipoPerfil == planParaBuscar.CodigoTipoPerfil
                                        && plan.PlanDefault == 1
                                        select plan).AsNoTracking()
                                                    .FirstOrDefaultAsync();

            return planBuscado;
        }

        public async Task<int?> BuscarCodigoPlanDefault(TipoPerfil tipoPerfil)
        {
            int si = (int)SiNoEnum.Si;
            int codigoTipoPerfil = (int)tipoPerfil;
            int? codigoPlanDefault = await _context.Planes.Where(x => x.PlanDefault == si && x.CodigoTipoPerfil == codigoTipoPerfil).Select(x => x.Consecutivo).FirstOrDefaultAsync();

            return codigoPlanDefault;
        }

        public async Task<int> NumeroPlanesDefault(TipoPerfil tipoPerfil)
        {
            int si = (int)SiNoEnum.Si;
            int codigoTipoPerfil = (int)tipoPerfil;
            int numeroDePlanesDefault = await _context.Planes.Where(x => x.PlanDefault == si && x.CodigoTipoPerfil == codigoTipoPerfil).Select(x => x.Consecutivo).CountAsync();

            return numeroDePlanesDefault;
        }

        public async Task<int?> BuscarCodigoTipoPerfilDeUnPlan(Planes planParaBuscar)
        {
            int? codigoTipoPerfil = await _context.Planes.Where(x => x.Consecutivo == planParaBuscar.Consecutivo).Select(x => x.CodigoTipoPerfil).FirstOrDefaultAsync();

            return codigoTipoPerfil;
        }

        public async Task<bool> BuscarSiPlanEsDefault(Planes planParaBuscar)
        {
            int? codigoTipoPerfil = await _context.Planes.Where(x => x.Consecutivo == planParaBuscar.Consecutivo).Select(x => x.PlanDefault).FirstOrDefaultAsync();

            return codigoTipoPerfil.HasValue && codigoTipoPerfil.Value == 1;
        }

        public async Task<List<PlanesDTO>> ListarPlanesPorIdioma(Planes planParaListar)
        {
            int codigoPerfilAdministrador = (int)TipoPerfil.Administrador;

            IQueryable<Planes> queryPlanes = _context.Planes.Where(x => x.CodigoTipoPerfil != codigoPerfilAdministrador).AsQueryable();

            if (planParaListar.TipoPerfil != TipoPerfil.SinTipoPerfil)
            {
                queryPlanes = queryPlanes.Where(x => x.CodigoTipoPerfil == planParaListar.CodigoTipoPerfil);
            }

            int numeroRegistros = await queryPlanes.CountAsync();

            List<PlanesDTO> listaPlanes = await queryPlanes
                .Select(x => new PlanesDTO
                {
                    Consecutivo = x.Consecutivo,
                    Creacion = x.Creacion,
                    Precio = x.Precio,
                    PrecioEnMonedaFantasma = x.Precio,
                    CodigoPeriodicidad = x.CodigoPeriodicidad,
                    PlanDefault = x.PlanDefault,
                    VideosPerfil = x.VideosPerfil,
                    ServiciosChat = x.ServiciosChat,
                    ConsultaCandidatos = x.ConsultaCandidatos,
                    DetalleCandidatos = x.DetalleCandidatos,
                    ConsultaGrupos = x.ConsultaGrupos,
                    DetalleGrupos = x.DetalleGrupos,
                    ConsultaEventos = x.ConsultaEventos,
                    CreacionAnuncios = x.CreacionAnuncios,
                    EstadisticasAnuncios = x.EstadisticasAnuncios,
                    Modificacion = x.Modificacion,
                    CodigoTipoPerfil = x.CodigoTipoPerfil,
                    NumeroCategoriasPermisibles = x.NumeroCategoriasPermisibles,
                    CodigoArchivo = x.CodigoArchivo,
                    TiempoPermitidoVideo = x.TiempoPermitidoVideo,
                    NumeroRegistrosExistentes = numeroRegistros,
                    NumeroAparicionesAnuncio = x.NumeroAparicionesAnuncio,
                    NumeroDiasVigenciaAnuncio = x.NumeroDiasVigenciaAnuncio,
                    DescripcionIdiomaBuscado = x.PlanesContenidos.Where(y => y.CodigoIdioma == planParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    PlanesContenidos = x.PlanesContenidos
                                        .Select(z => new PlanesContenidosDTO
                                        {
                                            Consecutivo = z.Consecutivo,
                                            Descripcion = z.Descripcion,
                                            CodigoIdioma = z.CodigoIdioma,
                                            CodigoPlan = z.CodigoPlan
                                        }).ToList(),
                    Periodicidades = new PeriodicidadesDTO
                    {
                        Consecutivo = x.Periodicidades.Consecutivo,
                        Descripcion = x.Periodicidades.Descripcion,
                        NumeroDias = x.Periodicidades.NumeroDias
                    }
                })
                .OrderBy(x => x.DescripcionIdiomaBuscado)
                .Skip(() => planParaListar.SkipIndexBase)
                .Take(() => planParaListar.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaPlanes;
        }

        public async Task<Planes> ModificarPlan(Planes planParaModificar)
        {
            Planes planExistente = await _context.Planes.Where(x => x.Consecutivo == planParaModificar.Consecutivo).FirstOrDefaultAsync();

            planExistente.Modificacion = DateTime.Now;
            planExistente.Precio = planParaModificar.Precio;
            planExistente.CodigoPeriodicidad = planParaModificar.CodigoPeriodicidad;
            planExistente.VideosPerfil = planParaModificar.VideosPerfil;
            planExistente.ServiciosChat = planParaModificar.ServiciosChat;
            planExistente.ConsultaCandidatos = planParaModificar.ConsultaCandidatos;
            planExistente.DetalleCandidatos = planParaModificar.DetalleCandidatos;
            planExistente.ConsultaGrupos = planParaModificar.ConsultaGrupos;
            planExistente.DetalleGrupos = planParaModificar.DetalleGrupos;
            planExistente.ConsultaEventos = planParaModificar.ConsultaEventos;
            planExistente.CreacionAnuncios = planParaModificar.CreacionAnuncios;
            planExistente.EstadisticasAnuncios = planParaModificar.EstadisticasAnuncios;
            planExistente.NumeroCategoriasPermisibles = planParaModificar.NumeroCategoriasPermisibles;
            planExistente.CodigoTipoPerfil = planParaModificar.CodigoTipoPerfil;
            planExistente.NumeroAparicionesAnuncio = planParaModificar.NumeroAparicionesAnuncio;
            planExistente.NumeroDiasVigenciaAnuncio = planParaModificar.NumeroDiasVigenciaAnuncio;
            planExistente.TiempoPermitidoVideo = planParaModificar.TiempoPermitidoVideo;

            return planExistente;
        }

        public async Task<Planes> AsignarPlanDefault(Planes planParaAsignar)
        {
            Planes planExistente = await _context.Planes.Where(x => x.Consecutivo == planParaAsignar.Consecutivo).FirstOrDefaultAsync();

            planExistente.Modificacion = DateTime.Now;
            planExistente.PlanDefault = (int)SiNoEnum.Si;

            return planExistente;
        }

        public async Task DesasignarPlanDefaultDeUnPerfilMenosActual(int codigoPlanExcepcion, int codigoTipoPerfil)
        {
            List<Planes> listaPlanExistente = await _context.Planes.Where(x => x.Consecutivo != codigoPlanExcepcion && x.CodigoTipoPerfil == codigoTipoPerfil).ToListAsync();

            foreach (var planExistente in listaPlanExistente)
            {
                planExistente.PlanDefault = (int)SiNoEnum.No;
            }
        }

        public async Task<Planes> DesasignarPlanDefault(Planes planParaDesasignar)
        {
            Planes planExistente = await _context.Planes.Where(x => x.Consecutivo == planParaDesasignar.Consecutivo).FirstOrDefaultAsync();

            planExistente.Modificacion = DateTime.Now;
            planExistente.PlanDefault = (int)SiNoEnum.No;

            return planExistente;
        }

        public void EliminarPlan(Planes planParaEliminar)
        {
            _context.Planes.Attach(planParaEliminar);
            _context.Planes.Remove(planParaEliminar);
        }


        #endregion


        #region Metodos PlanesContenidos


        public void CrearPlanesContenidos(ICollection<PlanesContenidos> planesContenidosParaCrear)
        {
            planesContenidosParaCrear.ForEach(x => x.Descripcion.Trim());
            _context.PlanesContenidos.AddRange(planesContenidosParaCrear);
        }

        public void CrearPlanContenido(PlanesContenidos planesContenidosParaCrear)
        {
            planesContenidosParaCrear.Descripcion.Trim();
            _context.PlanesContenidos.Add(planesContenidosParaCrear);
        }

        public async Task<PlanesContenidos> BuscarPlanContenido(PlanesContenidos planParaBuscar)
        {
            PlanesContenidos planContenidoBuscado = await (from planContenido in _context.PlanesContenidos
                                                           where planContenido.Consecutivo == planParaBuscar.Consecutivo
                                                           select planContenido).Include(x => x.Idiomas)
                                                                                .Include(x => x.Planes)
                                                                                .AsNoTracking()
                                                                                .FirstOrDefaultAsync();

            return planContenidoBuscado;
        }

        public async Task<List<PlanesContenidos>> ListarContenidoDeUnPlan(PlanesContenidos planContenidoParaListar)
        {
            List<PlanesContenidos> listaPlanesContenidos = await (from planContenido in _context.PlanesContenidos
                                                                  where planContenido.CodigoPlan == planContenidoParaListar.CodigoPlan
                                                                  select planContenido).Include(x => x.Idiomas)
                                                                                       .AsNoTracking()
                                                                                       .ToListAsync();

            return listaPlanesContenidos;
        }

        public async Task<PlanesContenidos> ModificarPlanContenido(PlanesContenidos planContenidoParaModificar)
        {
            PlanesContenidos planContenidoExistente = await _context.PlanesContenidos.Where(x => x.Consecutivo == planContenidoParaModificar.Consecutivo).FirstOrDefaultAsync();

            planContenidoExistente.Descripcion = planContenidoParaModificar.Descripcion.Trim();

            return planContenidoExistente;
        }

        public void EliminarPlanContenido(PlanesContenidos planContenidoParaEliminar)
        {
            _context.PlanesContenidos.Attach(planContenidoParaEliminar);
            _context.PlanesContenidos.Remove(planContenidoParaEliminar);
        }

        public void EliminarMultiplesPlanesContenidos(PlanesContenidos planContenidoParaEliminar)
        {
            _context.PlanesContenidos.RemoveRange(_context.PlanesContenidos.Where(x => x.CodigoPlan == planContenidoParaEliminar.CodigoPlan));
        }


        #endregion


        #region Metodos PlanesUsuarios


        public async Task<PlanesUsuariosDTO> BuscarPlanUsuario(PlanesUsuarios planParaBuscar)
        {
            IQueryable<PlanesUsuarios> queryPlanUsuario = _context.PlanesUsuarios.Where(x => x.Consecutivo == planParaBuscar.Consecutivo).AsQueryable();

            PlanesUsuariosDTO planUsuarioBuscado = await queryPlanUsuario
                .Select(x => new PlanesUsuariosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoPlan = x.CodigoPlan,
                    Adquisicion = x.Adquisicion,
                    Vencimiento = x.Vencimiento,
                    NumeroCategoriasUsadas = x.NumeroCategoriasUsadas,
                    Planes = new PlanesDTO
                    {
                        Consecutivo = x.Planes.Consecutivo,
                        Creacion = x.Planes.Creacion,
                        CodigoArchivo = x.Planes.CodigoArchivo,
                        Precio = x.Planes.Precio,
                        PrecioEnMonedaFantasma = x.Planes.Precio,
                        CodigoPeriodicidad = x.Planes.CodigoPeriodicidad,
                        PlanDefault = x.Planes.PlanDefault,
                        VideosPerfil = x.Planes.VideosPerfil,
                        ServiciosChat = x.Planes.ServiciosChat,
                        ConsultaCandidatos = x.Planes.ConsultaCandidatos,
                        DetalleCandidatos = x.Planes.DetalleCandidatos,
                        ConsultaGrupos = x.Planes.ConsultaGrupos,
                        DetalleGrupos = x.Planes.DetalleGrupos,
                        ConsultaEventos = x.Planes.ConsultaEventos,
                        CreacionAnuncios = x.Planes.CreacionAnuncios,
                        EstadisticasAnuncios = x.Planes.EstadisticasAnuncios,
                        Modificacion = x.Planes.Modificacion,
                        TiempoPermitidoVideo = x.Planes.TiempoPermitidoVideo,
                        CodigoTipoPerfil = x.Planes.CodigoTipoPerfil,
                        NumeroCategoriasPermisibles = x.Planes.NumeroCategoriasPermisibles,
                        NumeroAparicionesAnuncio = x.Planes.NumeroAparicionesAnuncio,
                        NumeroDiasVigenciaAnuncio = x.Planes.NumeroDiasVigenciaAnuncio,
                        DescripcionIdiomaBuscado = x.Planes.PlanesContenidos.Where(z => z.CodigoIdioma == planParaBuscar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return planUsuarioBuscado;
        }

        public async Task<int?> BuscarCodigoPlanUsuarioPorCodigoCandidato(int codigoCandidato)
        {
            int? codigoPlanExistente = await _context.Candidatos.Where(x => x.Consecutivo == codigoCandidato)
                                                                .Select(x => x.Personas.Usuarios.PlanesUsuarios.Consecutivo)
                                                                .FirstOrDefaultAsync();

            return codigoPlanExistente;
        }

        public async Task<int?> BuscarCodigoPlanUsuarioPorCodigoGrupo(int codigoGrupo)
        {
            int? codigoPlanExistente = await _context.Grupos.Where(x => x.Consecutivo == codigoGrupo)
                                                            .Select(x => x.Personas.Usuarios.PlanesUsuarios.Consecutivo)
                                                            .FirstOrDefaultAsync();

            return codigoPlanExistente;
        }

        public async Task<int?> BuscarCodigoPlanUsuarioPorCodigoRepresentante(int codigoRepresentante)
        {
            int? codigoPlanExistente = await _context.Representantes.Where(x => x.Consecutivo == codigoRepresentante)
                                                                    .Select(x => x.Personas.Usuarios.PlanesUsuarios.Consecutivo)
                                                                    .FirstOrDefaultAsync();

            return codigoPlanExistente;
        }

        public async Task<decimal?> BuscarPrecioDeUnPlan(int codigoPlan)
        {
            decimal? precioDelPlan = await _context.Planes.Where(x => x.Consecutivo == codigoPlan)
                                                          .Select(x => x.Precio)
                                                          .FirstOrDefaultAsync();

            return precioDelPlan;
        }

        public async Task<int?> BuscarCodigoPlanUsuarioPorCodigoPersona(int codigoPersona)
        {
            int? codigoPlanExistente = await _context.Personas.Where(x => x.Consecutivo == codigoPersona)
                                                                .Select(x => x.Usuarios.PlanesUsuarios.Consecutivo)
                                                                .FirstOrDefaultAsync();

            return codigoPlanExistente;
        }

        public async Task<int?> BuscarCodigoUsuarioPorCodigoPlanUsuario(int codigoPlanUsuario)
        {
            int? codigoUsuario = await _context.Usuarios.Where(x => x.CodigoPlanUsuario == codigoPlanUsuario)
                                                                .Select(x => x.Consecutivo)
                                                                .FirstOrDefaultAsync();

            return codigoUsuario;
        }

        public async Task<DateTime> BuscarFechaVencimientoPlan(PlanesUsuarios planParaBuscar)
        {
            DateTime fechaVencimiento = await _context.PlanesUsuarios.Where(x => x.Consecutivo == planParaBuscar.Consecutivo).Select(x => x.Vencimiento).FirstOrDefaultAsync();

            return fechaVencimiento;
        }

        public async Task<PlanesUsuarios> ModificarNumeroCategoriasUsadas(int codigoPlanUsuario, int numeroParaAumentarODisminuir)
        {
            PlanesUsuarios planUsuarioExistente = await _context.PlanesUsuarios.Where(x => x.Consecutivo == codigoPlanUsuario).FirstOrDefaultAsync();

            planUsuarioExistente.NumeroCategoriasUsadas += numeroParaAumentarODisminuir;

            return planUsuarioExistente;
        }

        public async Task<bool> VerificarSiPlanSoportaLaOperacion(PlanesUsuarios planParaValidar, TipoOperacion tipoOperacion)
        {
            PlanesUsuarios planUsuario = await _context.PlanesUsuarios.Where(x => x.Consecutivo == planParaValidar.Consecutivo)
                                                                      .Select(x => x)
                                                                      .Include(x => x.Planes)
                                                                      .FirstOrDefaultAsync();

            if (planUsuario.Vencimiento < DateTime.Now && planUsuario.Planes.PlanDefault != 1)
            {
                return false;
            }

            bool esPosible = false;

            switch (tipoOperacion)
            {
                case TipoOperacion.VideosPerfil:
                    if (planUsuario.Planes.VideosPerfil.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.ServiciosChat:
                    if (planUsuario.Planes.ServiciosChat.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.ConsultaCandidatos:
                    if (planUsuario.Planes.ConsultaCandidatos.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.DetalleCandidatos:
                    if (planUsuario.Planes.DetalleCandidatos.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.ConsultaGrupos:
                    if (planUsuario.Planes.ConsultaGrupos.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.DetalleGrupos:
                    if (planUsuario.Planes.DetalleGrupos.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.ConsultaEventos:
                    if (planUsuario.Planes.ConsultaEventos.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.CreacionAnuncios:
                    if (planUsuario.Planes.CreacionAnuncios.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.EstadisticasAnuncios:
                    if (planUsuario.Planes.EstadisticasAnuncios.ToEnum<SiNoEnum>() == SiNoEnum.Si)
                        esPosible = true;
                    break;
                case TipoOperacion.MultiplesCategorias:
                    if (planUsuario.Planes.NumeroCategoriasPermisibles >= planUsuario.NumeroCategoriasUsadas + 1)
                        esPosible = true;
                    break;
                default:
                    throw new InvalidOperationException("Tipo de Operacion Invalido!.");
            }

            return esPosible;
        }

        public async Task<PlanesUsuarios> CambiarDePlanUsuario(PlanesUsuarios planParaModificar)
        {
            PlanesUsuarios planExistente = await _context.PlanesUsuarios.Where(x => x.Consecutivo == planParaModificar.Consecutivo).FirstOrDefaultAsync();

            planExistente.Adquisicion = DateTime.Now;
            planExistente.Vencimiento = planParaModificar.Vencimiento;
            planExistente.CodigoPlan = planParaModificar.CodigoPlanDeseado;

            return planExistente;
        }

        public async Task<DateTime> CalcularFechaVencimientoPlanUsuario(PlanesUsuarios planParaCalcular)
        {
            int numeroDiasPeriodicidad = await _context.Planes.Where(x => x.Consecutivo == planParaCalcular.CodigoPlanDeseado)
                                                              .Select(x => x.Periodicidades.NumeroDias)
                                                              .FirstOrDefaultAsync();

            DateTime fechaVencimiento = DateTime.Now.AddDays(numeroDiasPeriodicidad);

            return fechaVencimiento;
        }


        #endregion


    }
}
