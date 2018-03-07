using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Entities;
using System.IO;
using Microsoft.AspNet.SignalR;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class AdministracionController : ApiController
    {
        AdministracionBusiness _adminBusiness;

        public AdministracionController()
        {
            _adminBusiness = new AdministracionBusiness();
        }

        #region Metodos Administrar Usuarios


        public async Task<IHttpActionResult> ModificarUsuario(Usuarios usuarioParamodificar)
        {
            if (usuarioParamodificar == null || usuarioParamodificar.PlanesUsuarios == null || usuarioParamodificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(usuarioParamodificar.Usuario) || string.IsNullOrWhiteSpace(usuarioParamodificar.Email)
                || usuarioParamodificar.PlanesUsuarios.Consecutivo <= 0 || usuarioParamodificar.PlanesUsuarios.CodigoPlanDeseado <= 0)
            {
                return BadRequest("usuarioParamodificar vacio y/o invalido!.");
            }

            try
            {
                Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones> tupleWrapper = await _adminBusiness.ModificarUsuario(usuarioParamodificar);

                if (tupleWrapper.Item1.Exitoso && tupleWrapper.Item2 != null)
                {
                    NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

                    IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hubContext.Clients.Group(ChatHub._prefixChatGroupName + tupleWrapper.Item2.CodigoPersonaDestino.ToString()).receiveNotification(tupleWrapper.Item2);
                }

                return Ok(tupleWrapper.Item1);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarUsuario(Usuarios usuarioParaEliminar)
        {
            if (usuarioParaEliminar == null || usuarioParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("usuarioParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarUsuario = await _adminBusiness.EliminarUsuario(usuarioParaEliminar);

                return Ok(wrapperEliminarUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion


        #region Metodos TerminosCondiciones


        public async Task<IHttpActionResult> AsignarTerminosCondiciones(TerminosCondiciones terminosCondicionesParaAsignar)
        {
            if (terminosCondicionesParaAsignar == null || string.IsNullOrWhiteSpace(terminosCondicionesParaAsignar.Texto) || terminosCondicionesParaAsignar.IdiomaDeLosTerminos == Idioma.SinIdioma)
            {
                return BadRequest("terminosCondicionesParaAsignar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAsignarTerminosCondiciones = await _adminBusiness.AsignarTerminosCondiciones(terminosCondicionesParaAsignar);

                return Ok(wrapperAsignarTerminosCondiciones);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> AsignarTerminosCondicionesLista(List<TerminosCondiciones> terminosCondicionesParaAsignar)
        {
            if (terminosCondicionesParaAsignar == null || terminosCondicionesParaAsignar.Count <= 0 || !terminosCondicionesParaAsignar.TrueForAll(x => !string.IsNullOrWhiteSpace(x.Texto) && x.IdiomaDeLosTerminos != Idioma.SinIdioma))
            {
                return BadRequest("terminosCondicionesParaAsignar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAsignarTerminosCondicionesLista = await _adminBusiness.AsignarTerminosCondicionesLista(terminosCondicionesParaAsignar);

                return Ok(wrapperAsignarTerminosCondicionesLista);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarTerminosCondiciones(TerminosCondiciones terminosCondicionesParaBuscar)
        {
            if (terminosCondicionesParaBuscar == null || terminosCondicionesParaBuscar.IdiomaDeLosTerminos == Idioma.SinIdioma)
            {
                return BadRequest("terminosCondicionesParaBuscar vacio y/o invalido!.");
            }

            try
            {
                TerminosCondiciones terminosCondicionesBuscados = await _adminBusiness.BuscarTerminosCondiciones(terminosCondicionesParaBuscar);

                return Ok(terminosCondicionesBuscados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarTerminosCondiciones()
        {
            try
            {
                List<TerminosCondiciones> terminosCondicionesBuscados = await _adminBusiness.ListarTerminosCondiciones();

                return Ok(terminosCondicionesBuscados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos ImagenesPerfilAdministradores


        [Route("api/Administracion/AsignarImagenPerfilAdministrador/{codigoUsuario}")]
        public async Task<IHttpActionResult> AsignarImagenPerfilAdministrador(int codigoUsuario)
        {
            if (codigoUsuario <= 0)
            {
                return BadRequest("codigoUsuario esta vacio o invalido");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    WrapperSimpleTypesDTO wrapperAsignarImagenPerfilAdministrador = await _adminBusiness.AsignarImagenPerfilAdministrador(codigoUsuario, sourceStream);

                    return Ok(wrapperAsignarImagenPerfilAdministrador);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos Paises


        public async Task<IHttpActionResult> CrearPais(Paises paisParaCrear)
        {
            if (paisParaCrear == null || paisParaCrear.CodigoMoneda <= 0 || paisParaCrear.CodigoIdioma <= 0 || paisParaCrear.PaisesContenidos == null || paisParaCrear.PaisesContenidos.Count <= 0
                || !paisParaCrear.PaisesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0)
                || paisParaCrear.Archivos == null || paisParaCrear.Archivos.ArchivoContenido == null)
            {
                return BadRequest("paisParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearPais = await _adminBusiness.CrearPais(paisParaCrear);

                return Ok(wrapperCrearPais);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarPais(Paises paisParaBuscar)
        {
            if (paisParaBuscar == null || paisParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("paisParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Paises paisBuscada = await _adminBusiness.BuscarPais(paisParaBuscar);

                return Ok(paisBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarPaisesPorIdioma(Paises paisParaListar)
        {
            if (paisParaListar == null || paisParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("paisParaListar vacio y/o invalido!.");
            }

            try
            {
                List<PaisesDTO> listaPaises = await _adminBusiness.ListarPaisesPorIdioma(paisParaListar);

                return Ok(listaPaises);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarArchivoPais(Paises paisParaModificar)
        {
            if (paisParaModificar == null || paisParaModificar.CodigoArchivo <= 0 || paisParaModificar.ArchivoContenido == null)
            {
                return BadRequest("paisParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarArchivoPais = await _adminBusiness.ModificarArchivoPais(paisParaModificar);

                return Ok(wrapperModificarArchivoPais);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarPais(Paises paisParaModificar)
        {
            if (paisParaModificar == null || paisParaModificar.Consecutivo <= 0 || paisParaModificar.CodigoIdioma <= 0 || paisParaModificar.CodigoMoneda <= 0
                || paisParaModificar.PaisesContenidos == null || paisParaModificar.PaisesContenidos.Count <= 0
                || !paisParaModificar.PaisesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.Consecutivo > 0))
            {
                return BadRequest("paisParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarPais = await _adminBusiness.ModificarPais(paisParaModificar);

                return Ok(wrapperModificarPais);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        public async Task<IHttpActionResult> EliminarPais(Paises paisParaEliminar)
        {
            if (paisParaEliminar == null || paisParaEliminar.Consecutivo <= 0 || paisParaEliminar.CodigoArchivo <= 0)
            {
                return BadRequest("paisParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarPais = await _adminBusiness.EliminarPais(paisParaEliminar);

                return Ok(wrapperEliminarPais);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos PaisesContenidos


        public async Task<IHttpActionResult> CrearPaisesContenidos(List<PaisesContenidos> paisesContenidosParaCrear)
        {
            if (paisesContenidosParaCrear == null || paisesContenidosParaCrear.Count <= 0 ||
                !paisesContenidosParaCrear.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.CodigoPais > 0))
            {
                return BadRequest("categoriaContenidoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearPaisesContenidos = await _adminBusiness.CrearPaisesContenidos(paisesContenidosParaCrear);

                return Ok(wrapperCrearPaisesContenidos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarPaisContenido(PaisesContenidos paisContenidoParaBuscar)
        {
            if (paisContenidoParaBuscar == null || paisContenidoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("paisContenidoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                PaisesContenidos paisContenidoBuscada = await _adminBusiness.BuscarPaisContenido(paisContenidoParaBuscar);

                return Ok(paisContenidoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarContenidoDeUnPais(PaisesContenidos paisContenidoParaListar)
        {
            if (paisContenidoParaListar == null || paisContenidoParaListar.CodigoPais <= 0)
            {
                return BadRequest("paisContenidoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<PaisesContenidos> listaPaisesContenido = await _adminBusiness.ListarContenidoDeUnPais(paisContenidoParaListar);

                return Ok(listaPaisesContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarPaisContenido(PaisesContenidos paisContenidoParaModificar)
        {
            if (paisContenidoParaModificar == null || paisContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(paisContenidoParaModificar.Descripcion))
            {
                return BadRequest("paisContenidoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarPaisContenido = await _adminBusiness.ModificarPaisContenido(paisContenidoParaModificar);

                return Ok(wrapperModificarPaisContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarPaisContenido(PaisesContenidos paisContenidoParaEliminar)
        {
            if (paisContenidoParaEliminar == null || paisContenidoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("paisContenidoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarPaisContenido = await _adminBusiness.EliminarPaisContenido(paisContenidoParaEliminar);

                return Ok(wrapperEliminarPaisContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
