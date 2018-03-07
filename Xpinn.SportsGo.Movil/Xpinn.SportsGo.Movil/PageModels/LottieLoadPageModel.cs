using FreshMvvm;
using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Controls;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class LottieLoadPageModel : BasePageModel
    {
        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                await BuscarInformacionParaLogin();

                if (App.Persona != null && App.Usuario != null)
                {
                    App.Persona.Usuarios = App.Usuario;
                    App.Persona.PersonaRecordandose = true;

                    App.IdiomaPersona = App.Persona.IdiomaDeLaPersona;
                    App.ConfigureCultureIdiomsApp(App.Persona.IdiomaDeLaPersona);

                    BadgeColorTabbedNavigationContainer tabbedPage = App.ConfigureTabbedNavigationContainer(App.Persona, App.Usuario);
                    CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainTabbedContainer);
                    tabbedPage.CurrentPage = tabbedPage.Children[2];
                }
                else
                {
                    FreshNavigationContainer basicNavContainer = App.ConfigureNavigationContainer();
                    CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.AuthenticationContainer);
                    ClearInfoUsuario();
                }
            }
            catch (Exception)
            {
                FreshNavigationContainer basicNavContainer = App.ConfigureNavigationContainer();
                CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.AuthenticationContainer);
                ClearInfoUsuario();
            }
        }

        async Task BuscarInformacionParaLogin()
        {
            PersonasServices personaSer = new PersonasServices();
            PersonasDTO personaParaBuscar = new PersonasDTO
            {
                Consecutivo = Convert.ToInt32(App.RecordedPerson),
                CodigoIdioma = Convert.ToInt32(App.RecordedIdiomPerson)
            };

            if (IsNotConnected) return;
            App.Persona = await personaSer.BuscarPersona(personaParaBuscar);

            if (App.Persona == null) return;

            AuthenticateServices authSer = new AuthenticateServices();
            UsuariosDTO usuarioParaBuscar = new UsuariosDTO
            {
                Usuario = App.RecordedUser,
                Clave = App.RecordedPasswordUser
            };

            if (IsNotConnected) return;
            App.Usuario = await authSer.VerificarUsuario(usuarioParaBuscar);

            if (App.Usuario == null) return;

            if (!IsNotConnected)
            {
                await App.ConnectPersonToChatHub();
            }
        }

        void ClearInfoUsuario()
        {
            App.Persona = null;
            App.Usuario = null;
            App.RecordedPerson = App.RecordedIdiomPerson = App.RecordedUser = App.RecordedPasswordUser = null;
        }
    }
}
