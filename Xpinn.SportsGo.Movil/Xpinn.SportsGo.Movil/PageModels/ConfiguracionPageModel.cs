using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ConfiguracionPageModel : BasePageModel
    {
        PlanesServices _planesService;
        IVersionHelper _versionHelper;

        public PersonasDTO Persona { get; set; }

        public string NombreDelUsuario
        {
            get
            {
                string nombreDelUsuario = string.Empty;

                if (Persona != null && Persona.Usuarios != null)
                {
                    nombreDelUsuario = Persona.Usuarios.Usuario;
                }

                return nombreDelUsuario;
            }
        }

        public string NombreTipoPerfil
        {
            get
            {
                string nombreTipoPerfil = string.Empty;

                if (Persona != null)
                {
                    nombreTipoPerfil = TipoPerfilModel.RecuperarNombreTipoPerfil(Persona.TipoPerfil);
                }

                return nombreTipoPerfil;
            }
        }

        public string NombreDelPlan
        {
            get
            {
                string nombreDelPlan = string.Empty;

                if (Persona != null && Persona.Usuarios != null && Persona.Usuarios.PlanesUsuarios != null && Persona.Usuarios.PlanesUsuarios.Planes != null)
                {
                    nombreDelPlan = Persona.Usuarios.PlanesUsuarios.Planes.DescripcionIdiomaBuscado;
                }

                return nombreDelPlan;
            }
        }

        public string VencimientoDelPlan
        {
            get
            {
                string vencimientoDelPlan = string.Empty;

                if (Persona != null && Persona.Usuarios != null && Persona.Usuarios.PlanesUsuarios != null)
                {
                    int year = Persona.Usuarios.PlanesUsuarios.Vencimiento.Year;
                    int month = Persona.Usuarios.PlanesUsuarios.Vencimiento.Month;
                    int day = Persona.Usuarios.PlanesUsuarios.Vencimiento.Day;

                    if (year == DateTime.MaxValue.Year && month == DateTime.MaxValue.Month && day == DateTime.MaxValue.Day)
                    {
                        vencimientoDelPlan = SportsGoResources.NoVence;
                    }
                    else
                    {
                        vencimientoDelPlan = Persona.Usuarios.PlanesUsuarios.Vencimiento.ToString("d");
                    }
                }

                return vencimientoDelPlan;
            }
        }

        public string DetalleDeVersion
        {
            get
            {
                string detalleVersion = string.Empty;

                if (_versionHelper != null)
                {
                    detalleVersion = _versionHelper.VersionName + " (Build " + _versionHelper.VersionCode + ")";
                }

                return detalleVersion;
            }
        }

        public ConfiguracionPageModel(IVersionHelper versionHelper)
        {
            _planesService = new PlanesServices();
            _versionHelper = versionHelper;
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Persona = App.Persona;

            RaisePropertyChanged(nameof(NombreDelPlan));
            RaisePropertyChanged(nameof(VencimientoDelPlan));
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            RaisePropertyChanged(nameof(NombreDelUsuario));
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        public ICommand VerTerminosCondiciones
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

        public ICommand VerDatosCuenta
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<ConfiguracionUsuarioPageModel>(Persona.Usuarios);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand VerDetallePlan
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<DetallePlanPageModel>(App.Usuario.PlanesUsuarios.Planes);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand HistorialPagos
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<HistorialPagosPageModel>();

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand CerrarSesion
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    App.RecordedPerson = string.Empty;
                    App.RecordedIdiomPerson = string.Empty;
                    App.RecordedUser = string.Empty;
                    App.RecordedPasswordUser = string.Empty;

                    App.Persona = null;
                    App.Usuario = null;

                    App.DisposeChatEvents();
                    ChatsServices.DisposeChatHub();

                    var currentPage = App.Current.MainPage;
                    FreshNavigationContainer basicNavContainer = App.ConfigureNavigationContainer();
                    CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.AuthenticationContainer);
                    currentPage = null;

                    GC.Collect(2, GCCollectionMode.Forced);

                    tcs.SetResult(true);
                });
            }
        }
    }
}
