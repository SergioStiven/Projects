using FreshMvvm;
using Plugin.Fingerprint;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Controls;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ConfiguracionUsuarioPageModel : BasePageModel
    {
        AuthenticateServices _authService;
        PersonasServices _personaService;

        UsuariosDTO _usuario;
        string _viejoUsuario;
        string _viejoEmail;

        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string VerificacionClave { get; set; }
        public string Email { get; set; }
        public bool AsociaDeviceId { get; set; }
        public bool AgregarDeviceId { get; set; }

        public ConfiguracionUsuarioPageModel()
        {
            _authService = new AuthenticateServices();
            _personaService = new PersonasServices();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            _usuario = initData as UsuariosDTO;
            _viejoEmail = _usuario.Email;
            _viejoUsuario = _usuario.Usuario;

            if (!_usuario.EsRecuperarClave)
            {
                Clave = _usuario.Clave;
                VerificacionClave = _usuario.Clave;
            }

            Usuario = _usuario.Usuario;
            Email = _usuario.Email;

            if (!string.IsNullOrWhiteSpace(_usuario.DeviceId))
            {
                AgregarDeviceId = _usuario.DeviceId.Trim() == App.DeviceId.Trim();
            }

            //AsociaDeviceId = await CrossFingerprint.Current.IsAvailableAsync();
            AsociaDeviceId = false;
        }

        public ICommand GuardarUsuario
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (!await ValidarDatosCuenta())
                    {
                        return;
                    }

                    try
                    {
                        UsuariosDTO usuarioParaModificar = new UsuariosDTO
                        {
                            Consecutivo = _usuario.Consecutivo,
                            Usuario = Usuario,
                            Clave = Clave,
                            Email = Email
                        };

                        if (_viejoUsuario.Trim() != Usuario.Trim())
                        {
                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapperUsuario = await _authService.VerificarSiUsuarioYaExiste(usuarioParaModificar);

                            if (wrapperUsuario == null)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorValidarUsuario, "OK");
                                tcs.SetResult(true);
                                return;
                            }
                            else if (wrapperUsuario.Existe)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.UsuarioRepetido, "OK");
                                tcs.SetResult(true);
                                return;
                            }
                        }

                        if (_viejoEmail.Trim() != Email.Trim())
                        {
                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapperEmail = await _authService.VerificarSiEmailYaExiste(usuarioParaModificar);

                            if (wrapperEmail == null)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorValidarEmail, "OK");
                                tcs.SetResult(true);
                                return;
                            }
                            else if (wrapperEmail.Existe)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.EmailYaExiste, "OK");
                                tcs.SetResult(true);
                                return;
                            }
                        }

                        if (IsNotConnected)
                        {
                            tcs.SetResult(true);
                            return;
                        }
                        WrapperSimpleTypesDTO wrapperModificarUsuario = await _authService.ModificarUsuario(usuarioParaModificar);

                        if (AsociaDeviceId)
                        {
                            if (AgregarDeviceId)
                            {
                                usuarioParaModificar.DeviceId = App.DeviceId;
                            }
                            else
                            {
                                usuarioParaModificar.DeviceId = string.Empty;
                            }

                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapperAsociaDeviceId = await _authService.ModificarDeviceId(usuarioParaModificar);

                            if (wrapperAsociaDeviceId != null && wrapperAsociaDeviceId.Exitoso)
                            {
                                App.Usuario.DeviceId = App.DeviceId;
                            }
                        }

                        if (wrapperModificarUsuario == null)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorModificarUsuario, "OK");
                        }
                        else
                        {
                            if (_usuario.EsRecuperarClave)
                            {
                                if (IsNotConnected)
                                {
                                    tcs.SetResult(true);
                                    return;
                                }
                                PersonasDTO personaBuscada = await _personaService.BuscarPersona(_usuario.PersonaDelUsuario);

                                if (personaBuscada != null)
                                {
                                    _usuario.PersonaDelUsuario = personaBuscada;
                                    _usuario.Usuario = Usuario;
                                    _usuario.Clave = Clave;
                                    _usuario.Email = Email;

                                    App.Usuario = _usuario;
                                    App.Persona = personaBuscada;
                                    App.Persona.Usuarios = _usuario;
                                    App.IdiomaPersona = personaBuscada.IdiomaDeLaPersona;
                                    App.ConfigureCultureIdiomsApp(App.Persona.IdiomaDeLaPersona);

                                    BadgeColorTabbedNavigationContainer tabbedPage = App.ConfigureTabbedNavigationContainer(App.Persona, App.Usuario);
                                    CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainTabbedContainer);
                                    tabbedPage.CurrentPage = tabbedPage.Children[2];
                                    await App.ConnectPersonToChatHub();
                                }
                                else
                                {
                                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorIniciarSesion, "OK");
                                }
                            }
                            else
                            {
                                _usuario.Usuario = Usuario;
                                _usuario.Clave = Clave;
                                _usuario.Email = Email;

                                App.Usuario = _usuario;
                                App.Persona.Usuarios = App.Usuario;

                                await CoreMethods.PopPageModel();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorModificarCuenta, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task<bool> ValidarDatosCuenta()
        {
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Clave) || string.IsNullOrWhiteSpace(Email))
            {
                esValido = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltanDatosCuenta, "OK");
            }
            else if (string.IsNullOrWhiteSpace(VerificacionClave) || Clave.Trim() != VerificacionClave.Trim())
            {
                esValido = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ContraseñasDiferentes, "OK");
            }
            else if (!Regex.IsMatch(Email, AppConstants.RegexEmail))
            {
                esValido = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.EmailFormatoInvalido, "OK");
            }

            return esValido;
        }
    }
}
