using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Effects;
using BaseAndroid = Android;
using Xamarin.Forms.Internals;

[assembly: ExportEffect(typeof(ClearEntryEffect), nameof(ClearEntryEffect))]
namespace Xpinn.SportsGo.Movil.Android.Effects
{
    [Preserve(true, true)]
    public class ClearEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            ConfigureControl();
        }

        protected override void OnDetached()
        {
        }

        private void ConfigureControl()
        {
            var editText = Control as EditText;
            if (editText == null)
                return;
            
            editText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, Resource.Drawable.ic_clear_icon, 0);
            editText.SetOnTouchListener(new OnDrawableTouchListener());
        }
    }

    class OnDrawableTouchListener : Java.Lang.Object, BaseAndroid.Views.View.IOnTouchListener
    {
        public bool OnTouch(BaseAndroid.Views.View v, MotionEvent e)
        {
            if (v is EditText && e.Action == MotionEventActions.Up)
            {
                var editText = (EditText)v;

                if (e.RawX - 150 >= (editText.Right - (editText.GetCompoundDrawables()[2].Bounds.Width())))
                {
                    editText.Text = string.Empty;
                    return true;
                }
            }

            return false;
        }
    }
}