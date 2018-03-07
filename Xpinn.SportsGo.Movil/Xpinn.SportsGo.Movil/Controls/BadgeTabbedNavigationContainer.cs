using FreshMvvm;
using System;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Controls
{
    [Flags]
    public enum BarBackgroundApplyTo
    {
        None = 0x01,
        Android = 0x10,
        iOS = 0x100
    }

    public class BadgeTabbedNavigationContainer : FreshTabbedNavigationContainer
    {
        public BadgeTabbedNavigationContainer() : base()
        {

        }

        public BadgeTabbedNavigationContainer(string navigationServiceName) : base(navigationServiceName)
        {

        }


        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(
                nameof(SelectedColor), 
                typeof(Color), 
                typeof(BadgeTabbedNavigationContainer), 
                default(Color)
            );

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly new BindableProperty BarBackgroundColorProperty =
            BindableProperty.Create(
                nameof(BarBackgroundColor), 
                typeof(Color), 
                typeof(BadgeTabbedNavigationContainer), 
                default(Color)
            );

        public new Color BarBackgroundColor
        {
            get { return (Color)GetValue(BarBackgroundColorProperty); }
            set { SetValue(BarBackgroundColorProperty, value); }
        }

        public static readonly BindableProperty BarBackgroundApplyToProperty =
            BindableProperty.Create(
                nameof(BarBackgroundApplyTo), 
                typeof(BarBackgroundApplyTo), 
                typeof(BadgeTabbedNavigationContainer), 
                BarBackgroundApplyTo.Android
            );

        public BarBackgroundApplyTo BarBackgroundApplyTo
        {
            get { return (BarBackgroundApplyTo)GetValue(BarBackgroundApplyToProperty); }
            set { SetValue(BarBackgroundApplyToProperty, value); }
        }
    }
}
