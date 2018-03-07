using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using Xpinn.SportsGo.Movil.iOS.Renderers;
using Xpinn.SportsGo.Movil.Renderers;

[assembly: ExportRenderer(typeof(NoSleepContentPage), typeof(NoSleepPageRenderer))]
namespace Xpinn.SportsGo.Movil.iOS.Renderers
{
    [Foundation.Preserve(AllMembers = true)]
    public class NoSleepPageRenderer : PageRenderer
    {

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
            }
            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnElementPropertyChanged;
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UIApplication.SharedApplication.IdleTimerDisabled = true;

            SetupSwipe();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            UIApplication.SharedApplication.IdleTimerDisabled = false;
        }

        void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NoSleepContentPage.SwipeEnabled):
                    SetupSwipe();
                    break;
            }
        }

        void SetupSwipe()
        {
            var contentPage = Element as NoSleepContentPage;
            if (contentPage != null)
            {
                var navctrl = this.ViewController.NavigationController;
                if (navctrl != null)
                {
                    navctrl.InteractivePopGestureRecognizer.Enabled = contentPage.SwipeEnabled;
                }
            }
        }
    }
}
