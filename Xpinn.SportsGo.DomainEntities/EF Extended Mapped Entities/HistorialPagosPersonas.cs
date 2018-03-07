using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class HistorialPagosPersonas : BaseEntity
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
    }
 
}
