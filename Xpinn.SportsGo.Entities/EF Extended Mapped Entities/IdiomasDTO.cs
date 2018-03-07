using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Entities
{
    public partial class IdiomasDTO
    {
        public Idioma Idioma
        {
            get
            {
                Idioma idioma;

                Enum.TryParse(Consecutivo.ToString(), true, out idioma);

                return idioma;
            }
            set
            {
                Consecutivo = (int)value;
            }
        }
    }
}