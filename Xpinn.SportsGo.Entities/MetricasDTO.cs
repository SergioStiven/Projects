using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public class MetricasDTO : BaseEntity
    {
        public int NumeroCandidatos { get; set; }
        public int NumeroGrupos { get; set; }
        public int NumeroRepresentantes { get; set; }
        public int NumeroAnunciantes { get; set; }
        public int NumeroAnuncios { get; set; }
        public int NumeroEventos { get; set; }
        public List<int> CategoriasParaBuscar { get; set; }
        public List<int> PaisesParaBuscar { get; set; }
        public List<int> IdiomasParaBuscar { get; set; }
        public List<int> PlanesParaBuscar { get; set; }
        public List<int> PerfilesParaBuscar { get; set; }
        public int CodigoAnunciante { get; set; }
        public string UsuarioParaBuscar { get; set; }
        public string NombrePersonaParaBuscar { get; set; }
        public string EmailParaBuscar { get; set; }
    }
}
