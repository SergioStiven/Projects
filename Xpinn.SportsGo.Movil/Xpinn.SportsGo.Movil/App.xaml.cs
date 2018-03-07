using FreshMvvm;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.PageModels;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Movil.Controls;
using System.Threading.Tasks;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Movil.Pages;
using System;
using Xpinn.SportsGo.Util.Portable.Args;
using Plugin.Connectivity;
using Xpinn.SportsGo.Util.Portable;
using Plugin.Settings;
using Plugin.DeviceInfo;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Plugin.FirebasePushNotification;
using System.Linq;
using System.Threading;

namespace Xpinn.SportsGo.Movil
{
    public partial class App : Application
    {
        public const string EffectNamespace = "Xpinn.SportsGo.Effects";
        public const string ResourceId = "Xpinn.SportsGo.Movil.Resources.SportsGoResources";

        static readonly string AppSettingsRecordedPerson = "Xpinn.SportsGo.Movil.RecordedPerson";
        static readonly string AppSettingsDefaultRecordedPerson = string.Empty;

        static readonly string AppSettingsRecordedIdiomPerson = "Xpinn.SportsGo.Movil.RecordedIdiomPerson";
        static readonly string AppSettingsDefaultRecordedIdiomPerson = string.Empty;

        static readonly string AppSettingsRecordedUser = "Xpinn.SportsGo.Movil.RecordedUser";
        static readonly string AppSettingsDefaultRecordedUser = string.Empty;

        static readonly string AppSettingsRecordedPasswordUser = "Xpinn.SportsGo.Movil.RecordedPasswordUser";
        static readonly string AppSettingsDefaultRecordedPasswordUser = string.Empty;

