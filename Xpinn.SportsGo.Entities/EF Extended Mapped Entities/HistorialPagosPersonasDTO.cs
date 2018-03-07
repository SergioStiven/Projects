using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Entities
{
    public partial class HistorialPagosPersonasDTO : BaseEntity
    {
        public bool EsPagoPorPayU { get; set; }

        public EstadoDeLosPagos EstadoDelPago
        {
            get
            {
                EstadoDeLosPagos result;
                return Enum.TryParse(CodigoEstado.ToString(), true, out result) ? result : default(EstadoDeLosPagos);
            }
            set
            {
                CodigoEstado = (int)value;
            }
        }

        public string UrlImagen
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
    }
}
