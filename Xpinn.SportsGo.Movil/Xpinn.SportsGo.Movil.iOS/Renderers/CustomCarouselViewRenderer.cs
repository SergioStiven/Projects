using CarouselView.FormsPlugin.iOS;
using Xamarin.Forms;
using Xpinn.SportsGo.Movil.Controls;
using Xpinn.SportsGo.Movil.iOS.Renderers;

[assembly: ExportRenderer(typeof(CustomCarouselView), typeof(CustomCarouselViewRenderer))]
namespace Xpinn.SportsGo.Movil.iOS.Renderers
{
    class CustomCarouselViewRenderer : CarouselViewRenderer
    {
    }
}
