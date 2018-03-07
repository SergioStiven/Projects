using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class AuthenticateController : ApiController
    {
        AuthenticateBusiness _authenticateBusiness;

        public AuthenticateController()
        {
            _authenticateBusiness = new AuthenticateBusiness();
        }

        public async Task<IHttpActionResult> VerificarUsuario(Usuarios usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null || string.IsNullOrWhiteSpace(usuarioParaVerificar.Usuario) || string.IsNullOrWhiteSpace(usuarioParaVerificar.Clave))
            {
                return BadRequest("usuarioParaVerificar vacio y/o invalido!.");
            }

            try
            {
                UsuariosDTO usuarioVerificado = await _authenticateBusiness.VerificarUsuario(usuarioParaVerificar);

                return Ok(usuarioVerificado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarUsuarioConDeviceID(Usuarios usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null || string.IsNullOrWhiteSpace(usuarioParaVerificar.DeviceId))
            {
                return BadRequest("usuarioParaVerificar vacio y/o invalido!.");
            }
            try
            {
                UsuariosDTO usuarioExistente = await _authenticateBusiness.VerificarUsuarioConDeviceID(usuarioParaVerificar);

                return Ok(usuarioExistente);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarUsuarioConEmailUsuarioYDeviceId(Usuarios usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null || string.IsNullOrWhiteSpace(usuarioParaVerificar.DeviceId) || string.IsNullOrWhiteSpace(usuarioParaVerificar.Email)
                || string.IsNullOrWhiteSpace(usuarioParaVerificar.Usuario))
            {
                return BadRequest("usuarioParaVerificar vacio y/o invalido!.");
            }
            try
            {
                UsuariosDTO usuarioExistente = await _authenticateBusiness.VerificarUsuarioConEmailUsuarioYDeviceId(usuarioParaVerificar);

                return Ok(usuarioExistente);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarUsuario(Usuarios usuarioParaModificar)
        {
            if (usuarioParaModificar == null || usuarioParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(usuarioParaModificar.Usuario)
                || string.IsNullOrWhiteSpace(usuarioParaModificar.Clave) || string.IsNullOrWhiteSpace(usuarioParaModificar.Email))
            {
                return BadRequest("usuarioParaModificar vacio y/o invalido!.");
            }
            try
            {
                WrapperSimpleTypesDTO wrapperModificarUsuario = await _authenticateBusiness.ModificarUsuario(usuarioParaModificar);

                return Ok(wrapperModificarUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ActivarUsuario(Usuarios usuarioParaActivar)
        {
            if (usuarioParaActivar == null || usuarioParaActivar.Consecutivo <= 0)
            {
                return BadRequest("usuarioParaActivar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperActivarUsuario = await _authenticateBusiness.ActivarUsuario(usuarioParaActivar);

                return Ok(wrapperActivarUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BloquearUsuario(Usuarios usuarioParaBloquear)
        {
            if (usuarioParaBloquear == null || usuarioParaBloquear.Consecutivo <= 0)
            {
                return BadRequest("usuarioParaBloquear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperBloquearUsuario = await _authenticateBusiness.BloquearUsuario(usuarioParaBloquear);

                return Ok(wrapperBloquearUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarDeviceId(Usuarios usuarioParaModificar)
        {
            if (usuarioParaModificar == null || usuarioParaModificar.Consecutivo <= 0)
            {
                return BadRequest("usuarioParaModificar vacio y/o invalido!.");
            }
            try
            {
                WrapperSimpleTypesDTO wrapperModificarUsuario = await _authenticateBusiness.ModificarDeviceId(usuarioParaModificar);

                return Ok(wrapperModificarUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarEmailUsuario(Usuarios usuarioParaModificar)
        {
            if (usuarioParaModificar == null || usuarioParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(usuarioParaModificar.Email))
            {
                return BadRequest("usuarioParaModificar vacio y/o invalido!.");
            }
            try
            {
                WrapperSimpleTypesDTO wrapperModificarEmailUsuario = await _authenticateBusiness.ModificarEmailUsuario(usuarioParaModificar);

                return Ok(wrapperModificarEmailUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        public async Task<IHttpActionResult> RecuperarClave(Usuarios usuarioParaRecuperar)
        {
            if (usuarioParaRecuperar == null || string.IsNullOrWhiteSpace(usuarioParaRecuperar.Usuario) || string.IsNullOrWhiteSpace(usuarioParaRecuperar.Email))
            {
                return BadRequest("usuarioParaRecuperar vacio y/o invalido!.");
            }

            try
            {
                string urlLogo = Url.Content("~/Content/Images/LogoSportsGo.png");
                string urlBanner = Url.Content("~/Content/Images/BannerSportsGo.png");

                WrapperSimpleTypesDTO wrapperRecuperarClave = await _authenticateBusiness.RecuperarClave(usuarioParaRecuperar, urlLogo, urlBanner);

                return Ok(wrapperRecuperarClave);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarSiUsuarioYaExiste(Usuarios usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null || string.IsNullOrWhiteSpace(usuarioParaVerificar.Usuario))
            {
                return BadRequest("usuarioParaVerificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperExisteUsuario = await _authenticateBusiness.VerificarSiUsuarioYaExiste(usuarioParaVerificar);

                return Ok(wrapperExisteUsuario);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarSiEmailYaExiste(Usuarios emailParaVerificar)
        {
            if (emailParaVerificar == null || string.IsNullOrWhiteSpace(emailParaVerificar.Email))
            {
                return BadRequest("emailParaVerificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEmailExiste = await _authenticateBusiness.VerificarSiEmailYaExiste(emailParaVerificar);

                return Ok(wrapperEmailExiste);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarSiCuentaEstaActiva(Usuarios usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null || string.IsNullOrWhiteSpace(usuarioParaVerificar.Usuario))
            {
                return BadRequest("usuarioParaVerificarvacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperVerificarCuentaActiva = await _authenticateBusiness.VerificarSiCuentaEstaActiva(usuarioParaVerificar);

                return Ok(wrapperVerificarCuentaActiva);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarTiposPerfiles()
        {
            try
            {
                List<TiposPerfiles> listaTiposPerfiles = await _authenticateBusiness.ListarTiposPerfiles();

                return Ok(listaTiposPerfiles);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}