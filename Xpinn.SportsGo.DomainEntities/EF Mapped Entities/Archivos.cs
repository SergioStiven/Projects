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
    
    public partial class Archivos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Archivos()
        {
            this.Paises = new HashSet<Paises>();
            this.PersonasBanner = new HashSet<Personas>();
            this.PersonasPerfil = new HashSet<Personas>();
            this.Planes = new HashSet<Planes>();
            this.Anuncios = new HashSet<Anuncios>();
            this.CandidatosVideos = new HashSet<CandidatosVideos>();
            this.Categorias = new HashSet<Categorias>();
            this.GruposEventos = new HashSet<GruposEventos>();
            this.Noticias = new HashSet<Noticias>();
            this.ImagenesPerfilAdministradores = new HashSet<ImagenesPerfilAdministradores>();
            this.HistorialPagosPersonas = new HashSet<HistorialPagosPersonas>();
        }
    
        public int Consecutivo { get; set; }
        public System.Guid RowGUID { get; set; }
        public int CodigoTipoArchivo { get; set; }
        public byte[] ArchivoContenido { get; set; }
    
        public virtual ArchivosTipos ArchivosTipos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Paises> Paises { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personas> PersonasBanner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personas> PersonasPerfil { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Planes> Planes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Anuncios> Anuncios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidatosVideos> CandidatosVideos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Categorias> Categorias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GruposEventos> GruposEventos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Noticias> Noticias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImagenesPerfilAdministradores> ImagenesPerfilAdministradores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistorialPagosPersonas> HistorialPagosPersonas { get; set; }
    }
}