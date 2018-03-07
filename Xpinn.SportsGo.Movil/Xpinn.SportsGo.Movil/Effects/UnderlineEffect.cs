using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Effects
{
    public class UnderlineEffect : RoutingEffect
    {
        public UnderlineEffect() : base($"{App.EffectNamespace}.{nameof(UnderlineEffect)}")
        {
        }
    }
}
