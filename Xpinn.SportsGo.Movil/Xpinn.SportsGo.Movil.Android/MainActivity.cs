using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using CarouselView.FormsPlugin.Android;
using FFImageLoading.Forms;
using FFImageLoading.Forms.Droid;
using FreshEssentials.Droid;
using FreshMvvm;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using Messier16.Forms.Android.Controls;
using Octane.Xam.VideoPlayer.Android;
using Plugin.FirebasePushNotification;
using Plugin.Permissions;
using Syncfusion.SfImageEditor.XForms.Droid;
using Syncfusion.SfRating.XForms.Droid;
using System.Net.Http;
using Xamarin.Android.Net;
using Xamarin.Forms;
using Xamarin.RangeSlider.Forms;
using Xfx;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Movil.Android.Dependencies;
using Xpinn.SportsGo.Movil.Android.Renderers;
using Xpinn.SportsGo.Util;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Abstract;

[assembly: ResolutionGroupName("Xpinn.SportsGo.Effects")]
namespace Xpinn.SportsGo.Movil.Android
{
    [Activity(LaunchMode = LaunchMode.SingleTask, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);
            
            if (App.Instance != null)
            {
                LoadApplication(App.Instance);
            }
            else
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                InitDependenciesBeforeLibraries();

                Forms.SetFlags("FastRenderers_Experimental");
                global::Xamarin.Forms.Forms.Init(this, bundle);

                InitDependenciesAfterLibraries();
                RegisterDependencies();

                CrashManager.Register(this, AppConstants.IdHockeyAppAndroid, new XpinnCrashManagerListener());
                MetricsManager.Register(Application, AppConstants.IdHockeyAppAndroid);

                LoadApplication(new App());
            }

            FirebasePushNotificationManager.ProcessIntent(Intent);

            CheckForUpdates();
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
            CachedImage.FixedAndroidMotionEventHandler = true;
            CachedImage.FixedOnMeasureBehavior = true;
            CarouselViewRenderer.Init();
            FormsVideoPlayer.Init(@"966AC4DFAAB5D7CEAC7250857C34B76223973994");
            BadgeColorTabbedNavigationContainerRenderer.Init();
            BadgeTabbedNavigationContainerRenderer.Init();
            AdvancedTimerImplementation.Init();
            Messier16Controls.InitAll();
            UserDialogs.Init(this);
            new AdvancedFrameRendererDroid();
            new SfRatingRenderer();
            new SfImageEditorRenderer();
            var t = typeof(RangeSliderRenderer);
            var l = new LockHelper();
        }

        // Se registra las implementaciones para que asi se puedan resolver las dependencias :D
        void RegisterDependencies()
        {
            FreshIOC.Container.Register<ISecureableMessage, SecureMessagesHelper>().AsSingleton();
            FreshIOC.Container.Register<ILocalize, Localize>().AsSingleton();
            FreshIOC.Container.Register<IKeyboardHelper, DroidKeyboardHelper>().AsSingleton();
            FreshIOC.Container.Register<IVersionHelper, VersionHelper>().AsSingleton();
            FreshIOC.Container.Register<IAudioManager, DroidAudioManager>().AsSingleton();
            FreshIOC.Container.Register<IHelperImagen, DroidConverterImage>().AsSingleton();
            FreshIOC.Container.Register<IDateTimeHelper, DateTimeHelperNoPortable>().AsSingleton();
            FreshIOC.Container.Register<HttpMessageHandler, AndroidClientHandler>().AsSingleton();

            FreshIOC.Container.Register<ILockeable, LockHelper>().AsMultiInstance();
        }

        void CheckForUpdates()
        {
            // Remove this for store builds!
            UpdateManager.Register(this, AppConstants.IdHockeyAppAndroid);
        }

        void UnregisterManagers()
        {
            UpdateManager.Unregister();
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterManagers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterManagers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(intent);
        }
    }

    class XpinnCrashManagerListener : CrashManagerListener
    {
        public override bool ShouldAutoUploadCrashes()
        {
            return true;
        }
    }
}