        public static string RecordedPerson
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(AppSettingsRecordedPerson, AppSettingsDefaultRecordedPerson);
            }
            set
            {
                CrossSettings.Current.AddOrUpdateValue(AppSettingsRecordedPerson, value);
            }
        }

        public static string RecordedIdiomPerson
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(AppSettingsRecordedIdiomPerson, AppSettingsDefaultRecordedIdiomPerson);
            }
            set
            {
                CrossSettings.Current.AddOrUpdateValue(AppSettingsRecordedIdiomPerson, value);
            }
        }

        public static string RecordedUser
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(AppSettingsRecordedUser, AppSettingsDefaultRecordedUser);
            }
            set
            {
                CrossSettings.Current.AddOrUpdateValue(AppSettingsRecordedUser, value);
            }
        }

        public static string RecordedPasswordUser
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(AppSettingsRecordedPasswordUser, AppSettingsDefaultRecordedPasswordUser);
            }
            set
            {
                CrossSettings.Current.AddOrUpdateValue(AppSettingsRecordedPasswordUser, value);
            }
        }

        public static string DeviceId { get { return CrossDeviceInfo.Current.Id; } }
        public static Color PrimaryColorWithAlpha { get { return ((Color)Current.Resources["PrimaryAppColor"]).MultiplyAlpha(0.30); } }
        public static string ConnectionIDChatHub { get { return ChatsServices.ConnectionIDChatHub; } }
        public static UsuariosDTO Usuario { get; set; }
        public static PersonasDTO Persona { get; set; }
        public static CultureInfo ActualCulture { get; private set; }
        public static App Instance { get; private set; }
        public static bool AppIsInBackGround { get; private set; }

        static Idioma _idiomaPersona;
        public static Idioma IdiomaPersona
        {
            get
            {
                if (Persona != null)
                {
                    _idiomaPersona = Persona.IdiomaDeLaPersona;
                    return Persona.IdiomaDeLaPersona;
                }
                else
                {
                    return _idiomaPersona;
                }
            }
            set
            {
                _idiomaPersona = value;
            }
        }

        public static readonly List<IdiomaModel> ListaIdioma = new List<IdiomaModel>
                                                                {
                                                                    new IdiomaModel(Idioma.Español),
                                                                    new IdiomaModel(Idioma.Ingles),
                                                                    new IdiomaModel(Idioma.Portugues)
                                                                };

        public static readonly List<TipoPerfilModel> ListaTipoPerfil = new List<TipoPerfilModel>
                                                                {
                                                                    new TipoPerfilModel(TipoPerfil.Candidato),
                                                                    new TipoPerfilModel(TipoPerfil.Grupo),
                                                                    new TipoPerfilModel(TipoPerfil.Representante)
                                                                };

        public static bool EstoyEnPantallaConversacion
        {
            get
            {
                BadgeColorTabbedNavigationContainer currentPage = Current.MainPage as BadgeColorTabbedNavigationContainer;
                bool estoyConversando = false;

                if (currentPage != null)
                {
                    NavigationPage currentNavigation = currentPage.CurrentPage as NavigationPage;

                    if (currentNavigation != null)
                    {
                        estoyConversando = currentNavigation.CurrentPage is ConversacionChatPage;
                    }
                }

                return estoyConversando;
            }
        }

        static ILockeable _lockeable;
        static bool _appInicializada;
        bool _sonidoYaFueInicializado;

        public App()
        {
            InitializeComponent();

            // Configurar idioma cada vez que el app inicializa
            ILocalize localizeDependency = DependencyService.Get<ILocalize>();
            CultureInfo ci = localizeDependency.GetCurrentCultureInfo();
            ConfigureCultureIdiomsApp(ci);

            // Inicializamos locker del app instance
            if (_lockeable == null)
            {
                _lockeable = FreshIOC.Container.Resolve<ILockeable>();
            }

            // Solo se necesita inicializar sonidos cuando se reproduce un sound personalizado, con PlaySound
            // Si usar PlayNotificationDefaultSound no se necesita
            //// Inicializamos sonidos 
            //if (Device.RuntimePlatform == Device.Android)
            //{
            //    InicializarSoundsFiles();
            //}

            if (!string.IsNullOrWhiteSpace(RecordedPerson) && !string.IsNullOrWhiteSpace(RecordedIdiomPerson) && !string.IsNullOrWhiteSpace(RecordedUser) && !string.IsNullOrWhiteSpace(RecordedPasswordUser))
            {
                MainPage = FreshPageModelResolver.ResolvePageModel<LottieLoadPageModel>();
            }
            else
            {
                FreshNavigationContainer basicNavContainer = ConfigureNavigationContainer();
                MainPage = basicNavContainer;
            }

            _appInicializada = true;
        }

        public static FreshNavigationContainer ConfigureNavigationContainer()
        {
            Page carouselPage = FreshPageModelResolver.ResolvePageModel<MainPageModel>();
            FreshNavigationContainer basicNavContainer = new FreshNavigationContainer(carouselPage, NavigationContainerNames.AuthenticationContainer);
            basicNavContainer.BarBackgroundColor = (Color)Current.Resources["PrimaryAppColor"];
            basicNavContainer.BarTextColor = (Color)Current.Resources["BarTextAppColor"];
            return basicNavContainer;
        }

        public static BadgeColorTabbedNavigationContainer ConfigureTabbedNavigationContainer(PersonasDTO persona, UsuariosDTO usuario)
        {
            BadgeColorTabbedNavigationContainer tabbedNavigation = new BadgeColorTabbedNavigationContainer(NavigationContainerNames.MainTabbedContainer);

            tabbedNavigation.AddTab<ChatPageModel>(null, "ic_tab_chat");
            tabbedNavigation.AddTab<BuscadorPageModel>(null, "ic_tab_buscar");
            tabbedNavigation.AddTab<NoticiasPageModel>(null, "ic_tab_news");
            tabbedNavigation.AddTab<NotificacionesPageModel>(null, "ic_tab_notifications");
            tabbedNavigation.AddTab<PerfilPageModel>(null, "ic_tab_profile", persona);

            tabbedNavigation.BarBackgroundColor = Color.White;
            tabbedNavigation.SelectedColor = (Color)App.Current.Resources["PrimaryAppColor"];
            tabbedNavigation.BarBackgroundApplyTo = BarBackgroundApplyTo.Android;

            return tabbedNavigation;
        }

        public static void ConfigureCultureIdiomsApp(Idioma idioma)
        {
            CultureInfo ci = null;

            if (idioma == Idioma.Español)
            {
                ci = new CultureInfo("es");
            }
            else if (idioma == Idioma.Portugues)
            {
                ci = new CultureInfo("pt");
            }
            else if (idioma == Idioma.Ingles)
            {
                ci = new CultureInfo("en");
            }

            if (ci != null)
            {
                ConfigureCultureIdiomsApp(ci);
            }
        }

        public static void ConfigureCultureIdiomsApp(CultureInfo ci)
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                string twoLetterISOLanguageName = ci.TwoLetterISOLanguageName.ToLowerInvariant().Trim();

                if (twoLetterISOLanguageName != "en" && twoLetterISOLanguageName != "es" && twoLetterISOLanguageName != "pt")
                {
                    ci = new CultureInfo("en");
                    IdiomaPersona = Idioma.Ingles;
                }
                else if (twoLetterISOLanguageName == "en")
                {
                    IdiomaPersona = Idioma.Ingles;
                }
                else if (twoLetterISOLanguageName == "es")
                {
                    IdiomaPersona = Idioma.Español;
                }
                else if (twoLetterISOLanguageName == "pt")
                {
                    IdiomaPersona = Idioma.Portugues;
                }

                SportsGoResources.Culture = ci; // set the RESX for resource localization
                ActualCulture = ci;
                //DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }
        }

        // Debido a un bug en la libreria la primera invocacion del sonido no funciona
        async void InicializarSoundsFiles()
        {
            if (!_sonidoYaFueInicializado)
            {
                IAudioManager audioManager = FreshIOC.Container.Resolve<IAudioManager>();

                float vol = audioManager.EffectsVolume;
                audioManager.EffectsVolume = 0;

                await audioManager.PlaySound("notificationSound.mp3");

                audioManager.EffectsVolume = vol;

                _sonidoYaFueInicializado = true;
            }
        }

        public static async Task ConnectPersonToChatHub()
        {
            if (Persona != null)
            {
                ChatsServices chatService = new ChatsServices();

                for (int i = 0; i < 3; i++)
                {
                    if (CrossConnectivity.Current.IsConnected && await CrossConnectivity.Current.IsRemoteReachable(URL.Host, msTimeout: 1000))
                    {
                        try
                        {
                            await chatService.ConectarChatHub(Persona.Consecutivo);

                            if (!string.IsNullOrWhiteSpace(ConnectionIDChatHub))
                            {
                                DisposeChatEvents();
                                ChatsServices.ErrorConexion += ChatsServices_ErrorConexion;
                                ChatsServices.ConexionPerdida += ChatsServices_ConexionPerdida;
                                break;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        public static void DisposeChatEvents()
        {
            ChatsServices.ErrorConexion -= ChatsServices_ErrorConexion;
            ChatsServices.ConexionPerdida -= ChatsServices_ConexionPerdida;
        }

        static async void ChatsServices_ConexionPerdida(object sender, EventArgs e)
        {
            await ReconectarChatHub();
        }

        static async void ChatsServices_ErrorConexion(object sender, ExceptionArgs e)
        {
            await ReconectarChatHub();
        }

        static bool _reconectando;
        public static async Task ReconectarChatHub()
        {
            if (string.IsNullOrWhiteSpace(ConnectionIDChatHub) && Persona != null && _appInicializada && !_reconectando)
            {
                try
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    using (await _lockeable.LockAsync(cancellationTokenSource.Token))
                    {
                        await Task.Run(async () =>
                        {
                            _reconectando = true;

                            ChatsServices chatService = new ChatsServices();
                            while (string.IsNullOrWhiteSpace(ConnectionIDChatHub))
                            {
                                if (await CrossConnectivity.Current.IsRemoteReachable(URL.Host, msTimeout: 1000))
                                {
                                    try
                                    {
                                        await chatService.ConectarChatHub(Persona.Consecutivo);

                                        if (!string.IsNullOrWhiteSpace(ConnectionIDChatHub))
                                        {
                                            _reconectando = false;
                                            break;
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }

                                await Task.Delay(3000);
                            }
                        });
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public static void InteractuarValorBadgeNotificacion(int numero)
        {
            BadgeColorTabbedNavigationContainer currentPage = Current.MainPage as BadgeColorTabbedNavigationContainer;

            if (currentPage != null)
            {
                NavigationPage currentNavigation = currentPage.CurrentPage as NavigationPage;

                if (currentNavigation != null)
                {
                    Page notificacionPage = currentPage.Children[3];

                    string value = notificacionPage.GetValue(TabBadge.BadgeTextProperty) as string;
                    int currentBadgeValue = 0;

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        currentBadgeValue = Convert.ToInt32(value);
                    }

                    int nuevoValor = currentBadgeValue + numero;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (nuevoValor <= 0)
                        {
                            notificacionPage.SetValue(TabBadge.BadgeTextProperty, string.Empty);
                        }
                        else
                        {
                            notificacionPage.SetValue(TabBadge.BadgeTextProperty, nuevoValor);
                        }
                    });
                }
            }
        }

        public static void InteractuarValorBadgeChat(int numero)
        {
            BadgeColorTabbedNavigationContainer currentPage = Current.MainPage as BadgeColorTabbedNavigationContainer;

            if (currentPage != null)
            {
                NavigationPage currentNavigation = currentPage.CurrentPage as NavigationPage;

                if (currentNavigation != null)
                {
                    Page chatPage = currentPage.Children[0];

                    string value = chatPage.GetValue(TabBadge.BadgeTextProperty) as string;
                    int currentBadgeValue = 0;

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        currentBadgeValue = Convert.ToInt32(value);
                    }

                    int nuevoValor = currentBadgeValue + numero;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (nuevoValor <= 0)
                        {
                            chatPage.SetValue(TabBadge.BadgeTextProperty, string.Empty);
                        }
                        else
                        {
                            chatPage.SetValue(TabBadge.BadgeTextProperty, nuevoValor);
                        }
                    });
                }
            }
        }

        protected override void OnStart()
        {
            AppIsInBackGround = false;

            //Al abrir el app me desuscribo a todo ya que el app maneja notificaciones localmente
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Count() > 0)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
            }
        }

        protected override void OnSleep()
        {
            Instance = this;
            AppIsInBackGround = true;

            // Al cerrar el app si tengo una persona valida me suscribo a notificaciones push
            if (Persona != null)
            {
                CrossFirebasePushNotification.Current.Subscribe(Persona.Consecutivo.ToString());
            }
        }

        protected override void OnResume()
        {
            AppIsInBackGround = false;

            //Al abrir el app me desuscribo a todo ya que el app maneja notificaciones localmente
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Count() > 0)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
            }
        }
    }
}
