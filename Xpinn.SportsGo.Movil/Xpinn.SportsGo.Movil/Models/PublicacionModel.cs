using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    [AddINotifyPropertyChangedInterface]
    class PublicacionModel
    {
        public CategoriasEventosDTO CategoriaDelEvento { get; set; }
        public IdiomaModel IdiomaDelEvento { get; set; }
        public PaisesDTO PaisDelEvento { get; set; }
        public PersonasDTO PersonaDeLaPublicacion { get; set; }

        public bool EsMiPersona
        {
            get
            {
                return PersonaDeLaPublicacion != null && PersonaDeLaPublicacion.Consecutivo > 0 && App.Persona != null && App.Persona.Consecutivo == PersonaDeLaPublicacion.Consecutivo;
            }
        }

        string _urlArchivo;
        public string UrlArchivo
        {
            get
            {
                string urlRetornar = string.Empty;

                if (!string.IsNullOrWhiteSpace(_urlArchivo))
                {
                    urlRetornar = _urlArchivo;
                }

                return urlRetornar;
            }
            set
            {
                _urlArchivo = value;
            }
        }
        public int? CodigoArchivo { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        public DateTime Creacion { get; set; }
        public int CodigoPerfil { get; set; }
        public int CodigoPublicacion { get; set; }

        public DateTime FechaInicio { get; set; }
        public string FechaInicioString
        {
            get
            {
                return FechaInicio.ToString("d");
            }
        }

        public DateTime FechaTerminacion { get; set; }
        public string FechaTerminacionString
        {
            get
            {
                return FechaTerminacion.ToString("d");
            }
        }

        public string Ubicacion { get; set; }

        public bool FueBorrado { get; set; }
        public TipoPerfil TipoPerfil { get; set; }
        public TipoArchivo TipoArchivoPublicacion { get; set; }

        public bool EsVideo
        {
            get
            {
                return TipoArchivoPublicacion == TipoArchivo.Video;
            }
            set
            {
                if (value == true)
                {
                    TipoArchivoPublicacion = TipoArchivo.Video;
                }
                else
                {
                    TipoArchivoPublicacion = TipoArchivo.Imagen;
                }
            }
        }
    }
}
