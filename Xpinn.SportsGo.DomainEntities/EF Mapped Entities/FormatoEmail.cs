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
    
    public partial class FormatoEmail
    {
        public int Consecutivo { get; set; }
        public string TextoHtml { get; set; }
        public int CodigoIdioma { get; set; }
        public int CodigoTipoFormato { get; set; }
    
        public virtual Idiomas Idiomas { get; set; }
        public virtual TiposFormatosEmail TiposFormatosEmail { get; set; }
    }
}
