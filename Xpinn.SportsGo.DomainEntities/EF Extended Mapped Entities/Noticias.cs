using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class Noticias : BaseEntity
    {
        public TipoArchivo TipoArchivo
        {
            get
            {
                return CodigoTipoArchivo.ToEnum<TipoArchivo>();
            }
            set
            {
                CodigoTipoArchivo = (int)value;
            }
        }

        public int CodigoTipoArchivo { get; set; }

        public string DescripcionIdiomaBuscado { get; set; }

        public string TituloIdiomaBuscado { get; set; }
    }
}
