using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Entities
{
    public partial class PlanesDTO : BaseEntity
    {
        public string UrlArchivo
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

        public TipoPerfil TipoPerfil
        {
            get
            {
                TipoPerfil result;
                return Enum.TryParse(CodigoTipoPerfil.ToString(), true, out result) ? result : default(TipoPerfil);
            }
            set
            {
                CodigoTipoPerfil = (int)value;
            }
        }

        public string DescripcionIdiomaBuscado { get; set; }

        public byte[] ArchivoContenido { get; set; }

        public int CodigoPaisParaBuscarMoneda { get; set; }

        public decimal PrecioEnMonedaFantasma { get; set; }

    }
}
