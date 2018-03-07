using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Services
{
    public class GruposServices : BaseService
    {


        #region Metodos Grupos


        public async Task<WrapperSimpleTypesDTO> CrearGrupo(GruposDTO grupoParaCrear)
        {
            if (grupoParaCrear == null || grupoParaCrear.Personas == null || grupoParaCrear.Personas.Usuarios == null || grupoParaCrear.CategoriasGrupos == null)
            {
                throw new ArgumentNullException("No puedes crear un grupo si grupoParaCrear, la persona, las categorias o el usuario del grupo es nulo!.");
            }
            else if (string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Nombres) || grupoParaCrear.Personas.CodigoPais <= 0 || grupoParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil
                    || grupoParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.CiudadResidencia))
            {
                throw new ArgumentException("Persona de grupoParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Usuarios.Email))
            {
                throw new ArgumentException("Usuario de grupoParaCrear vacio y/o invalido!.");
            }
            else if (grupoParaCrear.CategoriasGrupos.Count <= 0 || !grupoParaCrear.CategoriasGrupos.All(x => x.CodigoCategoria > 0))
            {
                throw new ArgumentException("Categorias de grupoParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearGrupo = await client.PostAsync<GruposDTO, WrapperSimpleTypesDTO>("Grupos/CrearGrupo", grupoParaCrear);

            return wrapperCrearGrupo;
        }

        public async Task<GruposDTO> BuscarGrupoPorCodigoPersona(GruposDTO grupoParaBuscar)
        {
            if (grupoParaBuscar == null || grupoParaBuscar.Personas == null) throw new ArgumentNullException("No puedes buscar un grupo si grupoParaBuscar es nulo!.");
            if (grupoParaBuscar.Personas.Consecutivo <= 0) throw new ArgumentException("grupoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            GruposDTO grupoBuscado = await client.PostAsync("Grupos/BuscarGrupoPorCodigoPersona", grupoParaBuscar);

            return grupoBuscado;
        }

        public async Task<GruposDTO> BuscarGrupoPorCodigoGrupo(GruposDTO grupoParaBuscar)
        {
            if (grupoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un grupo si grupoParaBuscar es nulo!.");
            if (grupoParaBuscar.Consecutivo <= 0) throw new ArgumentException("grupoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            GruposDTO grupoBuscado = await client.PostAsync("Grupos/BuscarGrupoPorCodigoGrupo", grupoParaBuscar);

            return grupoBuscado;
        }

        public async Task<List<GruposDTO>> ListarGrupos(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar los grupos si buscador es nulo!.");
            if (buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<GruposDTO> listaInformacionGrupos = await client.PostAsync<BuscadorDTO, List<GruposDTO>>("Grupos/ListarGrupos", buscador);

            return listaInformacionGrupos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionGrupo(GruposDTO grupoParaModificar)
        {
            if (grupoParaModificar == null) throw new ArgumentNullException("No puedes modificar un grupo si grupoParaModificar es nulo!.");
            if (grupoParaModificar.Consecutivo <= 0)
            {
                throw new ArgumentException("grupoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarGrupo = await client.PostAsync<GruposDTO, WrapperSimpleTypesDTO>("Grupos/ModificarInformacionGrupo", grupoParaModificar);

            return wrapperModificarGrupo;
        }


        #endregion


        #region Metodos GruposEventos


        public async Task<WrapperSimpleTypesDTO> CrearGrupoEvento(GruposEventosDTO grupoEventoParaCrear)
        {
            if (grupoEventoParaCrear == null || grupoEventoParaCrear.CategoriasEventos == null)
            {
                throw new ArgumentNullException("No puedes crear un grupoEvento si grupoEventoParaCrear o sus categorias son nulas!.");
            }
            if (grupoEventoParaCrear.CodigoIdioma <= 0 || grupoEventoParaCrear.CodigoPais <= 0
               || string.IsNullOrWhiteSpace(grupoEventoParaCrear.Titulo) || grupoEventoParaCrear.CodigoGrupo <= 0
               || grupoEventoParaCrear.CategoriasEventos.Count <= 0 || !grupoEventoParaCrear.CategoriasEventos.All(x => x.CodigoCategoria > 0)
               || grupoEventoParaCrear.FechaInicio == DateTime.MinValue || grupoEventoParaCrear.FechaTerminacion == DateTime.MinValue)
            {
                throw new ArgumentException("grupoEventoParaCrear vacia y/o invalida!.");
            }
            else if (grupoEventoParaCrear.Archivos != null)
            {
                throw new ArgumentException("Usa CrearArchivoStream en ArchivosService para crear el archivo o mataras la memoria del servidor!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearGrupoEvento = await client.PostAsync<GruposEventosDTO, WrapperSimpleTypesDTO>("Grupos/CrearGrupoEvento", grupoEventoParaCrear);

            return wrapperCrearGrupoEvento;
        }

        public async Task<GruposEventosDTO> BuscarGrupoEventoPorConsecutivo(GruposEventosDTO grupoEventoParaBuscar)
        {
            if (grupoEventoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un grupoEvento si grupoEventoParaBuscar es nulo!.");
            if (grupoEventoParaBuscar.Consecutivo <= 0) throw new ArgumentException("grupoEventoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            GruposEventosDTO grupoEventoBuscado = await client.PostAsync("Grupos/BuscarGrupoEventoPorConsecutivo", grupoEventoParaBuscar);

            return grupoEventoBuscado;
        }

        public async Task<List<GruposEventosDTO>> ListarEventosDeUnGrupo(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes buscar un grupoEvento si grupoEventoParaListar es nulo!.");
            if (buscador.ConsecutivoPerfil <= 0 || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0)
            {
                throw new ArgumentException("grupoEventoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<GruposEventosDTO> listaEventosDeUnGrupo = await client.PostAsync<BuscadorDTO, List<GruposEventosDTO>>("Grupos/ListarEventosDeUnGrupo", buscador);

            return listaEventosDeUnGrupo;
        }

        public async Task<List<GruposEventosDTO>> ListarEventos(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar los grupoEvento si buscador es nulo!.");
            if (buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<GruposEventosDTO> listaEventos = await client.PostAsync<BuscadorDTO, List<GruposEventosDTO>>("Grupos/ListarEventos", buscador);

            return listaEventos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionGrupoEvento(GruposEventosDTO grupoEventoParaModificar)
        {
            if (grupoEventoParaModificar == null) throw new ArgumentNullException("No puedes modificar un grupoEvento si grupoEventoParaModificar es nulo!.");
            if (grupoEventoParaModificar.Consecutivo <= 0 || grupoEventoParaModificar.CodigoIdioma <= 0 || grupoEventoParaModificar.CodigoPais <= 0
                || string.IsNullOrWhiteSpace(grupoEventoParaModificar.Titulo) || grupoEventoParaModificar.CodigoGrupo <= 0 
                || grupoEventoParaModificar.FechaInicio == DateTime.MinValue || grupoEventoParaModificar.FechaTerminacion == DateTime.MinValue) 
            {
                throw new ArgumentNullException("grupoEventoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarGrupoEvento = await client.PostAsync<GruposEventosDTO, WrapperSimpleTypesDTO>("Grupos/ModificarInformacionGrupoEvento", grupoEventoParaModificar);

            return wrapperModificarGrupoEvento;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarGrupoEvento(GruposEventosDTO grupoEventoParaEliminar)
        {
            if (grupoEventoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un grupoEvento si grupoEventoParaEliminar es nulo!.");
            if (grupoEventoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("grupoEventoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarGrupoEvento = await client.PostAsync<GruposEventosDTO, WrapperSimpleTypesDTO>("Grupos/EliminarGrupoEvento", grupoEventoParaEliminar);

            return wrapperEliminarGrupoEvento;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarArchivoGrupoEvento(GruposEventosDTO grupoEventoArchivoParaBorrar)
        {
            if (grupoEventoArchivoParaBorrar == null) throw new ArgumentNullException("No puedes eliminar un archivoGrupoEvento si grupoEventoArchivoParaBorrar es nulo!.");
            if (grupoEventoArchivoParaBorrar.Consecutivo <= 0 || grupoEventoArchivoParaBorrar.CodigoArchivo <= 0)
            {
                throw new ArgumentException("grupoEventoArchivoParaBorrar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarArchivoGrupoEvento = await client.PostAsync<GruposEventosDTO, WrapperSimpleTypesDTO>("Grupos/EliminarArchivoGrupoEvento", grupoEventoArchivoParaBorrar);

            return wrapperEliminarArchivoGrupoEvento;
        }


        #endregion


        #region Metodos GruposEventosAsistentes


        public async Task<WrapperSimpleTypesDTO> CrearGruposEventosAsistentes(GruposEventosAsistentesDTO grupoEventoAsistentesParaCrear)
        {
            if (grupoEventoAsistentesParaCrear == null) throw new ArgumentNullException("No puedes crear un grupoEventoAsistente si grupoEventoAsistentesParaCrear es nulo!.");
            if (grupoEventoAsistentesParaCrear.CodigoEvento <= 0 || grupoEventoAsistentesParaCrear.CodigoPersona <= 0)
            {
                throw new ArgumentException("grupoEventoAsistentesParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearGrupoEventoAsistentes = await client.PostAsync<GruposEventosAsistentesDTO, WrapperSimpleTypesDTO>("Grupos/CrearGruposEventosAsistentes", grupoEventoAsistentesParaCrear);

            return wrapperCrearGrupoEventoAsistentes;
        }

        public async Task<WrapperSimpleTypesDTO> BuscarNumeroAsistentesGruposEventos(GruposEventosAsistentesDTO grupoEventoAsistenteParaBuscar)
        {
            if (grupoEventoAsistenteParaBuscar == null) throw new ArgumentNullException("No puedes enumerar los grupoEventoAsistente si grupoEventoAsistenteParaBuscar es nulo!.");
            if (grupoEventoAsistenteParaBuscar.CodigoEvento <= 0) throw new ArgumentException("grupoEventoAsistenteParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroAsistentes = await client.PostAsync<GruposEventosAsistentesDTO, WrapperSimpleTypesDTO>("Grupos/BuscarNumeroAsistentesGruposEventos", grupoEventoAsistenteParaBuscar);

            return wrapperNumeroAsistentes;
        }

        public async Task<WrapperSimpleTypesDTO> BuscarSiPersonaAsisteAGrupoEvento(GruposEventosAsistentesDTO grupoEventoAsistenteParaBuscar)
        {
            if (grupoEventoAsistenteParaBuscar == null) throw new ArgumentNullException("No puedes ver si la persona asiste a un grupo evento si grupoEventoAsistenteParaBuscar es nulo!.");
            if (grupoEventoAsistenteParaBuscar.CodigoEvento <= 0 || grupoEventoAsistenteParaBuscar.CodigoPersona <= 0)
            {
                throw new ArgumentException("grupoEventoAsistenteParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperBuscarSiPersonaAsisteAGrupoEvento = await client.PostAsync<GruposEventosAsistentesDTO, WrapperSimpleTypesDTO>("Grupos/BuscarSiPersonaAsisteAGrupoEvento", grupoEventoAsistenteParaBuscar);

            return wrapperBuscarSiPersonaAsisteAGrupoEvento;
        }

        public async Task<List<GruposEventosAsistentesDTO>> ListarEventosAsistentesDeUnEvento(GruposEventosAsistentesDTO grupoEventoAsistenteParaListar)
        {
            if (grupoEventoAsistenteParaListar == null) throw new ArgumentNullException("No puedes listar un grupoEventoAsistente si grupoEventoAsistenteParaListar es nulo!.");
            if (grupoEventoAsistenteParaListar.CodigoEvento <= 0 || grupoEventoAsistenteParaListar.IdiomaBase == Idioma.SinIdioma
                || grupoEventoAsistenteParaListar.SkipIndexBase < 0 || grupoEventoAsistenteParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("grupoEventoAsistenteParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnGrupo = await client.PostAsync<GruposEventosAsistentesDTO, List<GruposEventosAsistentesDTO>>("Grupos/ListarEventosAsistentesDeUnEvento", grupoEventoAsistenteParaListar);

            return listaEventosAsistentesDeUnGrupo;
        }

        public async Task<List<GruposEventosAsistentesDTO>> ListarEventosAsistentesDeUnaPersona(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar un grupoEventoAsistente si buscador es nulo!.");
            if (buscador.ConsecutivoPersona <= 0 || buscador.IdiomaBase == Idioma.SinIdioma 
                || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnaPersona = await client.PostAsync<BuscadorDTO, List<GruposEventosAsistentesDTO>>("Grupos/ListarEventosAsistentesDeUnaPersona", buscador);

            return listaEventosAsistentesDeUnaPersona;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarGrupoEventoAsistente(GruposEventosAsistentesDTO grupoEventoAsistenteParaEliminar)
        {
            if (grupoEventoAsistenteParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un grupoEventoAsistente si grupoEventoAsistenteParaEliminar es nulo!.");
            if (grupoEventoAsistenteParaEliminar.CodigoPersona <= 0 || grupoEventoAsistenteParaEliminar.CodigoEvento <= 0)
            {
                throw new ArgumentException("grupoEventoAsistenteParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarGrupoEventoAsistente = await client.PostAsync<GruposEventosAsistentesDTO, WrapperSimpleTypesDTO>("Grupos/EliminarGrupoEventoAsistente", grupoEventoAsistenteParaEliminar);

            return wrapperEliminarGrupoEventoAsistente;
        }


        #endregion


    }
}
