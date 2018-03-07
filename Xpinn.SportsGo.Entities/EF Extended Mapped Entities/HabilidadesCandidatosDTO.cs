using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Entities
{
    public partial class HabilidadesCandidatosDTO : BaseEntity
    {
        public HabilidadesCandidatosDTO(HabilidadesDTO habilidad, int numeroEstrellas, int codigoCategoriaCandidato)
        {
            CodigoHabilidad = habilidad.Consecutivo;
            Habilidades = habilidad;
            NumeroEstrellas = numeroEstrellas;
            CodigoCategoriaCandidato = codigoCategoriaCandidato;
        }

        public HabilidadesCandidatosDTO()
        {

        }
    }
}
