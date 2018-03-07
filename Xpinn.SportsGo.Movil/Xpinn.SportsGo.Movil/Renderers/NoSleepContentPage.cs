using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Renderers
{
    public class NoSleepContentPage : ContentPage
    {
        public static readonly BindableProperty SwipeEnabledProperty =
        BindableProperty.Create(
            nameof(SwipeEnabled),
            typeof(bool),
            typeof(NoSleepContentPage),
            true,
            BindingMode.TwoWay
        );

        public bool SwipeEnabled
        {
            get { return (bool)GetValue(SwipeEnabledProperty); }
            set { SetValue(SwipeEnabledProperty, value); }
        }
    }
}
