using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Movil.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ImagenEditorModel
    {
        public string Source { get; set; }
        public int CodigoArchivoCreado { get; set; }

        public PersonasDTO Persona { get; set; }
        public bool EsImagenBanner { get; set; }
        public bool EsImagenPerfil { get; set; }
        public bool EsPrimerRegistro { get; set; }

        public int CodigoEvento { get; set; }
        public bool EsEvento { get; set; }
    }
}
