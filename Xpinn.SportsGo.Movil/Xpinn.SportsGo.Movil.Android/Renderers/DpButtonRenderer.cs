using Android.Content;
using Android.Graphics;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Renderers;
using BaseAndroid = Android;

[assembly: ExportRenderer(typeof(Button), typeof(DpButtonRenderer))]

namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Preserve(true, true)]
    public class DpButtonRenderer : ButtonRenderer
    {
        public DpButtonRenderer(Context context) : base(context)
        {

        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }

        protected void SetFontSizeAgain()
        {
            var abtn = (BaseAndroid.Widget.Button)Control;
            var xfbtn = Element;
            if (abtn != null && xfbtn != null)
                abtn.SetTextSize(BaseAndroid.Util.ComplexUnitType.Dip, xfbtn.Font.ToScaledPixel());
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            SetFontSizeAgain();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Button.FontProperty.PropertyName)
                SetFontSizeAgain();
        }
    }
}