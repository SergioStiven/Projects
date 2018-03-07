using Android.App;
using Android.Views;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Renderers;
using Xpinn.SportsGo.Movil.Renderers;
using Android.Runtime;
using Android.Content;

[assembly: ExportRenderer(typeof(NoSleepContentPage), typeof(NoSleepPageRenderer))]
namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Xamarin.Forms.Internals.Preserve(true, true)]
    public class NoSleepPageRenderer : PageRenderer
    {
        public NoSleepPageRenderer(Context context) : base(context)
        {

        }

        protected override void OnWindowVisibilityChanged([GeneratedEnum] ViewStates visibility)
        {
            base.OnWindowVisibilityChanged(visibility); 

            if (visibility == ViewStates.Visible)
            {
                Activity activity = CrossCurrentActivity.Current.Activity;

                activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
            else
            {
                Activity activity = CrossCurrentActivity.Current.Activity;

                activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            }
        }
    }
}