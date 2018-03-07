using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class ImagenesPerfilAdministradores : BaseEntity
    {
        public string UrlImagenPerfil
        {
            get
            {
                string urlImagen = string.Empty;

                if (CodigoArchivo > 0)
                {
                    urlImagen = URL.UrlBase + "Archivos/BuscarArchivo/" + CodigoArchivo + "/" + (int)TipoArchivo.Imagen;
                }

                return urlImagen;
            }
        }

        public byte[] ArchivoContenido { get; set; }
    }
}
