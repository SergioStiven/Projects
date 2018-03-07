using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using FFImageLoading;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using System;

namespace Xpinn.SportsGo.Movil.Android
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static Context CurrentContext { get { return CrossCurrentActivity.Current.Activity.BaseContext; }  }

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);

            // To implement fingerprint support with plugin
            //CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
            //CrossFingerprint.SetDialogFragmentType<NoFingerPrintFallBackDialogFragment>();

            //If debug you should reset the token each time.
            #if DEBUG
                FirebasePushNotificationManager.Initialize(this,true);
            #else
                FirebasePushNotificationManager.Initialize(this,false);
            #endif

            //Handle notification when app is closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {

            };

            //A great place to initialize Xamarin.Insights and Dependency Services!
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public override void OnLowMemory()
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnLowMemory();
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}