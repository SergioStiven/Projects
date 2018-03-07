using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xpinn.SportsGo.Movil.iOS;
using Xpinn.SportsGo.Movil.Renderers;

[assembly: ExportRenderer(typeof(NoSwipeContentPage), typeof(NoSwipeContentPageRenderer))]
namespace Xpinn.SportsGo.Movil.iOS
{
    [Foundation.Preserve(AllMembers = true)]
    public class NoSwipeContentPageRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            var navctrl = this.ViewController.NavigationController;
            navctrl.InteractivePopGestureRecognizer.Enabled = false;
        }
    }
}