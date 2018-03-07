using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Entities
{
    public partial class MonedasDTO
    {
        public MonedasEnum MonedaEnum
        {
            get
            {
                MonedasEnum moneda;

                Enum.TryParse(Consecutivo.ToString(), true, out moneda);

                return moneda;
            }
            set
            {
                Consecutivo = (int)value;
            }
        }
    }
}
