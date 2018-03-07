using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;
using Newtonsoft.Json;

namespace Xpinn.SportsGo.Entities
{
    public partial class PersonasDTO : BaseEntity
    {
        public int ConsecutivoViendoPersona { get; set; }
        public int ConsecutivoContacto { get; set; }
        public bool YaEstaAgregadaContactos { get; set; }
        public bool PersonaRecordandose { get; set; }

        public TipoPerfil TipoPerfil
        {
            get
            {
                TipoPerfil result;
                return Enum.TryParse(CodigoTipoPerfil.ToString(), true, out result) ? result : default(TipoPerfil);
            }
            set
            {
                CodigoTipoPerfil = (int)value;
            }
        }

        public Idioma IdiomaDeLaPersona
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

        public string UrlImagenPerfil
        {
            get
            {
                string urlImagen = string.Empty;

                if (CodigoArchivoImagenPerfil.HasValue && CodigoArchivoImagenPerfil > 0)
                {
                    urlImagen = URL.UrlBase + "Archivos/BuscarArchivo/" + CodigoArchivoImagenPerfil + "/" + (int)TipoArchivo.Imagen;
                }

                return urlImagen;
            }
        }

        public string UrlImagenBanner
        {
            get
            {
                string urlImagen = string.Empty;

                if (CodigoArchivoImagenBanner.HasValue && CodigoArchivoImagenBanner > 0)
                {
                    urlImagen = URL.UrlBase + "Archivos/BuscarArchivo/" + CodigoArchivoImagenBanner + "/" + (int)TipoArchivo.Imagen;
                }
                
                return urlImagen;
            }
        }

        public string NombreYApellido
        {
            get
            {
                return Nombres + " " + Apellidos;
            }
        }

        [JsonIgnore]
        public CandidatosDTO CandidatoDeLaPersona
        {
            get
            {
                return Candidatos.ElementAtOrDefault(0);
            }
            set
            {
                Candidatos = new List<CandidatosDTO> { value };
            }
        }

        [JsonIgnore]
        public GruposDTO GrupoDeLaPersona
        {
            get
            {
                return Grupos.ElementAtOrDefault(0);
            }
            set
            {
                Grupos = new List<GruposDTO> { value };
            }
        }

        [JsonIgnore]
        public RepresentantesDTO RepresentanteDeLaPersona
        {
            get
            {
                return Representantes.ElementAtOrDefault(0);
            }
            set
            {
                Representantes = new List<RepresentantesDTO> { value };
            }
        }

        [JsonIgnore]
        public AnunciantesDTO AnuncianteDeLaPersona
        {
            get
            {
                return Anunciantes.ElementAtOrDefault(0);
            }
            set
            {
                Anunciantes = new List<AnunciantesDTO> { value };
            }
        }
    }
}
