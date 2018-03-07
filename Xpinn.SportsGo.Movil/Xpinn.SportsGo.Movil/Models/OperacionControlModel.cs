using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    class OperacionControlModel
    {
        public TipoOperacion TipoOperacion { get; set; }
        public bool EsPrimerRegistro { get; set; }

        public OperacionControlModel(TipoOperacion tipoOperacion, bool esPrimerRegistro = false)
        {
            TipoOperacion = tipoOperacion;
            EsPrimerRegistro = esPrimerRegistro;
        }
    }
}
