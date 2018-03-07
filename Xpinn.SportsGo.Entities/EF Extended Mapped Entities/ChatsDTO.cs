using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Entities
{
    public partial class ChatsDTO : BaseEntity
    {
        public EstadosChat EstadoChat
        {
            get
            {
                EstadosChat estadoChat;

                Enum.TryParse(CodigoEstado.ToString(), true, out estadoChat);

                return estadoChat;
            }
            set
            {
                CodigoEstado = (int)value;
            }
        }

        public int CodigoChatRecibe { get; set; }

        public bool EsNuevoMensaje { get; set; }

        public int NumeroMensajesNuevos { get; set; }

        public ChatsMensajesDTO UltimoMensaje { get; set; }

    }
}
