using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Entities
{
    public class TimeLineNotificaciones
    {
        public int ConsecutivoNotificacion { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string UrlPublicidad { get; set; }
        public bool EsNuevoMensaje { get; set; }
        public DateTime CreacionNotificacion { get; set; }

        string _urlArchivo;

        public string UrlArchivo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_urlArchivo))
                {
                    return string.Empty;
                }
                else
                {
                    return _urlArchivo;
                }
            }
            set
            {
                _urlArchivo = value;
            }
        }

        public string TituloEvento { get; set; }
        public DateTime FechaInicioEvento { get; set; }
        public DateTime FechaTerminacionEvento { get; set; }

        public int CodigoArchivo { get; set; }

        public int CodigoPersonaOrigen { get; set; }
        public int CodigoPersonaDestino { get; set; }
        public string NombreApellidoPersona { get; set; }

        public int CodigoPlanNuevo { get; set; }
        public DateTime FechaVencimientoPlan { get; set; }
        public string DescripcionPlan { get; set; }

        public int CodigoTipoNotificacion { get; set; }

        public TipoNotificacionEnum TipoDeLaNotificacion
        {
            get
            {
                TipoNotificacionEnum tipoNotificacion;

                Enum.TryParse(CodigoTipoNotificacion.ToString(), true, out tipoNotificacion);

                return tipoNotificacion;
            }
            set
            {
                CodigoTipoNotificacion = (int)value;
            }
        }

        public bool RedireccionaPersona
        {
            get
            {
                return (TipoDeLaNotificacion == TipoNotificacionEnum.PersonaAgregada || TipoDeLaNotificacion == TipoNotificacionEnum.PersonaEliminada || TipoDeLaNotificacion == TipoNotificacionEnum.InscripcionEventoUsuario || TipoDeLaNotificacion == TipoNotificacionEnum.DesuscripcionEventoUsuario) && CodigoPersonaOrigen > 0;
            }
        }

        public bool RedireccionaUrlPublicidadSiHay
        {
            get
            {
                return TipoDeLaNotificacion == TipoNotificacionEnum.PublicacionAdmin && !string.IsNullOrWhiteSpace(UrlPublicidad);
            }
        }

        public bool RedireccionaUrlFeedSiHay
        {
            get
            {
                return TipoDeLaNotificacion == TipoNotificacionEnum.RssFeed && !string.IsNullOrWhiteSpace(UrlPublicidad);
            }
        }

        public TimeLineNotificaciones()
        {

        }

        public TimeLineNotificaciones(NoticiasDTO noticia)
        {
            Titulo = noticia.TituloIdiomaBuscado;
            Descripcion = noticia.DescripcionIdiomaBuscado;

            CodigoArchivo = noticia.CodigoArchivo.HasValue ? noticia.CodigoArchivo.Value : 0;
            UrlArchivo = noticia.UrlArchivo;
            UrlPublicidad = noticia.UrlPublicidad;

            CreacionNotificacion = noticia.Creacion;

            TipoDeLaNotificacion = TipoNotificacionEnum.PublicacionAdmin;
        }

        public TimeLineNotificaciones(string title, string summary, string uri, string uriImagen, TipoNotificacionEnum tipoNotificacion)
        {
            Titulo = title;
            Descripcion = summary;
            UrlPublicidad = uri;
            UrlArchivo = uriImagen;

            TipoDeLaNotificacion = tipoNotificacion;
        }

        public TimeLineNotificaciones(NotificacionesDTO notificacion)
        {
            if (notificacion.TipoDeLaNotificacion == TipoNotificacionEnum.PersonaAgregada || notificacion.TipoDeLaNotificacion == TipoNotificacionEnum.PersonaEliminada)
            {
                CodigoArchivo = notificacion.PersonasOrigenAccion.CodigoArchivoImagenPerfil.HasValue ? notificacion.PersonasOrigenAccion.CodigoArchivoImagenPerfil.Value : 0;
                UrlArchivo = notificacion.PersonasOrigenAccion.UrlImagenPerfil;
                NombreApellidoPersona = notificacion.PersonasOrigenAccion.NombreYApellido;
            }
            else if (notificacion.TipoDeLaNotificacion == TipoNotificacionEnum.NuevoPlan || notificacion.TipoDeLaNotificacion == TipoNotificacionEnum.PlanAprobado || notificacion.TipoDeLaNotificacion == TipoNotificacionEnum.PlanRechazado)
            {
                CodigoArchivo = notificacion.Planes.CodigoArchivo;
                UrlArchivo = notificacion.Planes.UrlArchivo;
                DescripcionPlan = notificacion.Planes.DescripcionIdiomaBuscado;
            }
            else if (notificacion.TipoDeLaNotificacion == TipoNotificacionEnum.InscripcionEventoUsuario || notificacion.TipoDeLaNotificacion == TipoNotificacionEnum.DesuscripcionEventoUsuario)
            {
                CodigoArchivo = notificacion.PersonasOrigenAccion.CodigoArchivoImagenPerfil.HasValue ? notificacion.PersonasOrigenAccion.CodigoArchivoImagenPerfil.Value : 0;
                UrlArchivo = notificacion.PersonasOrigenAccion.UrlImagenPerfil;
                NombreApellidoPersona = notificacion.PersonasOrigenAccion.NombreYApellido;
                FechaInicioEvento = notificacion.GruposEventos.FechaInicio;
                FechaTerminacionEvento = notificacion.GruposEventos.FechaTerminacion;
                TituloEvento = notificacion.GruposEventos.Titulo;
            }

            CreacionNotificacion = notificacion.Creacion;
            CodigoPersonaDestino = notificacion.CodigoPersonaDestinoAccion.HasValue ? notificacion.CodigoPersonaDestinoAccion.Value : 0;
            CodigoPersonaOrigen = notificacion.CodigoPersonaOrigenAccion.HasValue ? notificacion.CodigoPersonaOrigenAccion.Value : 0;
            CodigoPlanNuevo = notificacion.CodigoPlanNuevo.HasValue ? notificacion.CodigoPlanNuevo.Value : 0;
            ConsecutivoNotificacion = notificacion.Consecutivo;

            TipoDeLaNotificacion = notificacion.TipoDeLaNotificacion;
        }

        public static List<TimeLineNotificaciones> CrearListaTimeLineNotificaciones(ICollection<NoticiasDTO> noticias, ICollection<NotificacionesDTO> notificaciones)
        {
            List<TimeLineNotificaciones> listaTimeLineNotificaciones = new List<TimeLineNotificaciones>();

            if (notificaciones != null && notificaciones.Count > 0)
            {
                foreach (NotificacionesDTO notificacion in notificaciones.OrderBy(x => x.CodigoTipoNotificacion == 8 || x.CodigoTipoNotificacion == 7))
                {
                    listaTimeLineNotificaciones.Add(new TimeLineNotificaciones(notificacion));
                }
            }

            if (noticias != null && noticias.Count > 0)
            {
                foreach (NoticiasDTO noticia in noticias)
                {
                    listaTimeLineNotificaciones.Add(new TimeLineNotificaciones(noticia));
                }
            }

            return listaTimeLineNotificaciones;
        }
    }
}
