using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public partial class CandidatosVideosDTO : BaseEntity
    {
        public string UrlArchivo
        {
            get
            {
                string urlImagen = string.Empty;

                if (CodigoArchivo > 0)
                {
                    urlImagen = URL.UrlBase + "Archivos/BuscarArchivo/" + CodigoArchivo + "/" + (int)TipoArchivo.Video;
                }

                return urlImagen;
            }
        }

        public byte[] ArchivoContenido { get; set; }

    }
}
