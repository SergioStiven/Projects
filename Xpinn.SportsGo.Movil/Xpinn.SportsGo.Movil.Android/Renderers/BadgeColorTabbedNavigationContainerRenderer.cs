using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Renderers;
using Xpinn.SportsGo.Movil.Controls;
using Xpinn.SportsGo.Movil.Helpers;
using BaseAndroid = Android;

[assembly: ExportRenderer(typeof(BadgeColorTabbedNavigationContainer), typeof(BadgeColorTabbedNavigationContainerRenderer))]
namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Preserve(true, true)]
    public class BadgeColorTabbedNavigationContainerRenderer : BadgeTabbedNavigationContainerRenderer
    {
        public BadgeColorTabbedNavigationContainerRenderer(Context context) : base(context)
        {

        }

        private BadgeColorTabbedNavigationContainer FormsTabbedPage => Element as BadgeColorTabbedNavigationContainer;
        BaseAndroid.Graphics.Color _selectedColor = BaseAndroid.Graphics.Color.Black;
        private static readonly BaseAndroid.Graphics.Color DefaultUnselectedColor = Xamarin.Forms.Color.Gray.Darken().ToAndroid();
        private static BaseAndroid.Graphics.Color BarBackgroundDefault;
        private BaseAndroid.Graphics.Color _unselectedColor = DefaultUnselectedColor;

        ViewPager _viewPager;
        TabLayout _tabLayout;

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {

            base.OnElementChanged(e);

            // Get tabs
            for (int i = 0; i < ChildCount; i++)
            {
                var v = GetChildAt(i);
                if (v is ViewPager)
                    _viewPager = (ViewPager)v;
                else if (v is TabLayout)
                    _tabLayout = (TabLayout)v;
            }


            if (e.OldElement != null)
            {
                _tabLayout.TabSelected -= TabLayout_TabSelected;
                _tabLayout.TabUnselected -= TabLayout_TabUnselected;
            }

            if (e.NewElement != null)
            {
                BarBackgroundDefault = (_tabLayout.Background as ColorDrawable)?.Color ?? BaseAndroid.Graphics.Color.Blue;
                SetSelectedColor();
                SetBarBackgroundColor();
                _tabLayout.TabSelected += TabLayout_TabSelected;
                _tabLayout.TabUnselected += TabLayout_TabUnselected;

                SetupTabColors();

                int lastPosition = _tabLayout.SelectedTabPosition;
                SelectTab(lastPosition);
            }
        }

        void SelectTab(int position)
        {
            if (_tabLayout.TabCount > position)
            {
                _tabLayout.GetTabAt(position).Icon?.SetColorFilter(_selectedColor, PorterDuff.Mode.SrcIn);
                _tabLayout.GetTabAt(position).Select();
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }


        void SetupTabColors()
        {
            _tabLayout.SetSelectedTabIndicatorColor(_selectedColor);
            _tabLayout.SetTabTextColors(_unselectedColor, _selectedColor);
            for (int i = 0; i < _tabLayout.TabCount; i++)
            {
                var tab = _tabLayout.GetTabAt(i);
                tab.Icon?.SetColorFilter(_unselectedColor, PorterDuff.Mode.SrcIn);
            }
        }

        private void TabLayout_TabUnselected(object sender, TabLayout.TabUnselectedEventArgs e)
        {
            var tab = e.Tab;
            tab.Icon?.SetColorFilter(_unselectedColor, PorterDuff.Mode.SrcIn);
        }

        private void TabLayout_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {
            var tab = e.Tab;
            _viewPager.CurrentItem = tab.Position;
            tab.Icon?.SetColorFilter(_selectedColor, PorterDuff.Mode.SrcIn);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int lastPosition = _tabLayout.SelectedTabPosition;
            switch (e.PropertyName)
            {
                case nameof(BadgeTabbedNavigationContainer.BarBackgroundColor):
                case nameof(BadgeTabbedNavigationContainer.BarBackgroundApplyTo):
                    SetBarBackgroundColor();
                    SetupTabColors();
                    SelectTab(lastPosition);
                    break;
                case nameof(BadgeTabbedNavigationContainer.SelectedColor):
                    SetSelectedColor();
                    SetupTabColors();
                    SelectTab(lastPosition);
                    break;
                default:
                    base.OnElementPropertyChanged(sender, e);
                    break;
            }
        }

        private void SetSelectedColor()
        {

            if (FormsTabbedPage.SelectedColor != default(Xamarin.Forms.Color))
                _selectedColor = FormsTabbedPage.SelectedColor.ToAndroid();
        }

        private void SetBarBackgroundColor()
        {
            if (FormsTabbedPage.BarBackgroundApplyTo.HasFlag(BarBackgroundApplyTo.Android))
            {
                _tabLayout.SetBackgroundColor(FormsTabbedPage.BarBackgroundColor.ToAndroid());
                _unselectedColor = FormsTabbedPage.BarBackgroundColor != default(Xamarin.Forms.Color)
                    ? FormsTabbedPage.BarBackgroundColor.Darken().ToAndroid()
                    : DefaultUnselectedColor;
            }
            else
            {
                _tabLayout.SetBackgroundColor(BarBackgroundDefault);
                _unselectedColor = DefaultUnselectedColor;
            }
        }
    }
}