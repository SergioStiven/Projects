using System;
using System.Collections.Generic;
using System.Linq;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    class BiografiaTimeLineModel
    {
        public ContactosDTO PrimerContacto { get; set; }
        public ContactosDTO SegundoContacto { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string FechaCreacion { get; set; }
        public DateTime DateTimeCreacion { get; set; }
        public string UrlArchivo { get; set; }
        public int CodigoArchivo { get; set; }
        public int CodigoPublicacion { get; set; }
        public TipoArchivo TipoArchivoTimeLine { get; set; }
        public TipoItemTimeLine TipoTimeLine { get; set; }

        public bool TengoPrimerContacto
        {
            get
            {
                return PrimerContacto != null;
            }
        }

        public bool TengoSegundoContacto
        {
            get
            {
                return SegundoContacto != null;
            }
        }

        public bool DebeMostrarImagenPublicacion
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UrlArchivo);
            }
        }

        public bool EsVideo
        {
            get
            {
                return TipoArchivoTimeLine == TipoArchivo.Video;
            }
        }

        public bool EsImagen
        {
            get
            {
                return !EsVideo;
            }
        }

        public BiografiaTimeLineModel(PublicacionModel publicacion)
        {
            Titulo = publicacion.Titulo;
            Descripcion = publicacion.Descripcion;
            DateTimeCreacion = publicacion.Creacion;
            FechaCreacion = publicacion.Creacion.ToString("d");
            UrlArchivo = publicacion.UrlArchivo;
            CodigoArchivo = publicacion.CodigoArchivo.HasValue ? publicacion.CodigoArchivo.Value : 0;
            CodigoPublicacion = publicacion.CodigoPublicacion;
            TipoArchivoTimeLine = publicacion.TipoArchivoPublicacion;

            TipoTimeLine = TipoItemTimeLine.Publicacion;
        }

        public BiografiaTimeLineModel(CandidatosVideosDTO candidatoVideo)
        {
            Titulo = candidatoVideo.Titulo;
            Descripcion = candidatoVideo.Descripcion;
            DateTimeCreacion = candidatoVideo.Creacion;
            FechaCreacion = candidatoVideo.Creacion.ToString("d");
            UrlArchivo = candidatoVideo.UrlArchivo;
            CodigoArchivo = candidatoVideo.CodigoArchivo;
            CodigoPublicacion = candidatoVideo.Consecutivo;
            TipoArchivoTimeLine = TipoArchivo.Video;

            TipoTimeLine = TipoItemTimeLine.Publicacion;
        }

        public BiografiaTimeLineModel(GruposEventosDTO grupoEventos)
        {
            Titulo = grupoEventos.Titulo;
            Descripcion = grupoEventos.Descripcion;
            DateTimeCreacion = grupoEventos.Creacion;
            FechaCreacion = grupoEventos.Creacion.ToString("d");
            UrlArchivo = grupoEventos.UrlArchivo;
            CodigoArchivo = grupoEventos.CodigoArchivo.HasValue ? grupoEventos.CodigoArchivo.Value : 0;
            CodigoPublicacion = grupoEventos.Consecutivo;
            TipoArchivoTimeLine = grupoEventos.TipoArchivo;

            TipoTimeLine = TipoItemTimeLine.Publicacion;
        }

        public BiografiaTimeLineModel(ContactosDTO primerContacto, ContactosDTO segundoContacto)
        {
            PrimerContacto = primerContacto;
            SegundoContacto = segundoContacto;

            TipoTimeLine = TipoItemTimeLine.Contacto;
        }

        public static List<BiografiaTimeLineModel> CrearListaBiografiaTimeLine(ICollection<CandidatosVideosDTO> candidatosVideos)
        {
            List<BiografiaTimeLineModel> listaCandidatosVideos = new List<BiografiaTimeLineModel>();

            if (candidatosVideos != null && candidatosVideos.Count > 0)
            {
                foreach (var video in candidatosVideos)
                {
                    listaCandidatosVideos.Add(new BiografiaTimeLineModel(video));
                }
            }

            return listaCandidatosVideos;
        }

        public static List<BiografiaTimeLineModel> CrearListaBiografiaTimeLine(ICollection<GruposEventosDTO> gruposEventos)
        {
            List<BiografiaTimeLineModel> listaCandidatosVideos = new List<BiografiaTimeLineModel>();

            if (gruposEventos != null && gruposEventos.Count > 0)
            {
                foreach (var evento in gruposEventos)
                {
                    listaCandidatosVideos.Add(new BiografiaTimeLineModel(evento));
                }
            }

            return listaCandidatosVideos;
        }

        public static List<BiografiaTimeLineModel> CrearListaBiografiaTimeLine(ICollection<ContactosDTO> contactos)
        {
            List<BiografiaTimeLineModel> listaContactos = new List<BiografiaTimeLineModel>();

            if (contactos != null && contactos.Count > 0)
            {
                ContactosDTO contactoPrimero = null;

                foreach (var contacto in contactos)
                {
                    if (contactoPrimero == null)
                    {
                        contactoPrimero = contacto;
                    }
                    else
                    {
                        listaContactos.Add(new BiografiaTimeLineModel(contactoPrimero, contacto));
                        contactoPrimero = null;
                    }
                }

                if (contactoPrimero != null && !listaContactos.Any(x => x.SegundoContacto == contactoPrimero))
                {
                    listaContactos.Add(new BiografiaTimeLineModel(contactoPrimero, null));
                }
            }

            return listaContactos;
        }
    }
}