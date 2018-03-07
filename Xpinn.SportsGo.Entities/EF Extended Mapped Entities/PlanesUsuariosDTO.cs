using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public partial class PlanesUsuariosDTO : BaseEntity
    {
        public int CodigoPlanDeseado { get; set; }
        public int CodigoUsuario { get; set; }
    }
}
