using UIKit;
using Xpinn.SportsGo.Movil.Abstract;

namespace Xpinn.SportsGo.Movil.iOS.Dependencies
{
    [Foundation.Preserve(AllMembers = true)]
    public class iOSKeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;

                if (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }

                vc.View.EndEditing(true);
            });
        }
    }
}