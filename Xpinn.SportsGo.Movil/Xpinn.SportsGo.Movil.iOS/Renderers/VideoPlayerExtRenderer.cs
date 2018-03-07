using Octane.Xam.VideoPlayer.iOS.Renderers;
using Xamarin.Forms;
using Xpinn.SportsGo.Movil.Controls;
using Xpinn.SportsGo.Movil.iOS.Renderers;

[assembly: ExportRenderer(typeof(VideoPlayerExtender), typeof(VideoPlayerExtRenderer))]
namespace Xpinn.SportsGo.Movil.iOS.Renderers
{
    [Foundation.Preserve(AllMembers = true)]
    public class VideoPlayerExtRenderer : VideoPlayerRenderer
    {
    }
}