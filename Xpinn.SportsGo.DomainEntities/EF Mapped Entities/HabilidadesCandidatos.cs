//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xpinn.SportsGo.DomainEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class HabilidadesCandidatos
    {
        public int Consecutivo { get; set; }
        public int CodigoHabilidad { get; set; }
        public int CodigoCategoriaCandidato { get; set; }
        public int NumeroEstrellas { get; set; }
    
        public virtual CategoriasCandidatos CategoriasCandidatos { get; set; }
        public virtual Habilidades Habilidades { get; set; }
    }
}
