using FreshMvvm;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class RecuperarClavePageModel : BasePageModel
    {
        AuthenticateServices _authService;

        public string Usuario { get; set; }
        public string Email { get; set; }
        public bool AceptaHuella { get; set; }

        public RecuperarClavePageModel()
        {
            _authService = new AuthenticateServices();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            //AceptaHuella = await CrossFingerprint.Current.IsAvailableAsync();
            AceptaHuella = false;
        }

        public ICommand RecuperarConCorreo
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
                        UsuariosDTO usuarioDTO = new UsuariosDTO
                        {
                            Usuario = Usuario,
                            Email = Email
                        };

                        if (IsNotConnected)
                        {
                            tcs.SetResult(true);
                            return;
                        }
                        WrapperSimpleTypesDTO wrapper = await _authService.RecuperarClave(usuarioDTO);

                        if (wrapper != null && wrapper.Exitoso)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.RecuperacionClaveExitosa, "OK");
                        }
                        else
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.UsuarioNoExiste, "OK");
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorRecuperarClave, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand RecuperarConHuella
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
                        FingerprintAuthenticationResult result = await AuthenticateAsync();

                        if (result.Authenticated)
                        {
                            UsuariosDTO usuarioParaRecuperar = new UsuariosDTO
                            {
                                Usuario = Usuario,
                                Email = Email,
                                DeviceId = App.DeviceId
                            };

                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            UsuariosDTO usuarioRecuperado = await _authService.VerificarUsuarioConEmailUsuarioYDeviceId(usuarioParaRecuperar);

                            if (usuarioRecuperado == null)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorValidarUsuario, "OK");
                                tcs.SetResult(true);
                                return;
                            }

                            if (usuarioRecuperado.CuentaActiva == 0)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.TituloAlerta, SportsGoResources.CuentaNoActiva, "OK");
                                tcs.SetResult(true);
                                return;
                            }

                            usuarioRecuperado.EsRecuperarClave = true;

                            await CoreMethods.PushPageModel<ConfiguracionUsuarioPageModel>(usuarioRecuperado);
                        }
                        else if (result.Status == FingerprintAuthenticationResultStatus.TooManyAttempts)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DemasiadosIntentos, "OK");
                        }
                        else if (result.Status != FingerprintAuthenticationResultStatus.Canceled)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.NoSePudoValidarHuella, "OK");
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorRecuperarClave, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task<bool> ValidarDatosCuenta()
        {
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Email))
            {
                esValido = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltanDatosCuenta, "OK");
            }

            return esValido;
        }
    }
}
