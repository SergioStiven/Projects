using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class Chats : BaseEntity
    {
        public EstadosChat EstadoChat
        {
            get
            {
                return CodigoEstado.ToEnum<EstadosChat>();
            }
            set
            {
                CodigoEstado = (int)value;
            }
        }

        public int CodigoChatRecibe { get; set; }

    }
}
