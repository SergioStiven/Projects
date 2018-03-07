using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xpinn.SportsGo.Movil.Controls
{
    public class BadgeColorTabbedNavigationContainer : BadgeTabbedNavigationContainer
    {
        public BadgeColorTabbedNavigationContainer() : base()
        {
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
        }

        public BadgeColorTabbedNavigationContainer(string navigationServiceName) : base(navigationServiceName)
        {
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
        }
    }
}
