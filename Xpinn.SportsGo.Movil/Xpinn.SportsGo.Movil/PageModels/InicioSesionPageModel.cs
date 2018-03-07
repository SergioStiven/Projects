using Acr.UserDialogs;
using FreshMvvm;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Controls;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class InicioSesionPageModel : BasePageModel
    {
        PlanesServices _planesServices;
        AuthenticateServices _authService;

        TipoCuentaPerfil _tipoCuentaPerfil;

        public bool Recuerdame { get; set; }
        public bool NoEsPrimerRegistro { get { return _tipoCuentaPerfil == null; } }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string VerificacionClave { get; set; }
        public bool AceptaTerminosCondiciones { get; set; }

        public string Titulo
        {
            get
            {
                string titulo = SportsGoResources.Registrate;

                if (NoEsPrimerRegistro)
                {
                    titulo = SportsGoResources.IniciaSesion;
                }

                return titulo;
            }
        }

        public InicioSesionPageModel()
        {
            _authService = new AuthenticateServices();
            _planesServices = new PlanesServices();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            _tipoCuentaPerfil = initData as TipoCuentaPerfil;

            //if (NoEsPrimerRegistro && await CrossFingerprint.Current.IsAvailableAsync())
            //{
            //    FingerprintAuthenticationResult result = await AuthenticateAsync();

            //    if (result.Authenticated)
            //    {
            //        var config = new ProgressDialogConfig()
            //        .SetTitle(SportsGoResources.Cargando)
            //        .SetIsDeterministic(false);

            //        if (Device.RuntimePlatform == Device.iOS)
            //        {
            //            config.SetMaskType(MaskType.Black);
            //        }
            //        else
            //        {
            //            config.SetMaskType(MaskType.Gradient);
            //        }

            //        using (Dialogs.Progress(config))
            //        {
            //            UsuariosDTO usuarioParaBuscar = new UsuariosDTO
            //            {
            //                DeviceId = App.DeviceId
            //            };

            //            if (IsNotConnected) return;
            //            UsuariosDTO usuarioVerificado = await _authService.VerificarUsuarioConDeviceID(usuarioParaBuscar);

            //            if (usuarioVerificado != null && usuarioVerificado.Consecutivo != 0)
            //            {
            //                if (usuarioVerificado.CuentaActiva == 0)
            //                {
            //                    await CoreMethods.DisplayAlert(SportsGoResources.TituloAlerta, SportsGoResources.CuentaNoActiva, "OK");
            //                    return;
            //                }

            //                await ConfigurarTabPrincipal(usuarioVerificado);
            //            }
            //            else
            //            {
            //                await CoreMethods.DisplayAlert(SportsGoResources.TituloAlerta, SportsGoResources.NoSePudoValidarHuella, "OK");
            //            }
            //        }
            //    }
            //    else if (result.Status == FingerprintAuthenticationResultStatus.TooManyAttempts)
            //    {
            //        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DemasiadosIntentos, "OK");
            //    }
            //    else if(result.Status != FingerprintAuthenticationResultStatus.Canceled)
            //    {
            //        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.NoSePudoValidarHuella, "OK");
            //    }
            //}
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        public ICommand IngresarCuenta
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await Ingresar();

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand VerTerminos
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<TerminosCondicionesPageModel>();
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IngresarFacebook
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<InicioSesionPageModel>();
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IngresarTwitter
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<InicioSesionPageModel>();
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand OlvidoContraseña
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<RecuperarClavePageModel>();
                    tcs.SetResult(true);
                });
            }
        }

        async Task Ingresar()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Usuario) && !string.IsNullOrWhiteSpace(Clave))
                {
                    UsuariosDTO usuario = new UsuariosDTO
                    {
                        Usuario = Usuario,
                        Clave = Clave,
                    };

                    if (!NoEsPrimerRegistro)
                    {
                        if (!AceptaTerminosCondiciones)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DebesAceptarTerminos, "OK");
                            return;
                        }
                        else if (string.IsNullOrWhiteSpace(VerificacionClave) || Clave.Trim() != VerificacionClave.Trim())
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ContraseñasDiferentes, "OK");
                            return;
                        }
                        else if (!Regex.IsMatch(Usuario, AppConstants.RegexUserName))
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.UsuarioFormatoInvalido, "OK");
                            return;
                        }
                        else if (!Regex.IsMatch(Clave, AppConstants.RegexPassword))
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ClaveFormatoInvalido, "OK");
                            return;
                        }

                        if (IsNotConnected) return;
                        await IniciarSesionUsuarioParaRegistrar(usuario);
                    }
                    else
                    {
                        var config = new ProgressDialogConfig()
                                            .SetTitle(SportsGoResources.Cargando)
                                            .SetIsDeterministic(false);

                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            config.SetMaskType(MaskType.Black);
                        }
                        else
                        {
                            config.SetMaskType(MaskType.Gradient);
                        }

                        using (IProgressDialog progress = Dialogs.Progress(config))
                        {
                            if (IsNotConnected)
                            {
                                progress.Hide();
                                return;
                            }
                            await IniciarSesionUsuarioRegistrado(usuario);

                            progress.Hide();
                        }
                    }
                }
            }
            catch (Exception)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorIniciarSesion, "OK");
            }
        }

        async Task IniciarSesionUsuarioParaRegistrar(UsuariosDTO usuario)
        {
            if (IsNotConnected) return;
            WrapperSimpleTypesDTO wrapperUsuario = await _authService.VerificarSiUsuarioYaExiste(usuario);

            if (!wrapperUsuario.Existe)
            {
                usuario.TipoPerfil = _tipoCuentaPerfil.TipoPerfil;
                PersonasDTO personaParaCrear = new PersonasDTO
                {
                    Usuarios = usuario,
                    TipoPerfil = _tipoCuentaPerfil.TipoPerfil,
                    IdiomaDeLaPersona = App.IdiomaPersona,
                    PersonaRecordandose = Recuerdame
                };

                PlanesDTO planParaBuscar = new PlanesDTO
                {
                    TipoPerfil = _tipoCuentaPerfil.TipoPerfil
                };

                PlanesDTO planDefaultParaElPerfil = await _planesServices.BuscarPlanDefaultDeUnPerfil(planParaBuscar);

                personaParaCrear.Usuarios.PlanesUsuarios = new PlanesUsuariosDTO
                {
                    Planes = planDefaultParaElPerfil
                };

                App.Usuario = null;
                App.Persona = null;

                await CoreMethods.PushPageModel<InformacionPerfilPageModel>(personaParaCrear);
            }
            else
            {
                await CoreMethods.DisplayAlert(SportsGoResources.TituloAlerta, SportsGoResources.UsuarioRepetido, "OK");
            }
        }

        async Task IniciarSesionUsuarioRegistrado(UsuariosDTO usuario)
        {
            if (IsNotConnected) return;
            UsuariosDTO usuarioVerificado = await _authService.VerificarUsuario(usuario);

            if (usuarioVerificado != null && (usuarioVerificado.TipoPerfil == TipoPerfil.Anunciante || usuarioVerificado.TipoPerfil == TipoPerfil.Administrador))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.TituloAlerta, SportsGoResources.UsuarioNoExiste, "OK");
                return;
            }

            if (usuarioVerificado != null && usuarioVerificado.Consecutivo != 0)
            {
                if (usuarioVerificado.CuentaActiva == 0)
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.TituloAlerta, SportsGoResources.CuentaNoActiva, "OK");
                    return;
                }

                await ConfigurarTabPrincipal(usuarioVerificado);
            }
            else
            {
                await CoreMethods.DisplayAlert(SportsGoResources.TituloAlerta, SportsGoResources.UsuarioNoExiste, "OK");
            }
        }

        private async Task ConfigurarTabPrincipal(UsuariosDTO usuarioVerificado)
        {
            PersonasServices personaSer = new PersonasServices();

            if (IsNotConnected) return;
            usuarioVerificado.PersonaDelUsuario = await personaSer.BuscarPersona(usuarioVerificado.PersonaDelUsuario);

            App.Usuario = usuarioVerificado;
            App.Persona = usuarioVerificado.PersonaDelUsuario;
            App.Persona.Usuarios = usuarioVerificado;
            App.IdiomaPersona = usuarioVerificado.PersonaDelUsuario.IdiomaDeLaPersona;

            App.ConfigureCultureIdiomsApp(usuarioVerificado.PersonaDelUsuario.IdiomaDeLaPersona);

            if (Recuerdame)
            {
                App.RecordedPerson = usuarioVerificado.PersonaDelUsuario.Consecutivo.ToString();
                App.RecordedIdiomPerson = usuarioVerificado.PersonaDelUsuario.CodigoIdioma.ToString();
                App.RecordedUser = usuarioVerificado.Usuario;
                App.RecordedPasswordUser = usuarioVerificado.Clave;
                App.Persona.PersonaRecordandose = Recuerdame;
            }

            var currentPage = App.Current.MainPage;

            BadgeColorTabbedNavigationContainer tabbedPage = App.ConfigureTabbedNavigationContainer(usuarioVerificado.PersonaDelUsuario, App.Usuario);
            CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainTabbedContainer);
            tabbedPage.CurrentPage = tabbedPage.Children[2];
            await App.ConnectPersonToChatHub();

            currentPage = null;
        }
    }
}