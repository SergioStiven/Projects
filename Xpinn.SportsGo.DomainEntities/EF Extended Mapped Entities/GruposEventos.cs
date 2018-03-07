using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class GruposEventos : BaseEntity
    {
        public Idioma Idioma
        {
            get
            {
                Idioma result;
                return Enum.TryParse(CodigoIdioma.ToString(), true, out result) ? result : default(Idioma);
            }
            set
            {
                CodigoIdioma = (int)value;
            }
        }

        public int? CodigoTipoArchivo { get; set; }
    }
}
