using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Renderers;
using Xpinn.SportsGo.Movil.Renderers;

[assembly: ExportRenderer(typeof(NoSwipeContentPage), typeof(NoSwipeContentPageRenderer))]
namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Preserve(true, true)]
    public class NoSwipeContentPageRenderer : PageRenderer
    {
        public NoSwipeContentPageRenderer(Context context) : base(context)
        {

        }
    }
}