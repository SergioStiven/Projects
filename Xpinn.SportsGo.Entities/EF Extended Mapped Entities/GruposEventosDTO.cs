using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public partial class GruposEventosDTO : BaseEntity
    {
        public string UrlArchivo
        {
            get
            {
                string urlImagen = string.Empty;

                if (CodigoArchivo > 0 && CodigoTipoArchivo > 0)
                {
                    urlImagen = URL.UrlBase + "Archivos/BuscarArchivo/" + CodigoArchivo + "/" + CodigoTipoArchivo;
                }

                return urlImagen;
            }
        }

        public Idioma Idioma
        {
            get
            {
                Idioma result;
                return Enum.TryParse(CodigoIdioma.ToString(), true, out result) ? result : default(Idioma);
            }
            set
            {
                CodigoIdioma = (int)value;
            }
        }

        public TipoArchivo TipoArchivo 
        {
            get
            {
                TipoArchivo result;
                return Enum.TryParse(CodigoTipoArchivo.ToString(), true, out result) ? result : default(TipoArchivo);
            }
            set
            {
                CodigoTipoArchivo = (int)value;
            }
        }

        int? _codigoTipoArchivo;
        public int? CodigoTipoArchivo
        {
            get
            {
                if (Archivos != null && Archivos.CodigoTipoArchivo > 0)
                {
                    return Archivos.CodigoTipoArchivo;
                }
                else
                {
                    return _codigoTipoArchivo;
                }
            }
            set
            {
                if (Archivos != null && value.HasValue)
                {
                    Archivos.CodigoTipoArchivo = value.Value;
                }

                _codigoTipoArchivo = value;
            }
        }

        public int NumeroEventosAsistentes { get; set; }
    }
}
