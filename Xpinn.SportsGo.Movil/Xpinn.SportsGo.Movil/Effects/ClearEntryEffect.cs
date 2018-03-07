using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Effects
{
    public class ClearEntryEffect : RoutingEffect
    {
        public ClearEntryEffect() : base($"{App.EffectNamespace}.{nameof(ClearEntryEffect)}")
        {
        }
    }
}