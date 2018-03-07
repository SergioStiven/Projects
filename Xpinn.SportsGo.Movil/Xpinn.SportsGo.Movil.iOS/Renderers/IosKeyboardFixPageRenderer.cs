using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xpinn.SportsGo.Movil.iOS.Renderers;
using Xpinn.SportsGo.Movil.Renderers;

[assembly: ExportRenderer(typeof(KeyboardResizingAwareContentPage), typeof(IosKeyboardFixPageRenderer))]
namespace Xpinn.SportsGo.Movil.iOS.Renderers
{
    [Foundation.Preserve(AllMembers = true)]
    public class IosKeyboardFixPageRenderer : PageRenderer
    {
        NSObject _onKeyBoardShow;
        NSObject _onKeyBoardHide;
        nfloat _barHeight;
        bool _tecladoArriba;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var cp = Element as KeyboardResizingAwareContentPage;
            if (cp != null && !cp.CancelsTouchesInView)
            {
                foreach (var g in View.GestureRecognizers)
                {
                    g.CancelsTouchesInView = false;
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (NavigationController != null && NavigationController.TabBarController != null && NavigationController.TabBarController.TabBar != null)
            {
                _barHeight = NavigationController.TabBarController.TabBar.Frame.Height;
            }
            
            _onKeyBoardShow = UIKeyboard.Notifications.ObserveWillShow(OnKeyBoardShow);
            _onKeyBoardHide = UIKeyboard.Notifications.ObserveWillHide(OnKeyBoardHide);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            _barHeight = 0;
            
            if (_onKeyBoardShow != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_onKeyBoardShow);
            }
            
            if (_onKeyBoardHide != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_onKeyBoardHide);
            }
        }

        void OnKeyBoardShow(object sender, UIKeyboardEventArgs args)
        {
            var frameEnd = args.FrameEnd;
            
            var page = Element as ContentPage;

            if (page != null && !(page.Content is ScrollView) && !_tecladoArriba)
            {
                var padding = page.Padding;

                var valorPaddingBottom = frameEnd.Height - _barHeight;
                if (valorPaddingBottom < 0)
                {
                    valorPaddingBottom = 0;
                }

                page.Padding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom + valorPaddingBottom);
                
                _tecladoArriba = true;
            }
        }

        void OnKeyBoardHide(object sender, UIKeyboardEventArgs args)
        {
            var frameBegin = args.FrameBegin;

            var page = Element as ContentPage;
            if (page != null && !(page.Content is ScrollView) && _tecladoArriba)
            {
                var padding = page.Padding;

                var valorPaddingBottom = frameBegin.Height - _barHeight;
                if (valorPaddingBottom < 0)
                {
                    valorPaddingBottom = 0;
                }

                page.Padding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom - valorPaddingBottom);

                _tecladoArriba = false;
            }
        }
    }
}