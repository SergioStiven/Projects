using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public partial class ChatsMensajesDTO : BaseEntity
    {
        public bool UltimoMensajeEsMio { get; set; }

        string _mensajeParaMostrar;
        public string MensajeParaMostrar
        {
            get
            {
                string mensaje = string.Empty;

                if (!string.IsNullOrWhiteSpace(_mensajeParaMostrar))
                {
                    mensaje = _mensajeParaMostrar;
                }
                else
                {
                    mensaje = Mensaje;
                }

                return mensaje;
            }
            set
            {
                _mensajeParaMostrar = value;
            }
        }
    }
}