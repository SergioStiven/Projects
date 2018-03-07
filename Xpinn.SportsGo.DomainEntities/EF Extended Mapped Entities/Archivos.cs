using System;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class Archivos
    {
        public TipoArchivo TipoArchivo
        {
            get
            {
                TipoArchivo archivoTipo;

                Enum.TryParse(CodigoTipoArchivo.ToString(), true, out archivoTipo);

                return archivoTipo;
            }
            set
            {
                CodigoTipoArchivo = (int)value;
            }
        }
    }
}
