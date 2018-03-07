using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Movil.Models
{
    class PosicionJugadorModel
    {
        public CategoriasModel CategoriaParaUbicar { get; set; }
        public bool EsRegistroCategoria { get; set; }
        public bool EsMiPersonaORegistro { get; set; }
    }
}
