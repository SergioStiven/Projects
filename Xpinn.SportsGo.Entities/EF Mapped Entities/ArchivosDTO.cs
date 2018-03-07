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
    
    public partial class ArchivosDTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ArchivosDTO()
        {
            this.Paises = new HashSet<PaisesDTO>();
            this.PersonasBanner = new HashSet<PersonasDTO>();
            this.PersonasPerfil = new HashSet<PersonasDTO>();
            this.Planes = new HashSet<PlanesDTO>();
            this.Anuncios = new HashSet<AnunciosDTO>();
            this.CandidatosVideos = new HashSet<CandidatosVideosDTO>();
            this.Categorias = new HashSet<CategoriasDTO>();
            this.GruposEventos = new HashSet<GruposEventosDTO>();
            this.Noticias = new HashSet<NoticiasDTO>();
            this.ImagenesPerfilAdministradores = new HashSet<ImagenesPerfilAdministradoresDTO>();
            this.HistorialPagosPersonas = new HashSet<HistorialPagosPersonasDTO>();
        }

        public int Consecutivo { get; set; }
        public System.Guid RowGUID { get; set; }
        public int CodigoTipoArchivo { get; set; }
        public byte[] ArchivoContenido { get; set; }

        public virtual ArchivosTiposDTO ArchivosTipos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaisesDTO> Paises { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonasDTO> PersonasBanner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonasDTO> PersonasPerfil { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlanesDTO> Planes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnunciosDTO> Anuncios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidatosVideosDTO> CandidatosVideos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategoriasDTO> Categorias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GruposEventosDTO> GruposEventos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoticiasDTO> Noticias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImagenesPerfilAdministradoresDTO> ImagenesPerfilAdministradores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistorialPagosPersonasDTO> HistorialPagosPersonas { get; set; }
    }
}