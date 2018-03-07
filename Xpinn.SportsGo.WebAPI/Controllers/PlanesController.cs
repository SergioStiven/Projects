using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Entities;
using Microsoft.AspNet.SignalR;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class PlanesController : ApiController
    {
        PlanesBusiness _planesBusiness;

        public PlanesController()
        {
            _planesBusiness = new PlanesBusiness();
        }


        #region Metodos Planes


        public async Task<IHttpActionResult> CrearPlan(Planes planParaCrear)
        {
            if (planParaCrear == null || planParaCrear.Archivos == null 
                || planParaCrear.Precio < 0 || planParaCrear.CodigoPeriodicidad <= 0 
                || (planParaCrear.VideosPerfil < 0 || planParaCrear.VideosPerfil > 1)
                || (planParaCrear.ServiciosChat < 0 | planParaCrear.ServiciosChat > 1)
                || (planParaCrear.ConsultaCandidatos < 0 || planParaCrear.ConsultaCandidatos > 1)
                || (planParaCrear.DetalleCandidatos < 0 || planParaCrear.DetalleCandidatos > 1)
                || (planParaCrear.ConsultaGrupos < 0 || planParaCrear.ConsultaGrupos > 1)
                || (planParaCrear.DetalleGrupos < 0 || planParaCrear.DetalleGrupos > 1)
                || (planParaCrear.ConsultaEventos < 0 || planParaCrear.ConsultaEventos > 1)
                || (planParaCrear.CreacionAnuncios < 0 || planParaCrear.CreacionAnuncios > 1)
                || (planParaCrear.EstadisticasAnuncios < 0 || planParaCrear.EstadisticasAnuncios > 1)
                || (planParaCrear.TiempoPermitidoVideo < AppConstants.MinimoSegundos || planParaCrear.TiempoPermitidoVideo > AppConstants.MaximoSegundos)
                || planParaCrear.NumeroCategoriasPermisibles <= 0 || planParaCrear.Archivos.ArchivoContenido == null 
                || planParaCrear.TipoPerfil == TipoPerfil.SinTipoPerfil)
            {
                return BadRequest("planParaCrear vacio y/o invalido!.");
            }
            else if (planParaCrear.TipoPerfil == TipoPerfil.Anunciante && (planParaCrear.NumeroDiasVigenciaAnuncio <= 0 && planParaCrear.NumeroAparicionesAnuncio <= 0))
            {
                return BadRequest("planParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearPlan = await _planesBusiness.CrearPlan(planParaCrear);

                //if (wrapperCrearPlan.Exitoso)
                //{
                //    NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

                //    IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                //    hubContext.Clients.Group(ChatHub._prefixUserGroupName + timeLineNotificaciones.CodigoPersonaDestino.ToString()).receiveNotification(timeLineNotificaciones);
                //}

                return Ok(wrapperCrearPlan);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarPlan(Planes planParaBuscar)
        {
            if (planParaBuscar == null || planParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("planParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Planes planExistente = await _planesBusiness.BuscarPlan(planParaBuscar);

                return Ok(planExistente);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarPlanDefaultDeUnPerfil(Planes planParaBuscar)
        {
            if (planParaBuscar == null || planParaBuscar.TipoPerfil == TipoPerfil.SinTipoPerfil)
            {
                return BadRequest("planParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Planes planExistente = await _planesBusiness.BuscarPlanDefaultDeUnPerfil(planParaBuscar);

                return Ok(planExistente);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarPlanesAdministrador(Planes planParaListar)
        {
            if (planParaListar == null || planParaListar.IdiomaBase == Idioma.SinIdioma
                || planParaListar.SkipIndexBase < 0 || planParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("planParaListar vacio y/o invalido!.");
            }

            try
            {
                List<PlanesDTO> listaPlanes = await _planesBusiness.ListarPlanesAdministrador(planParaListar);

                return Ok(listaPlanes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarPlanesPorIdioma(Planes planParaListar)
        {
            if (planParaListar == null || planParaListar.IdiomaBase == Idioma.SinIdioma || planParaListar.CodigoPaisParaBuscarMoneda <= 0
                || planParaListar.SkipIndexBase < 0 || planParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("planParaListar vacio y/o invalido!.");
            }

            try
            {
                List<PlanesDTO> listaPlanes = await _planesBusiness.ListarPlanesPorIdioma(planParaListar);

                return Ok(listaPlanes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarPlan(Planes planParaModificar)
        {
            if (planParaModificar == null || planParaModificar.Consecutivo <= 0 || planParaModificar.Precio < 0 || planParaModificar.CodigoPeriodicidad <= 0
                || (planParaModificar.VideosPerfil < 0 || planParaModificar.VideosPerfil > 1)
                || (planParaModificar.ServiciosChat < 0 | planParaModificar.ServiciosChat > 1)
                || (planParaModificar.ConsultaCandidatos < 0 || planParaModificar.ConsultaCandidatos > 1)
                || (planParaModificar.DetalleCandidatos < 0 || planParaModificar.DetalleCandidatos > 1)
                || (planParaModificar.ConsultaGrupos < 0 || planParaModificar.ConsultaGrupos > 1)
                || (planParaModificar.DetalleGrupos < 0 || planParaModificar.DetalleGrupos > 1)
                || (planParaModificar.ConsultaEventos < 0 || planParaModificar.ConsultaEventos > 1)
                || (planParaModificar.CreacionAnuncios < 0 || planParaModificar.CreacionAnuncios > 1)
                || (planParaModificar.EstadisticasAnuncios < 0 || planParaModificar.EstadisticasAnuncios > 1)
                || (planParaModificar.TiempoPermitidoVideo < AppConstants.MinimoSegundos || planParaModificar.TiempoPermitidoVideo > AppConstants.MaximoSegundos)
                || planParaModificar.NumeroCategoriasPermisibles <= 0 || planParaModificar.TipoPerfil == TipoPerfil.SinTipoPerfil)
            {
                return BadRequest("planParaModificar vacio y/o invalido!.");
            }
            else if (planParaModificar.TipoPerfil == TipoPerfil.Anunciante && (planParaModificar.NumeroDiasVigenciaAnuncio <= 0 && planParaModificar.NumeroAparicionesAnuncio <= 0))
            {
                return BadRequest("planParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarPlan = await _planesBusiness.ModificarPlan(planParaModificar);

                return Ok(wrapperModificarPlan);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarArchivoPlan(Planes planArchivoParaModificar)
        {
            if (planArchivoParaModificar == null || planArchivoParaModificar.CodigoArchivo <= 0 || planArchivoParaModificar.ArchivoContenido == null)
            {
                return BadRequest("planArchivoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarArchivoPlan = await _planesBusiness.ModificarArchivoPlan(planArchivoParaModificar);

                return Ok(wrapperModificarArchivoPlan);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> AsignarPlanDefault(Planes planParaAsignar)
        {
            if (planParaAsignar == null || planParaAsignar.Consecutivo <= 0)
            {
                return BadRequest("planParaAsignar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAsignarPlanDefault = await _planesBusiness.AsignarPlanDefault(planParaAsignar);

                return Ok(wrapperAsignarPlanDefault);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> DesasignarPlanDefault(Planes planParaDesasignar)
        {
            if (planParaDesasignar == null || planParaDesasignar.Consecutivo <= 0)
            {
                return BadRequest("planParaDesasignar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperDesasignarPlanDefault = await _planesBusiness.DesasignarPlanDefault(planParaDesasignar);

                return Ok(wrapperDesasignarPlanDefault);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarPlan(Planes planParaEliminar)
        {
            if (planParaEliminar == null || planParaEliminar.Consecutivo <= 0 || planParaEliminar.CodigoArchivo <= 0)
            {
                return BadRequest("planParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperPlanParaEliminar = await _planesBusiness.EliminarPlan(planParaEliminar);

                return Ok(wrapperPlanParaEliminar);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion


        #region Metodos PlanesContenidos


        public async Task<IHttpActionResult> CrearPlanesContenidos(List<PlanesContenidos> planContenidoParaCrear)
        {
            if (planContenidoParaCrear == null || planContenidoParaCrear.Count <= 0 ||
                !planContenidoParaCrear.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.CodigoPlan > 0))
            {
                return BadRequest("planContenidoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearPlanesContenidos = await _planesBusiness.CrearPlanesContenidos(planContenidoParaCrear);

                return Ok(wrapperCrearPlanesContenidos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarPlanContenido(PlanesContenidos planContenidoParaBuscar)
        {
            if (planContenidoParaBuscar == null || planContenidoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("planContenidoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                PlanesContenidos planContenidoBuscado = await _planesBusiness.BuscarPlanContenido(planContenidoParaBuscar);

                return Ok(planContenidoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarContenidoDeUnPlan(PlanesContenidos planContenidoParaListar)
        {
            if (planContenidoParaListar == null || planContenidoParaListar.CodigoPlan <= 0)
            {
                return BadRequest("planContenidoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<PlanesContenidos> listaPlanesContenidos = await _planesBusiness.ListarContenidoDeUnPlan(planContenidoParaListar);

                return Ok(listaPlanesContenidos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarPlanContenido(PlanesContenidos planContenidoParaModificar)
        {
            if (planContenidoParaModificar == null || planContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(planContenidoParaModificar.Descripcion))
            {
                return BadRequest("planContenidoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarPlanContenido = await _planesBusiness.ModificarPlanContenido(planContenidoParaModificar);

                return Ok(wrapperModificarPlanContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarPlanContenido(PlanesContenidos planContenidoParaEliminar)
        {
            if (planContenidoParaEliminar == null || planContenidoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("planContenidoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarPlanContenido = await _planesBusiness.EliminarPlanContenido(planContenidoParaEliminar);

                return Ok(wrapperEliminarPlanContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos PlanesUsuarios


        public async Task<IHttpActionResult> BuscarPlanUsuario(PlanesUsuarios planUsuarioParaBuscar)
        {
            if (planUsuarioParaBuscar == null || planUsuarioParaBuscar.Consecutivo <= 0 || planUsuarioParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("planUsuarioParaBuscar vacio y/o invalido!.");
            }

            try
            {
                PlanesUsuariosDTO planUsuarioExistente = await _planesBusiness.BuscarPlanUsuario(planUsuarioParaBuscar);

                return Ok(planUsuarioExistente);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarSiPlanSoportaLaOperacion(PlanesUsuarios planUsuarioParaValidar)
        {
            if (planUsuarioParaValidar == null || planUsuarioParaValidar.Consecutivo <= 0 || planUsuarioParaValidar.TipoOperacionBase == TipoOperacion.SinOperacion)
            {
                return BadRequest("planUsuarioParaValidar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperVerificarPlanOperacion = await _planesBusiness.VerificarSiPlanSoportaLaOperacion(planUsuarioParaValidar);

                return Ok(wrapperVerificarPlanOperacion);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> CambiarDePlanUsuario(PlanesUsuarios planParaCambiar)
        {
            if (planParaCambiar == null || planParaCambiar.Consecutivo <= 0 || planParaCambiar.CodigoPlanDeseado <= 0)
            {
                return BadRequest("planParaCambiar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCambiarDePlanUsuario = await _planesBusiness.CambiarDePlanUsuario(planParaCambiar);

                return Ok(wrapperCambiarDePlanUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
