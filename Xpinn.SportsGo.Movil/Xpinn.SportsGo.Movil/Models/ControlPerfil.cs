using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    class ControlPerfil
    {
        public CategoriasModel CategoriaSeleccionada { get; set; }
        public TipoPerfil TipoPerfilControl { get; set; }
        public PersonasDTO PersonaParaVer { get; set; }
        public IEnumerable<int> CategoriasQueYaEstanAgregadas { get; set; }
    }
}
