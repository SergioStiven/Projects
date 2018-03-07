using System.ComponentModel;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Renderers;
using BaseAndroid = Android;
using Xamarin.Forms.Internals;
using Android.Content;

[assembly: ExportRenderer (typeof (Label), typeof (DpLabelRenderer))]
namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Preserve(true, true)]
    public class DpLabelRenderer : LabelRenderer
    {
        public DpLabelRenderer(Context context) : base(context)
        {

        }

        protected void setFontSizeAgain()
        {
            var nativeControl = Control;
            var xfControl = Element; //e.NewElement;
            if (nativeControl != null && xfControl != null)
            {
                // Se debe llamar al metodo ToScaledPixel() o cuando el usuario aumente las letras en la accesibilidad del dispositivo
                // Estas letras creceran demasiado grandes para ser manejadas por el app
                #pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    nativeControl.SetTextSize(BaseAndroid.Util.ComplexUnitType.Dip, xfControl.Font.ToScaledPixel());
                #pragma warning restore CS0618 // El tipo o el miembro están obsoletos
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);
            setFontSizeAgain();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            setFontSizeAgain();
        }
    }
}