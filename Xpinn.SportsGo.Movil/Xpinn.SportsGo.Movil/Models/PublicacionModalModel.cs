using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    class PublicacionModalModel
    {
        public string UrlArchivo { get; set; }
        public string UrlRedireccionar { get; set; }
        public int CodigoArchivo { get; set; }
        public TipoArchivo TipoArchivoPublicacion { get; set; }
    }
}
