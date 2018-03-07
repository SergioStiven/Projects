using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class Monedas
    {
        public MonedasEnum MonedaEnum
        {
            get
            {
                return Consecutivo.ToEnum<MonedasEnum>();
            }
            set
            {
                Consecutivo = (int)value;
            }
        }
    }
}
