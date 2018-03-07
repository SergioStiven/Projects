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
    public partial class AnunciosDTO : BaseEntity
    {
        public string UrlArchivoAnuncio
        {
            get
            {
                string urlArchivo = string.Empty;

                if (CodigoArchivo > 0 && CodigoTipoArchivo > 0)
                {
                    urlArchivo = URL.UrlBase + "Archivos/BuscarArchivo/" + CodigoArchivo + "/" + CodigoTipoArchivo;
                }

                return urlArchivo;
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

        public TipoPublicacionNoticiasAnuncios TipoPublicacionAnuncio
        {
            get
            {
                TipoPublicacionNoticiasAnuncios tipoPublicacionAnuncio;

                Enum.TryParse(CodigoTipoAnuncio.ToString(), true, out tipoPublicacionAnuncio);

                return tipoPublicacionAnuncio;
            }
            set
            {
                CodigoTipoAnuncio = (int)value;
            }
        }

        int? _codigoTipoArchivo;
        public int? CodigoTipoArchivo {
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

        public string DescripcionIdiomaBuscado { get; set; }
        public bool NoticiaLateral { get; set; }
        public string TituloIdiomaBuscado { get; set; }
        
    }
}
