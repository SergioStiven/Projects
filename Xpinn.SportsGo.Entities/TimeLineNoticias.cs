using System;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;

namespace Xpinn.SportsGo.Entities
{
    public class TimeLineNoticias
    {
        public int ConsecutivoPublicacion { get; set; }
        public int ConsecutivoPersona { get; set; }
        public int ConsecutivoPerfil { get; set; }
        public int? CodigoArchivoImagenPerfil { get; set; }
        public int? CodigoArchivoPublicacion { get; set; }
        public int? CodigoTipoArchivoPublicacion { get; set; }

        public string NombrePublicante { get; set; }
        public string TituloPublicacion { get; set; }
        public string DescripcionPublicacion { get; set; }
        public string UrlRedireccionar { get; set; }
        public DateTime FechaPublicacion { get; set; }

        public bool NoticiaLateral { get; set; }

        public TipoPublicacionTimeLine TipoPublicacion { get; set; }
        public TipoArchivo TipoArchivoPublicacion
        {
            get
            {
                TipoArchivo archivoTipo = TipoArchivo.SinTipoArchivo;

                if (CodigoTipoArchivoPublicacion.HasValue)
                {
                    Enum.TryParse(CodigoTipoArchivoPublicacion.ToString(), true, out archivoTipo);
                }

                return archivoTipo;
            }
            set
            {
                CodigoTipoArchivoPublicacion = (int)value;
            }
        }

        public string UrlArchivoPublicacion
        {
            get
            {
                string urlArchivoPublicacion = string.Empty;

                if (CodigoArchivoPublicacion.HasValue && TipoArchivoPublicacion != TipoArchivo.SinTipoArchivo)
                {
                    urlArchivoPublicacion = ArchivosDTO.CrearUrlArchivo(TipoArchivoPublicacion, CodigoArchivoPublicacion.Value);
                }

                return urlArchivoPublicacion;
            }
        }

        public string UrlImagenPerfil
        {
            get
            {
                string urlImagenPerfil = string.Empty;

                if (CodigoArchivoImagenPerfil.HasValue)
                {
                    urlImagenPerfil = ArchivosDTO.CrearUrlArchivo(TipoArchivo.Imagen, CodigoArchivoImagenPerfil.Value);
                }

                return urlImagenPerfil;
            }
        }

        public bool TieneArchivo
        {
            get
            {
                return TipoArchivoPublicacion != TipoArchivo.SinTipoArchivo && !string.IsNullOrWhiteSpace(UrlArchivoPublicacion);
            }
        }

        public bool EsVideo
        {
            get
            {
                return TipoArchivoPublicacion == TipoArchivo.Video;
            }
        }

        public bool EsImagen
        {
            get
            {
                return TipoArchivoPublicacion == TipoArchivo.Imagen;
            }
        }

        public bool EsPublicidad
        {
            get
            {
                return TipoPublicacion == TipoPublicacionTimeLine.PorAnunciante;
            }
        }

        public bool EsNoticia
        {
            get
            {
                return TipoPublicacion == TipoPublicacionTimeLine.PorAdministrador;
            }
        }

        public bool EsEvento
        {
            get
            {
                return TipoPublicacion == TipoPublicacionTimeLine.PorGrupo;
            }
        }

        public bool SeMuestraNombre
        {
            get
            {
                return !string.IsNullOrWhiteSpace(NombrePublicante) && (EsEvento || EsPublicidad);
            }
        }

        public bool SeMuestraIconoInteraccion
        {
            get
            {
                return TipoPublicacion !=  TipoPublicacionTimeLine.SinTipoPublicacion && (!string.IsNullOrWhiteSpace(UrlRedireccionar) || TipoPublicacion == TipoPublicacionTimeLine.PorGrupo);
            }
        }

        public TimeLineNoticias()
        {

        }

        public TimeLineNoticias(NoticiasDTO noticia)
        {
            ConsecutivoPublicacion = noticia.Consecutivo;
            CodigoArchivoPublicacion = noticia.CodigoArchivo;
            TituloPublicacion = noticia.TituloIdiomaBuscado;
            DescripcionPublicacion = noticia.DescripcionIdiomaBuscado;
            FechaPublicacion = noticia.Creacion;
            CodigoTipoArchivoPublicacion = noticia.CodigoTipoArchivo;
            UrlRedireccionar = noticia.UrlPublicidad;

            CodigoArchivoImagenPerfil = noticia.CodigoArchivoImagenPerfilAdministrador;

            TipoPublicacion = TipoPublicacionTimeLine.PorAdministrador;
        }

        public TimeLineNoticias(GruposEventosDTO evento)
        {
            ConsecutivoPublicacion = evento.Consecutivo;
            CodigoArchivoPublicacion = evento.CodigoArchivo;
            TituloPublicacion = evento.Titulo;
            DescripcionPublicacion = evento.Descripcion;
            FechaPublicacion = evento.Creacion;
            CodigoTipoArchivoPublicacion = evento.CodigoTipoArchivo;

            ConsecutivoPerfil = evento.Grupos.Consecutivo;
            ConsecutivoPersona = evento.Grupos.Personas.Consecutivo;
            CodigoArchivoImagenPerfil = evento.Grupos.Personas.CodigoArchivoImagenPerfil;
            NombrePublicante = evento.Grupos.Personas.NombreYApellido;

            TipoPublicacion = TipoPublicacionTimeLine.PorGrupo;
        }

        public TimeLineNoticias(AnunciosDTO anuncio)
        {
            ConsecutivoPublicacion = anuncio.Consecutivo;
            CodigoArchivoPublicacion = anuncio.CodigoArchivo;
            TituloPublicacion = anuncio.TituloIdiomaBuscado;
            DescripcionPublicacion = anuncio.DescripcionIdiomaBuscado;
            FechaPublicacion = anuncio.Creacion;
            CodigoTipoArchivoPublicacion = anuncio.CodigoTipoArchivo;
            UrlRedireccionar = anuncio.UrlPublicidad;

            ConsecutivoPerfil = anuncio.Anunciantes.Consecutivo;
            ConsecutivoPersona = anuncio.Anunciantes.Personas.Consecutivo;
            CodigoArchivoImagenPerfil = anuncio.Anunciantes.Personas.CodigoArchivoImagenPerfil;
            NombrePublicante = anuncio.Anunciantes.Personas.NombreYApellido;

            NoticiaLateral = anuncio.NoticiaLateral;

            TipoPublicacion = TipoPublicacionTimeLine.PorAnunciante;
        }

        public static List<TimeLineNoticias> CrearListaTimeLine(ICollection<GruposEventosDTO> eventos, ICollection<AnunciosDTO> anuncios, ICollection<NoticiasDTO> noticias)
        {
            List<TimeLineNoticias> listaTimeLine = new List<TimeLineNoticias>();

            if (noticias != null && noticias.Count > 0)
            {
                foreach (NoticiasDTO noticia in noticias)
                {
                    listaTimeLine.Add(new TimeLineNoticias(noticia));
                }
            }

            if (eventos != null && eventos.Count > 0)
            {
                foreach (GruposEventosDTO evento in eventos)
                {
                    listaTimeLine.Add(new TimeLineNoticias(evento));
                }
            }

            if (anuncios != null && anuncios.Count > 0)
            {
                foreach (AnunciosDTO anuncio in anuncios)
                {
                    listaTimeLine.Add(new TimeLineNoticias(anuncio));
                }
            }

            return listaTimeLine;
        }
    }
}
