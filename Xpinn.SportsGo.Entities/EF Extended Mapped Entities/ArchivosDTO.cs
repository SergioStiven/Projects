using System;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Entities
{
    public partial class ArchivosDTO
    {
        public TipoArchivo ArchivoTipo
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

        public static string CrearUrlArchivo(TipoArchivo tipoArchivo, int codigoArchivo)
        {
            string urlArchivo = string.Empty;

            if (tipoArchivo != TipoArchivo.SinTipoArchivo && codigoArchivo > 0)
            {
                urlArchivo = URL.UrlBase + "Archivos/BuscarArchivo/" + codigoArchivo + "/" + (int)tipoArchivo;
            }

            return urlArchivo;
        }
    }
}
