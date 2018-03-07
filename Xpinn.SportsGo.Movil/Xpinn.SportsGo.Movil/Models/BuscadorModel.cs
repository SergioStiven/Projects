using System;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Movil.Models
{
    enum TipoBusqueda
    {
        SinTipoBusqueda = 0,
        Candidato = 1,
        Grupo = 2,
        Evento = 3
    }

    class BuscadorModel
    {
        public int CodigoPais { get; set; }
        public int CodigoArchivoPais { get; set; }
        public string UrlArchivoPais { get; set; }
        public string DescripcionPais { get; set; }

        public int CodigoArchivoPrincipal { get; set; }
        public int CodigoPrincipal { get; set; }
        public string IdentificadorPrincipal { get; set; }
        public string UrlArchivoPrincipal { get; set; }

        public int Estatura { get; set; }
        public int Peso { get; set; }
        public int Edad { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }

        public TipoBusqueda TipoBusqueda { get; set; }

        public BuscadorModel()
        {

        }

        public BuscadorModel(CandidatosDTO candidato) : this(candidato.Personas, TipoBusqueda.Candidato)
        {
            Estatura = candidato.Estatura;
            Peso = candidato.Peso;
            Edad = Convert.ToInt32(DateTimeHelper.DiferenciaEntreDosFechasAños(DateTime.Now, candidato.FechaNacimiento));
        }

        public BuscadorModel(GruposDTO grupo) : this(grupo.Personas, TipoBusqueda.Grupo)
        {

        }

        public BuscadorModel(PersonasDTO persona, TipoBusqueda tipoBusqueda)
        {
            CodigoPais = persona.CodigoPais;
            CodigoArchivoPais = persona.Paises.CodigoArchivo;
            UrlArchivoPais = persona.Paises.UrlArchivo;
            DescripcionPais = persona.Paises.DescripcionIdiomaBuscado;

            CodigoArchivoPrincipal = persona.CodigoArchivoImagenPerfil.HasValue ? persona.CodigoArchivoImagenPerfil.Value : 0;
            CodigoPrincipal = persona.Consecutivo;
            IdentificadorPrincipal = persona.Nombres + " " + persona.Apellidos;
            UrlArchivoPrincipal = persona.UrlImagenPerfil;

            TipoBusqueda = tipoBusqueda;
        }

        public BuscadorModel(GruposEventosDTO grupoEvento)
        {
            CodigoPais = grupoEvento.CodigoPais;
            CodigoArchivoPais = grupoEvento.Paises.CodigoArchivo;
            UrlArchivoPais = grupoEvento.Paises.UrlArchivo;
            DescripcionPais = grupoEvento.Paises.DescripcionIdiomaBuscado;

            CodigoArchivoPrincipal = grupoEvento.Grupos.Personas.CodigoArchivoImagenPerfil.HasValue ? grupoEvento.Grupos.Personas.CodigoArchivoImagenPerfil.Value : 0;
            CodigoPrincipal = grupoEvento.Consecutivo;
            IdentificadorPrincipal = grupoEvento.Titulo;
            UrlArchivoPrincipal = grupoEvento.Grupos.Personas.UrlImagenPerfil;

            FechaInicio = grupoEvento.FechaInicio;
            FechaFinal = grupoEvento.FechaTerminacion;

            TipoBusqueda = TipoBusqueda.Evento;
        }

        public static List<BuscadorModel> CrearListaBuscadorModel(ICollection<CandidatosDTO> listaCandidatos)
        {
            List<BuscadorModel> listaBuscador = new List<BuscadorModel>();

            if (listaCandidatos != null && listaCandidatos.Count > 0)
            {
                foreach (var item in listaCandidatos)
                {
                    listaBuscador.Add(new BuscadorModel(item));
                }
            }

            return listaBuscador;
        }

        public static List<BuscadorModel> CrearListaBuscadorModel(ICollection<GruposDTO> listaGrupos)
        {
            List<BuscadorModel> listaBuscador = new List<BuscadorModel>();

            if (listaGrupos != null && listaGrupos.Count > 0)
            {
                foreach (var item in listaGrupos)
                {
                    listaBuscador.Add(new BuscadorModel(item));
                }
            }

            return listaBuscador;
        }

        public static List<BuscadorModel> CrearListaBuscadorModel(ICollection<GruposEventosDTO> listaEventos)
        {
            List<BuscadorModel> listaBuscador = new List<BuscadorModel>();

            if (listaEventos != null && listaEventos.Count > 0)
            {
                foreach (var item in listaEventos)
                {
                    listaBuscador.Add(new BuscadorModel(item));
                }
            }

            return listaBuscador;
        }
    }
}
