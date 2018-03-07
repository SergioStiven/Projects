using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Web.Security;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Business
{
    public class AuthenticateBusiness
    {
        public async Task<UsuariosDTO> VerificarUsuario(Usuarios usuarioParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                UsuariosDTO usuarioVerificado = await authenticateRepo.VerificarUsuario(usuarioParaVerificar);

                if (usuarioVerificado != null)
                {
                    Usuarios usuarioExistente = await authenticateRepo.ActualizarFechaUltimoAcceso(usuarioVerificado.Consecutivo);

                    // Se vencio el plan
                    if (usuarioVerificado.PlanesUsuarios.Vencimiento < DateTime.Now && usuarioVerificado.TipoPerfil != TipoPerfil.Administrador)
                    {
                        PlanesBusiness planBusiness = new PlanesBusiness();
                        WrapperSimpleTypesDTO wrapperCambiarPlan = await planBusiness.CambiarPlanUsuarioADefaultPerfilPorVencimiento(usuarioVerificado.PlanesUsuarios);
                        usuarioVerificado = await authenticateRepo.VerificarUsuario(usuarioParaVerificar);
                    }

                    await context.SaveChangesAsync();

                    if (usuarioVerificado.TipoPerfil == TipoPerfil.Administrador)
                    {
                        AdministracionRepository adminRepo = new AdministracionRepository(context);

                        ImagenesPerfilAdministradores imagenPerfilBuscada = await adminRepo.BuscarImagenPerfilAdministrador(usuarioVerificado.Consecutivo);

                        if (imagenPerfilBuscada != null)
                        {
                            usuarioVerificado.CodigoImagenPerfilAdmin = imagenPerfilBuscada.CodigoArchivo;
                        }
                    }
                }

                return usuarioVerificado;
            }
        }

        public async Task<UsuariosDTO> VerificarUsuarioConDeviceID(Usuarios usuarioParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                UsuariosDTO usuarioExistente = await authenticateRepo.VerificarUsuarioConDeviceID(usuarioParaVerificar);

                if (usuarioExistente != null)
                {
                    Usuarios usuarioModificado = await authenticateRepo.ActualizarFechaUltimoAcceso(usuarioExistente.Consecutivo);
                    await context.SaveChangesAsync();
                }

                return usuarioExistente;
            }
        }

        public async Task<UsuariosDTO> VerificarUsuarioConEmailUsuarioYDeviceId(Usuarios usuarioParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                UsuariosDTO usuarioExistente = await authenticateRepo.VerificarUsuarioConEmailUsuarioYDeviceId(usuarioParaVerificar);

                if (usuarioExistente != null)
                {
                    Usuarios usuarioModificado = await authenticateRepo.ActualizarFechaUltimoAcceso(usuarioExistente.Consecutivo);
                    await context.SaveChangesAsync();
                }

                return usuarioExistente;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarUsuario(Usuarios usuarioParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                Usuarios usuarioExistente = await authenticateRepo.ModificarUsuario(usuarioParaModificar);

                WrapperSimpleTypesDTO wrapperModificarUsuario = new WrapperSimpleTypesDTO();

                wrapperModificarUsuario.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarUsuario.NumeroRegistrosAfectados > 0) wrapperModificarUsuario.Exitoso = true;

                return wrapperModificarUsuario;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ActivarUsuario(Usuarios usuarioParaActivar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                Usuarios usuarioExistente = await authenticateRepo.ActivarUsuario(usuarioParaActivar);

                WrapperSimpleTypesDTO wrapperActivarUsuario = new WrapperSimpleTypesDTO();

                wrapperActivarUsuario.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperActivarUsuario.NumeroRegistrosAfectados > 0) wrapperActivarUsuario.Exitoso = true;

                return wrapperActivarUsuario;
            }
        }

        public async Task<WrapperSimpleTypesDTO> BloquearUsuario(Usuarios usuarioParaBloquear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                Usuarios usuarioExistente = await authenticateRepo.BloquearUsuario(usuarioParaBloquear);

                WrapperSimpleTypesDTO wrapperBloquearUsuario = new WrapperSimpleTypesDTO();

                wrapperBloquearUsuario.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperBloquearUsuario.NumeroRegistrosAfectados > 0) wrapperBloquearUsuario.Exitoso = true;

                return wrapperBloquearUsuario;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarDeviceId(Usuarios usuarioParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);

                await authenticateRepo.EliminarDeviceIdSimilares(usuarioParaModificar);

                Usuarios usuarioExistente = await authenticateRepo.ModificarDeviceId(usuarioParaModificar);

                WrapperSimpleTypesDTO wrapperModificarDeviceId = new WrapperSimpleTypesDTO();

                wrapperModificarDeviceId.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarDeviceId.NumeroRegistrosAfectados > 0) wrapperModificarDeviceId.Exitoso = true;

                return wrapperModificarDeviceId;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarEmailUsuario(Usuarios usuarioParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                Usuarios usuarioExistente = await authenticateRepo.ModificarEmailUsuario(usuarioParaModificar);

                WrapperSimpleTypesDTO wrapperModificarEmailUsuario = new WrapperSimpleTypesDTO();

                wrapperModificarEmailUsuario.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarEmailUsuario.NumeroRegistrosAfectados > 0) wrapperModificarEmailUsuario.Exitoso = true;

                return wrapperModificarEmailUsuario;
            }
        }

        public async Task<WrapperSimpleTypesDTO> RecuperarClave(Usuarios usuarioParaRecuperar, string urlLogo, string urlBanner)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                string nuevaClave = string.Empty;
                bool exitoso = false;

                UsuariosDTO usuarioExistente = await authenticateRepo.VerificarSiUsuarioExisteYPertenecesAlEmail(usuarioParaRecuperar);

                if (usuarioExistente != null)
                {
                    string formatoEmail = await authenticateRepo.BuscarFormatoCorreoPorCodigoIdioma(usuarioExistente.PersonaDelUsuario.CodigoIdioma, TipoFormatosEnum.RecuperacionClave);

                    if (!string.IsNullOrWhiteSpace(formatoEmail))
                    {
                        nuevaClave = Membership.GeneratePassword(10, 2);
                        Usuarios usuarioModificado = await authenticateRepo.ModificarClave(usuarioExistente.Consecutivo, nuevaClave);
                        usuarioModificado.CuentaActiva = (int)SiNoEnum.Si;

                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderNombre, usuarioExistente.PersonaDelUsuario.NombreYApellido);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUsuario, usuarioExistente.Usuario);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderClave, nuevaClave);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenLogo, urlLogo);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenBanner, urlBanner);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlWeb, URL.UrlWeb);

                        string tema = string.Empty;
                        switch (usuarioExistente.PersonaDelUsuario.IdiomaDeLaPersona)
                        {
                            case Idioma.Español:
                                tema = "Recuperacion Contraseña";
                                break;
                            case Idioma.Ingles:
                                tema = "Password Recovery";
                                break;
                            case Idioma.Portugues:
                                tema = "Recuperação de Senha";
                                break;
                        }

                        // Recordar configurar la cuenta Gmail en este caso para que permita el logeo de manera insegura y poder mandar correos
                        // https://myaccount.google.com/lesssecureapps?pli=1
                        CorreoHelper correoHelper = new CorreoHelper(usuarioExistente.Email.Trim(), AppConstants.CorreoAplicacion, AppConstants.ClaveCorreoAplicacion);
                        exitoso = correoHelper.EnviarCorreoConHTML(formatoEmail, Correo.Gmail, tema, "SportsGo");
                    }
                    else
                    {
                        throw new InvalidOperationException("No hay formatos parametrizados para recuperar la clave");
                    }
                }

                WrapperSimpleTypesDTO wrapperRecuperarClave = new WrapperSimpleTypesDTO();

                if (exitoso)
                {
                    wrapperRecuperarClave.NumeroRegistrosAfectados = await context.SaveChangesAsync();
                }

                if (wrapperRecuperarClave.NumeroRegistrosAfectados > 0 && exitoso)
                {
                    wrapperRecuperarClave.ClaveCreada = nuevaClave;
                    wrapperRecuperarClave.Exitoso = true;
                }

                return wrapperRecuperarClave;
            }
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiCuentaEstaActiva(Usuarios usuarioParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                WrapperSimpleTypesDTO wrapperVerificarCuentaActiva = await authenticateRepo.VerificarSiCuentaEstaActiva(usuarioParaVerificar);

                return wrapperVerificarCuentaActiva;
            }
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiUsuarioYaExiste(Usuarios usuarioParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                WrapperSimpleTypesDTO wrapperExisteUsuario = await authenticateRepo.VerificarSiUsuarioYaExiste(usuarioParaVerificar);

                return wrapperExisteUsuario;
            }
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiEmailYaExiste(Usuarios emailParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                WrapperSimpleTypesDTO wrapperExisteEmail = await authenticateRepo.VerificarSiEmailYaExiste(emailParaVerificar);

                return wrapperExisteEmail;
            }
        }

        public async Task<List<TiposPerfiles>> ListarTiposPerfiles()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                List<TiposPerfiles> listaTiposPerfiles = await authenticateRepo.ListarTiposPerfiles();

                return listaTiposPerfiles;
            }
        }
    }
}
