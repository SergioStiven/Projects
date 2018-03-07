namespace Xpinn.SportsGo.Util.Portable.Enums
{
    public enum TipoPerfil
    {
        SinTipoPerfil = 0,
        Candidato = 1,
        Grupo = 2,
        Representante = 3,
        Anunciante = 4,
        Administrador = 5
    }

    public enum TipoArchivo
    {
        SinTipoArchivo = 0,
        Imagen = 1,
        Video = 2
    }

    public enum Idioma
    {
        SinIdioma = 0,
        Español = 1,
        Ingles = 2,
        Portugues = 3
    }

    public enum FormatoImagen
    {
        SinTipoFormato,
        Png,
        Jpeg
    }

    public enum TipoHabilidad
    {
        SinTipoHabilidad = 0,
        Tecnica = 1,
        Tactica = 2,
        Fisica = 3
    }

    public enum SiNoEnum
    {
        No = 0,
        Si = 1
    }

    public enum TipoOperacion
    {
        SinOperacion = 0,
        VideosPerfil = 1,
        ServiciosChat = 2,
        ConsultaCandidatos = 3,
        DetalleCandidatos = 4,
        ConsultaGrupos = 5,
        DetalleGrupos = 6,
        ConsultaEventos = 7,
        CreacionAnuncios = 8,
        EstadisticasAnuncios = 9,
        MultiplesCategorias = 10
    }

    public enum TipoGeneros
    {
        SinGenero = 0,
        Hombre = 1,
        Mujer = 2
    }

    public enum EstadosChat
    {
        SinEstado = 0,
        Activo = 1,
        PendienteParaBorrarMensajes = 2
    }

    public enum TipoPublicacionTimeLine
    {
        SinTipoPublicacion = 0,
        PorAdministrador = 1,
        PorAnunciante = 2,
        PorGrupo = 3
    }

    public enum TipoItemTimeLine
    {
        SinTipoItemTimeLine = 0,
        Publicacion = 1,
        Contacto = 2
    }

    public enum TipoPublicacionNoticiasAnuncios
    {
        SinTipoPublicacion = 0,
        TimeLine = 1,
        NotificacionOAnuncioLateral = 2
    }

    public enum TipoNotificacionEnum
    {
        SinTipoNotificacion = 0,
        NuevoPlan = 1,
        PersonaAgregada = 2,
        PersonaEliminada = 3,
        PublicacionAdmin = 4,
        EstaPorVencersePlan = 5,
        SeVencioPlan = 6,
        PlanRechazado = 7,
        PlanAprobado = 8,
        InscripcionEventoUsuario = 9,
        DesuscripcionEventoUsuario = 10,
        RssFeed = 11
    }

    public enum EstadoDeLosPagos
    {
        SinEstadoDelPago = 0,
        EsperaPago = 1,
        PendientePorAprobar = 2,
        Rechazado = 3,
        Aprobado = 4
    }

    public enum MonedasEnum
    {
        SinMoneda = 0,
        Dolar = 1,
        PesosColombianos = 2,
        PesoArgentino = 3,
        RealBrasileño = 4,
        PesoChileno = 5,
        PesoMexicano = 6,
        NuevoSol = 7
    }

    public enum TipoFormatosEnum
    {
        SinTipoFormatos = 0,
        RecuperacionClave = 1,
        ConfirmacionCuenta = 2
    }
}
