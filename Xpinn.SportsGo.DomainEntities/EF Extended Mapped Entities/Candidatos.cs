using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.HelperClasses;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class Candidatos : BaseEntity
    {
        public TipoGeneros TipoGenero
        {
            get
            {
                return CodigoGenero.ToEnum<TipoGeneros>();
            }
            set
            {
                CodigoGenero = (int)value;
            }
        }
    }
}
