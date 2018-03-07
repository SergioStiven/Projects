using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class CandidatosVideos : BaseEntity
    {
        public byte[] ArchivoContenido { get; set; }
    }
}