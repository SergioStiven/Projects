using Android.Content;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Renderers;
using BaseAndroid = Android;

[assembly: ExportRenderer(typeof(Editor), typeof(BorderedEditorRenderer))]
namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Preserve(true, true)]
    public class BorderedEditorRenderer : EditorRenderer
    {

        public BorderedEditorRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                int borderWidth = 2;
                GradientDrawable customBG = new GradientDrawable();

                customBG.SetColor(BaseAndroid.Graphics.Color.Transparent);
                customBG.SetCornerRadius(3);
                customBG.SetStroke(borderWidth, BaseAndroid.Graphics.Color.Gray);

                Control.SetBackground(customBG);
            }
        }
    }
}