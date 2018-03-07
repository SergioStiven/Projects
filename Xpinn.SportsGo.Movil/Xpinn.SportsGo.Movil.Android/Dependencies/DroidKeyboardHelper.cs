using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using Xamarin.Forms;
using Xpinn.SportsGo.Movil.Abstract;
using Xamarin.Forms.Internals;

namespace Xpinn.SportsGo.Movil.Android.Dependencies
{
    [Preserve(true, true)]
    public class DroidKeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            var context = MainApplication.CurrentContext;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && context is Activity)
            {
                var activity = context as Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}