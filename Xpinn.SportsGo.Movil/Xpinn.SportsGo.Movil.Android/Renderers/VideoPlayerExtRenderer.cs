using System;
using Octane.Xam.VideoPlayer.Android.Renderers;
using Octane.Xam.VideoPlayer.Constants;
using Xamarin.Forms.Platform.Android;
using Octane.Xam.VideoPlayer;
using Xamarin.Forms;
using Xpinn.SportsGo.Movil.Android.Renderers;
using Xpinn.SportsGo.Movil.Controls;

[assembly: ExportRenderer(typeof(VideoPlayerExtender), typeof(VideoPlayerExtRenderer))]
namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Xamarin.Forms.Internals.Preserve(true, true)]
    public class VideoPlayerExtRenderer : VideoPlayerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.Prepared += (s2, e2) =>
                {
                    UpdateVideoSize();
                    Control.Player.VideoSizeChanged += (s3, e3) => UpdateVideoSize();
                };
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            UpdateVideoSize();
        }

        private void UpdateVideoSize()
        {
            if (Element.FillMode == FillMode.Resize)
            {
                Control.Layout(0, 0, Width, Height);
                return;
            }

            // assume video size = view size if the player has not been loaded yet
            var videoWidth = Control.Player?.VideoWidth ?? Width;
            var videoHeight = Control.Player?.VideoHeight ?? Height;

            var scaleWidth = (double)Width / (double)videoWidth;
            var scaleHeight = (double)Height / (double)videoHeight;

            double scale;
            switch (Element.FillMode)
            {
                case FillMode.ResizeAspect:
                    scale = Math.Min(scaleWidth, scaleHeight);
                    break;
                case FillMode.ResizeAspectFill:
                    scale = Math.Max(scaleWidth, scaleHeight);
                    break;
                default:
                    // should not happen
                    scale = 1;
                    break;
            }

            var scaledWidth = (int)Math.Round(videoWidth * scale);
            var scaledHeight = (int)Math.Round(videoHeight * scale);

            // center the video
            var l = (Width - scaledWidth) / 2;
            var t = (Height - scaledHeight) / 2;
            var r = l + scaledWidth;
            var b = t + scaledHeight;
            Control.Layout(l, t, r, b);
        }
    }
}