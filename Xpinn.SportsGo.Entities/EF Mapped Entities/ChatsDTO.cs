//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xpinn.SportsGo.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChatsDTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChatsDTO()
        {
            this.ChatsMensajesEnvia = new HashSet<ChatsMensajesDTO>();
            this.ChatsMensajesRecibe = new HashSet<ChatsMensajesDTO>();
        }
    
        public int Consecutivo { get; set; }
        public int CodigoPersonaOwner { get; set; }
        public int CodigoPersonaNoOwner { get; set; }
        public int CodigoEstado { get; set; }
        public System.DateTime Creacion { get; set; }
    
        public virtual ChatsEstadoDTO ChatsEstado { get; set; }
        public virtual PersonasDTO PersonasNoOwner { get; set; }
        public virtual PersonasDTO PersonasOwner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatsMensajesDTO> ChatsMensajesEnvia { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatsMensajesDTO> ChatsMensajesRecibe { get; set; }
    }
}
