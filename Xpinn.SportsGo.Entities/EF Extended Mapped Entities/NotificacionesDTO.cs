using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Entities
{
    public partial class NotificacionesDTO : BaseEntity
    {
        public TipoNotificacionEnum TipoDeLaNotificacion
        {
            get
            {
                TipoNotificacionEnum tipoNotificacion;

                Enum.TryParse(CodigoTipoNotificacion.ToString(), true, out tipoNotificacion);

                return tipoNotificacion;
            }
            set
            {
                CodigoTipoNotificacion = (int)value;
            }
        }
    }
}
