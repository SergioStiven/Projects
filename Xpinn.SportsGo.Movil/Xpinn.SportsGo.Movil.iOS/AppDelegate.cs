using System;
using CarouselView.FormsPlugin.iOS;
using FFImageLoading.Forms.Touch;
using Foundation;
using FreshEssentials.iOS;
using FreshMvvm;
using HockeyApp.iOS;
using Lottie.Forms.iOS.Renderers;
using Messier16.Forms.iOS.Controls;
using Octane.Xam.VideoPlayer.iOS;
using Syncfusion.SfImageEditor.XForms.iOS;
using Syncfusion.SfRating.XForms.iOS;
using System.Net.Http;
using FFImageLoading;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.RangeSlider.Forms;
using Xfx;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Movil.iOS.Dependencies;
using Xpinn.SportsGo.Movil.iOS.Renderers;
using Xpinn.SportsGo.Util;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Abstract;

[assembly: ResolutionGroupName("Xpinn.SportsGo.Effects")]
namespace Xpinn.SportsGo.Movil.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            InitDependenciesBeforeLibraries();
            global::Xamarin.Forms.Forms.Init();
            
            InitDependenciesAfterLibraries();
            RegisterDependencies();

            var manager = BITHockeyManager.SharedHockeyManager;

            // Desarrollo
            manager.Configure(AppConstants.IdHockeyAppiOS); 

            manager.CrashManager.CrashManagerStatus = BITCrashManagerStatus.AutoSend;
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation();

            LoadApplication(new App());

            UINavigationBar.Appearance.TintColor = ((Color)App.Current.Resources["BarTextAppColor"]).ToUIColor();
            UINavigationBar.Appearance.BarTintColor = ((Color)App.Current.Resources["PrimaryAppColor"]).ToUIColor();
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = ((Color)App.Current.Resources["BarTextAppColor"]).ToUIColor() };
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);

            return base.FinishedLaunching(app, options);
        }

        public override void ReceiveMemoryWarning(UIApplication application)
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.ReceiveMemoryWarning(application);
        }

        // Se inicializa cualquier libreria que se dependa en Xamarin.Forms, ya que el Linker o el Xamarin.Form puede trollearla 
        // Y descartar las librerias, esto pasa especialmente cuando se usan Third Party Libraries
        void InitDependenciesBeforeLibraries()
        {
            XfxControls.Init();
        }

        void InitDependenciesAfterLibraries()
        {
            CachedImageRenderer.Init();
            FormsVideoPlayer.Init(@"1EAE2298627D7932F01F1B2E700FE4631897463C");
            BadgeColorTabbedNavigationContainerRenderer.InitRender();
            ColorTabbedNavigationContainerRenderer.InitRender();
            AdvancedTimerImplementation.Init();
            Messier16Controls.InitAll();
            AnimationViewRenderer.Init();
            CarouselViewRenderer.Init();
            new AdvancedFrameRendereriOS();
            new SfRatingRenderer();
            new SfImageEditorRenderer();
            SfImageEditorRenderer.Init();
            var t = typeof(RangeSliderRenderer);
            var l = new LockHelper();
        }

        // Se registra las implementaciones para que asi se puedan resolver las dependencias :D
        void RegisterDependencies()
        {
            FreshIOC.Container.Register<ISecureableMessage, SecureMessagesHelper>().AsSingleton();
            FreshIOC.Container.Register<ILocalize, Localize>().AsSingleton();
            FreshIOC.Container.Register<IKeyboardHelper, iOSKeyboardHelper>().AsSingleton();
            FreshIOC.Container.Register<IVersionHelper, VersionHelper>().AsSingleton();
            FreshIOC.Container.Register<IAudioManager, AppleAudioManager>().AsSingleton();
            FreshIOC.Container.Register<IHelperImagen, iOSConverterImage>().AsSingleton();
            FreshIOC.Container.Register<IDateTimeHelper, DateTimeHelperNoPortable>().AsSingleton();
            FreshIOC.Container.Register<HttpMessageHandler, NSUrlSessionHandler>().AsSingleton();
            
            FreshIOC.Container.Register<ILockeable, LockHelper>().AsMultiInstance();
        }
    }
}
