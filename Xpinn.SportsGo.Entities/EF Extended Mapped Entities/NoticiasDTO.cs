using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Entities
{
    public partial class NoticiasDTO  : BaseEntity
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

        public TipoPublicacionNoticiasAnuncios TipoPublicacionNoticia
        {
            get
            {
                TipoPublicacionNoticiasAnuncios tipoPublicacionNoticia;

                Enum.TryParse(CodigoTipoNoticia.ToString(), true, out tipoPublicacionNoticia);

                return tipoPublicacionNoticia;
            }
            set
            {
                CodigoTipoNoticia = (int)value;
            }
        }

        public string UrlArchivo
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

        public int? CodigoArchivoImagenPerfilAdministrador { get; set; }

        public int CodigoTipoArchivo { get; set; }

        public string DescripcionIdiomaBuscado { get; set; }

        public string TituloIdiomaBuscado { get; set; }
    }
}
