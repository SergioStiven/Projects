using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xpinn.SportsGo.Movil.Controls;
using Xpinn.SportsGo.Movil.iOS.Renderers;

[assembly: ExportRenderer(typeof(BadgeTabbedNavigationContainer), typeof(ColorTabbedNavigationContainerRenderer))]
namespace Xpinn.SportsGo.Movil.iOS.Renderers
{
    [Foundation.Preserve(AllMembers = true)]
    public class ColorTabbedNavigationContainerRenderer : TabbedRenderer
    {
        public static void InitRender()
        {
            var test = DateTime.UtcNow;
        }

        BadgeTabbedNavigationContainer FormsTabbedPage => Element as BadgeTabbedNavigationContainer;
        UIColor DefaultTintColor;
        UIColor DefaultBarBackgroundColor;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);


            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
            }
            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnElementPropertyChanged;
            }

            DefaultTintColor = TabBar.TintColor;
            DefaultBarBackgroundColor = TabBar.BackgroundColor;
        }

        void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BadgeTabbedNavigationContainer.BarBackgroundColor):
                case nameof(BadgeTabbedNavigationContainer.BarBackgroundApplyTo):
                    SetBarBackgroundColor();
                    SetTintedColor();
                    break;
                case nameof(BadgeTabbedNavigationContainer.SelectedColor):
                    SetTintedColor();
                    break;
                default:
                    //base.OnElementProp⁄ertyChanged(sender, e);
                    break;
            }
        }

        public override void ViewWillAppear(bool animated)
        {

            if (TabBar?.Items == null)
                return;
            SetTintedColor();
            SetBarBackgroundColor();

            if (FormsTabbedPage != null)
            {
                for (int i = 0; i < TabBar.Items.Length; i++)
                {
                    var item = TabBar.Items[i];
                    var icon = FormsTabbedPage.Children[i].Icon;

                    if (item == null)
                        return;
                    try
                    {
                        icon = icon + "_active";
                        if (item?.SelectedImage?.AccessibilityIdentifier == icon)
                            return;
                        item.SelectedImage = UIImage.FromBundle(icon);
                        item.SelectedImage.AccessibilityIdentifier = icon;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to set selected icon: " + ex);
                    }
                }
            }

            base.ViewWillAppear(animated);
        }

        private void SetTintedColor()
        {
            if (FormsTabbedPage.SelectedColor != default(Color))
                TabBar.TintColor = FormsTabbedPage.SelectedColor.ToUIColor();
            else
            {
                TabBar.TintColor = DefaultTintColor;
            }
        }

        private void SetBarBackgroundColor()
        {
            if (FormsTabbedPage.BarBackgroundApplyTo.HasFlag(BarBackgroundApplyTo.iOS))
            {
                TabBar.BackgroundColor = FormsTabbedPage.BarBackgroundColor != default(Color)
                    ? FormsTabbedPage.BarBackgroundColor.ToUIColor()
                    : DefaultBarBackgroundColor;
            }
            else
            {
                TabBar.BackgroundColor = DefaultBarBackgroundColor;
            }
        }
    }
}