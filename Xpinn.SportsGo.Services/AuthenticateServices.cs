using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Services
{
    public class AuthenticateServices : BaseService
    {
        public async Task<UsuariosDTO> VerificarUsuario(UsuariosDTO usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null) throw new ArgumentNullException("No puedes verificar un usuario si usuarioParaVerificar es nulo!.");
            if (string.IsNullOrWhiteSpace(usuarioParaVerificar.Usuario) || string.IsNullOrWhiteSpace(usuarioParaVerificar.Clave))
            {
                throw new ArgumentException("usuarioParaVerificar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            UsuariosDTO usuarioVerificado = await client.PostAsync("Authenticate/VerificarUsuario", usuarioParaVerificar);

            return usuarioVerificado;
        }

        public async Task<UsuariosDTO> VerificarUsuarioConDeviceID(UsuariosDTO usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null) throw new ArgumentNullException("No puedes verificar un usuario si usuarioParaVerificar es nulo!.");
            if (string.IsNullOrWhiteSpace(usuarioParaVerificar.DeviceId))
            {
                throw new ArgumentException("usuarioParaVerificar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            UsuariosDTO usuarioVerificado = await client.PostAsync("Authenticate/VerificarUsuarioConDeviceID", usuarioParaVerificar);

            return usuarioVerificado;
        }

        public async Task<UsuariosDTO> VerificarUsuarioConEmailUsuarioYDeviceId(UsuariosDTO usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null) throw new ArgumentNullException("No puedes verificar un usuario si usuarioParaVerificar es nulo!.");
            if (string.IsNullOrWhiteSpace(usuarioParaVerificar.DeviceId) || string.IsNullOrWhiteSpace(usuarioParaVerificar.Email)
                || string.IsNullOrWhiteSpace(usuarioParaVerificar.Usuario))
            {
                throw new ArgumentException("usuarioParaVerificar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            UsuariosDTO usuarioVerificado = await client.PostAsync("Authenticate/VerificarUsuarioConEmailUsuarioYDeviceId", usuarioParaVerificar);

            return usuarioVerificado;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarUsuario(UsuariosDTO usuarioParaModificar)
        {
            if (usuarioParaModificar == null) throw new ArgumentNullException("No puedes modificar un usuario si usuarioParaModificar es nulo!.");
            if (usuarioParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(usuarioParaModificar.Usuario) 
                || string.IsNullOrWhiteSpace(usuarioParaModificar.Clave) || string.IsNullOrWhiteSpace(usuarioParaModificar.Email))
            {
                throw new ArgumentException("usuarioParaModificar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarUsuario = await client.PostAsync<UsuariosDTO,WrapperSimpleTypesDTO>("Authenticate/ModificarUsuario", usuarioParaModificar);

            return wrapperModificarUsuario;
        }

        public async Task<WrapperSimpleTypesDTO> ActivarUsuario(UsuariosDTO usuarioParaActivar)
        {
            if (usuarioParaActivar == null) throw new ArgumentNullException("No puedes activar un usuario si usuarioParaActivar es nulo!.");
            if (usuarioParaActivar.Consecutivo <= 0)
            {
                throw new ArgumentException("usuarioParaActivar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperActivarUsuario = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/ActivarUsuario", usuarioParaActivar);

            return wrapperActivarUsuario;
        }

        public async Task<WrapperSimpleTypesDTO> BloquearUsuario(UsuariosDTO usuarioParaBloquear)
        {
            if (usuarioParaBloquear == null) throw new ArgumentNullException("No puedes bloquear un usuario si usuarioParaBloquear es nulo!.");
            if (usuarioParaBloquear.Consecutivo <= 0)
            {
                throw new ArgumentException("usuarioParaBloquear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperBloquearUsuario = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/BloquearUsuario", usuarioParaBloquear);

            return wrapperBloquearUsuario;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarDeviceId(UsuariosDTO usuarioParaModificar)
        {
            if (usuarioParaModificar == null) throw new ArgumentNullException("No puedes modificar el deviceID un usuario si usuarioParaModificar es nulo!.");
            if (usuarioParaModificar.Consecutivo <= 0)
            {
                throw new ArgumentException("usuarioParaModificar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarDeviceId = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/ModificarDeviceId", usuarioParaModificar);

            return wrapperModificarDeviceId;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarEmailUsuario(UsuariosDTO usuarioParaModificar)
        {
            if (usuarioParaModificar == null) throw new ArgumentNullException("No puedes modificar un usuario si usuarioParaModificar es nulo!.");
            if (usuarioParaModificar.Consecutivo <= 0  || string.IsNullOrWhiteSpace(usuarioParaModificar.Email))
            {
                throw new ArgumentException("usuarioParaModificar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarEmailUsuario = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/ModificarEmailUsuario", usuarioParaModificar);

            return wrapperModificarEmailUsuario;
        }

        public async Task<WrapperSimpleTypesDTO> RecuperarClave(UsuariosDTO usuarioParaRecuperar)
        {
            if (usuarioParaRecuperar == null) throw new ArgumentNullException("No puedes recuperar una clave si usuarioParaRecuperar es nulo!.");
            if (string.IsNullOrWhiteSpace(usuarioParaRecuperar.Usuario) || string.IsNullOrWhiteSpace(usuarioParaRecuperar.Email))
            {
                throw new ArgumentException("usuarioParaRecuperar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperRecuperarClave = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/RecuperarClave", usuarioParaRecuperar);

            return wrapperRecuperarClave;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiUsuarioYaExiste(UsuariosDTO usuarioParaRevisar)
        {
            if (usuarioParaRevisar == null) throw new ArgumentNullException("No puedes revisar un usuario si usuarioParaRevisar es nulo!.");
            if (string.IsNullOrWhiteSpace(usuarioParaRevisar.Usuario)) throw new ArgumentException("usuarioParaRevisar vacia y/o invalida!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperUsuarioExiste = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/VerificarSiUsuarioYaExiste", usuarioParaRevisar);

            return wrapperUsuarioExiste;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiEmailYaExiste(UsuariosDTO emailParaRevisar)
        {
            if (emailParaRevisar == null) throw new ArgumentNullException("No puedes revisar un email si emailParaRevisar es nulo!.");
            if (string.IsNullOrWhiteSpace(emailParaRevisar.Email))  throw new ArgumentException("emailParaRevisar vacia y/o invalida!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEmailExiste = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/VerificarSiEmailYaExiste", emailParaRevisar);

            return wrapperEmailExiste;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiCuentaEstaActiva(UsuariosDTO usuarioParaVerificar)
        {
            if (usuarioParaVerificar == null) throw new ArgumentNullException("No puedes revisar si una cuenta esta activa si usuarioParaVerificar es nulo!.");
            if (string.IsNullOrWhiteSpace(usuarioParaVerificar.Usuario)) throw new ArgumentException("usuarioParaVerificar vacia y/o invalida!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperVerificarCuentaActiva = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Authenticate/VerificarSiCuentaEstaActiva", usuarioParaVerificar);

            return wrapperVerificarCuentaActiva;
        }

        public async Task<List<TiposPerfilesDTO>> ListarTiposPerfiles()
        {
            IHttpClient client = ConfigurarHttpClient();

            List<TiposPerfilesDTO> listaPerfiles = await client.PostAsync<List<TiposPerfilesDTO>>("Authenticate/ListarTiposPerfiles");

            return listaPerfiles;
        }
    }
}