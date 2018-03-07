using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class Habilidades : BaseEntity
    {
        public TipoHabilidad TipoHabilidad
        {
            get
            {
                return CodigoTipoHabilidad.ToEnum<TipoHabilidad>();
            }
            set
            {
                CodigoTipoHabilidad = (int)value;
            }
        }
    }
}
