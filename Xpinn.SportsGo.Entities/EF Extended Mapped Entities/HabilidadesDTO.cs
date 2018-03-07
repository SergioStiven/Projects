using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public partial class HabilidadesDTO : BaseEntity
    {
        public TipoHabilidad TipoHabilidad
        {
            get
            {
                TipoHabilidad result;
                return Enum.TryParse(CodigoTipoHabilidad.ToString(), true, out result) ? result : default(TipoHabilidad);
            }
            set
            {
                CodigoTipoHabilidad = (int)value;
            }
        }

        public string DescripcionIdiomaBuscado { get; set; }

    }
}
