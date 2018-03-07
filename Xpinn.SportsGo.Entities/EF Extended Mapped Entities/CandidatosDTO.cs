using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public partial class CandidatosDTO : BaseEntity
    {
        public TipoGeneros TipoGenero
        {
            get
            {
                TipoGeneros result;
                return Enum.TryParse(CodigoGenero.ToString(), true, out result) ? result : default(TipoGeneros);
            }
            set
            {
                CodigoGenero = (int)value;
            }
        }
    }
}
