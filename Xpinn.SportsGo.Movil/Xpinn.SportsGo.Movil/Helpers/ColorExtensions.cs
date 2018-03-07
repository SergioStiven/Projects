using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Helpers
{
    public static class ColorExtensions
    {
        public static Color Darken(this Color color)
        {
            return color.WithLuminosity(color.Luminosity * 0.7);
        }
    }
}
