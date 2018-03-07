using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class TerminosCondiciones : BaseEntity
    {
        public Idioma IdiomaDeLosTerminos
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
    }
}
