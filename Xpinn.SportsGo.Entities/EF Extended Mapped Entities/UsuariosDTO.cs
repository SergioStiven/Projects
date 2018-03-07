using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Newtonsoft.Json;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Entities
{
    public partial class UsuariosDTO : BaseEntity
    {
        public bool EsRecuperarClave { get; set; }
        public bool EsRecordarClave { get; set; }

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

        public int? CodigoImagenPerfilAdmin { get; set; }

        public string UrlImagenPerfilAdmin
        {
            get
            {
                string urlImagen = string.Empty;

                if (CodigoImagenPerfilAdmin.HasValue && CodigoImagenPerfilAdmin > 0)
                {
                    urlImagen = URL.UrlBase + "Archivos/BuscarArchivo/" + CodigoImagenPerfilAdmin.Value + "/" + (int)TipoArchivo.Imagen;
                }

                return urlImagen;
            }
        }

        [JsonIgnore]
        public PersonasDTO PersonaDelUsuario
        {
            get
            {
                return Personas.ElementAtOrDefault(0);
            }
            set
            {
                Personas = new List<PersonasDTO> { value };
            }
        }
    }
}
