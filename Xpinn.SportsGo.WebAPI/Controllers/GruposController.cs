using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Entities;
using Microsoft.AspNet.SignalR;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class GruposController : ApiController
    {
        GruposBusiness _grupoBusiness;

        public GruposController()
        {
            _grupoBusiness = new GruposBusiness();
        }


        #region Metodos Grupos


        public async Task<IHttpActionResult> CrearGrupo(Grupos grupoParaCrear)
        {
            if (grupoParaCrear == null || grupoParaCrear.Personas == null || grupoParaCrear.Personas.Usuarios == null || grupoParaCrear.CategoriasGrupos == null)
            {
                return BadRequest("grupoParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Nombres) || grupoParaCrear.Personas.CodigoPais <= 0 || grupoParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil
                    || grupoParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.CiudadResidencia))
            {
                return BadRequest("Persona de grupoParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(grupoParaCrear.Personas.Usuarios.Email))
            {
                return BadRequest("Usuario de grupoParaCrear vacio y/o invalido!.");
            }
            else if (grupoParaCrear.CategoriasGrupos.Count <= 0 || !grupoParaCrear.CategoriasGrupos.All(x => x.CodigoCategoria > 0))
            {
                return BadRequest("Categorias de grupoParaCrear vacio y/o invalido!.");
            }

            try
            {
                string urlLogo = Url.Content("~/Content/Images/LogoSportsGo.png");
                string urlBanner = Url.Content("~/Content/Images/BannerSportsGo.png");

                WrapperSimpleTypesDTO wrapperCrearGrupo = await _grupoBusiness.CrearGrupo(grupoParaCrear, urlLogo, urlBanner);

                return Ok(wrapperCrearGrupo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarGrupoPorCodigoPersona(Grupos grupoParaBuscar)
        {
            if (grupoParaBuscar == null || grupoParaBuscar.Personas == null || grupoParaBuscar.Personas.Consecutivo <= 0)
            {
                return BadRequest("grupoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Grupos grupoBuscado = await _grupoBusiness.BuscarGrupoPorCodigoPersona(grupoParaBuscar);

                return Ok(grupoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarGrupoPorCodigoGrupo(Grupos grupoParaBuscar)
        {
            if (grupoParaBuscar == null || grupoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("grupoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Grupos grupoBuscado = await _grupoBusiness.BuscarGrupoPorCodigoGrupo(grupoParaBuscar);

                return Ok(grupoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarGrupos(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<GruposDTO> listaInformacionGrupos = await _grupoBusiness.ListarGrupos(buscador);

                return Ok(listaInformacionGrupos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarInformacionGrupo(Grupos grupoParaModificar)
        {
            if (grupoParaModificar == null || grupoParaModificar.Consecutivo <= 0)
            {
                return BadRequest("grupoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarInformacionGrupo = await _grupoBusiness.ModificarInformacionGrupo(grupoParaModificar);

                return Ok(wrapperModificarInformacionGrupo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos GruposEventos


        public async Task<IHttpActionResult> CrearGrupoEvento(GruposEventos grupoEventoParaCrear)
        {
            if (grupoEventoParaCrear == null || grupoEventoParaCrear.CategoriasEventos == null || grupoEventoParaCrear.CodigoIdioma <= 0 || grupoEventoParaCrear.CodigoPais <= 0
                || string.IsNullOrWhiteSpace(grupoEventoParaCrear.Titulo) || grupoEventoParaCrear.CodigoGrupo <= 0
                || grupoEventoParaCrear.CategoriasEventos.Count <= 0 || !grupoEventoParaCrear.CategoriasEventos.All(x => x.CodigoCategoria > 0)
                || grupoEventoParaCrear.FechaInicio == DateTime.MinValue || grupoEventoParaCrear.FechaTerminacion == DateTime.MinValue)
            {
                return BadRequest("grupoEventoParaCrear vacio y/o invalido!.");
            }
            else if (grupoEventoParaCrear.Archivos != null)
            {
                return BadRequest("Usa CrearArchivoStream en ArchivosService para crear el archivo o mataras la memoria del servidor!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearGrupoEvento = await _grupoBusiness.CrearGrupoEvento(grupoEventoParaCrear);

                return Ok(wrapperCrearGrupoEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarGrupoEventoPorConsecutivo(GruposEventos grupoEventoParaBuscar)
        {
            if (grupoEventoParaBuscar == null || grupoEventoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("grupoEventoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                GruposEventosDTO grupoEventoBuscado = await _grupoBusiness.BuscarGrupoEventoPorConsecutivo(grupoEventoParaBuscar);

                return Ok(grupoEventoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarEventosDeUnGrupo(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.ConsecutivoPerfil <= 0 || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<GruposEventosDTO> listaEventosDeUnGrupo = await _grupoBusiness.ListarEventosDeUnGrupo(buscador);

                return Ok(listaEventosDeUnGrupo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarEventos(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<GruposEventosDTO> listaEventos = await _grupoBusiness.ListarEventos(buscador);

                return Ok(listaEventos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarInformacionGrupoEvento(GruposEventos grupoEventoParaModificar)
        {
            if (grupoEventoParaModificar == null || grupoEventoParaModificar.Consecutivo <= 0 || grupoEventoParaModificar.CodigoIdioma <= 0 || grupoEventoParaModificar.CodigoPais <= 0
                || string.IsNullOrWhiteSpace(grupoEventoParaModificar.Titulo) || grupoEventoParaModificar.CodigoGrupo <= 0
                || grupoEventoParaModificar.FechaInicio == DateTime.MinValue || grupoEventoParaModificar.FechaTerminacion == DateTime.MinValue)
            {
                return BadRequest("grupoEventoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarGrupoEvento = await _grupoBusiness.ModificarInformacionGrupoEvento(grupoEventoParaModificar);

                return Ok(wrapperModificarGrupoEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarGrupoEvento(GruposEventos grupoEventoParaEliminar)
        {
            if (grupoEventoParaEliminar == null || grupoEventoParaEliminar.Consecutivo <= 0)
            { 
                return BadRequest("grupoEventoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarGrupoEvento = await _grupoBusiness.EliminarGrupoEvento(grupoEventoParaEliminar);

                return Ok(wrapperEliminarGrupoEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarArchivoGrupoEvento(GruposEventos grupoEventoArchivoParaBorrar)
        {
            if (grupoEventoArchivoParaBorrar == null || grupoEventoArchivoParaBorrar.Consecutivo <= 0 || grupoEventoArchivoParaBorrar.CodigoArchivo <= 0)
            {
                return BadRequest("grupoEventoArchivoParaBorrar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarArchivoGrupoEvento = await _grupoBusiness.EliminarArchivoGrupoEvento(grupoEventoArchivoParaBorrar);

                return Ok(wrapperEliminarArchivoGrupoEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos GruposEventosAsistentes


        public async Task<IHttpActionResult> CrearGruposEventosAsistentes(GruposEventosAsistentes grupoEventoAsistentesParaCrear)
        {
            if (grupoEventoAsistentesParaCrear == null || grupoEventoAsistentesParaCrear.CodigoEvento <= 0 || grupoEventoAsistentesParaCrear.CodigoPersona <= 0)
            {
                return BadRequest("grupoEventoAsistentesParaCrear vacio y/o invalido!.");
            }

            try
            {
                Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones> tupleWrapper = await _grupoBusiness.CrearGruposEventosAsistentes(grupoEventoAsistentesParaCrear);

                if (tupleWrapper.Item1.Exitoso && tupleWrapper.Item2 != null)
                {
                    NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

                    IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hubContext.Clients.Group(ChatHub._prefixChatGroupName + tupleWrapper.Item2.CodigoPersonaDestino.ToString()).receiveNotification(tupleWrapper.Item2);
                }

                return Ok(tupleWrapper.Item1);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarNumeroAsistentesGruposEventos(GruposEventosAsistentes grupoEventoAsistenteParaBuscar)
        {
            if (grupoEventoAsistenteParaBuscar == null || grupoEventoAsistenteParaBuscar.CodigoEvento <= 0)
            {
                return BadRequest("grupoEventoAsistenteParaBuscar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperNumeroAsistentes = await _grupoBusiness.BuscarNumeroAsistentesGruposEventos(grupoEventoAsistenteParaBuscar);

                return Ok(wrapperNumeroAsistentes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarSiPersonaAsisteAGrupoEvento(GruposEventosAsistentes grupoEventoAsistenteParaBuscar)
        {
            if (grupoEventoAsistenteParaBuscar == null || grupoEventoAsistenteParaBuscar.CodigoEvento <= 0 || grupoEventoAsistenteParaBuscar.CodigoPersona <= 0)
            {
                return BadRequest("grupoEventoAsistenteParaBuscar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperBuscarSiPersonaAsisteAGrupoEvento = await _grupoBusiness.BuscarSiPersonaAsisteAGrupoEvento(grupoEventoAsistenteParaBuscar);

                return Ok(wrapperBuscarSiPersonaAsisteAGrupoEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarEventosAsistentesDeUnEvento(GruposEventosAsistentes grupoEventoAsistenteParaListar)
        {
            if (grupoEventoAsistenteParaListar == null || grupoEventoAsistenteParaListar.CodigoEvento <= 0 || grupoEventoAsistenteParaListar.IdiomaBase == Idioma.SinIdioma
                || grupoEventoAsistenteParaListar.SkipIndexBase < 0 || grupoEventoAsistenteParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("grupoEventoAsistenteParaListar vacio y/o invalido!.");
            }

            try
            {
                List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnEvento = await _grupoBusiness.ListarEventosAsistentesDeUnEvento(grupoEventoAsistenteParaListar);

                return Ok(listaEventosAsistentesDeUnEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarEventosAsistentesDeUnaPersona(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.ConsecutivoPersona <= 0 || buscador.IdiomaBase == Idioma.SinIdioma 
                || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnaPersona = await _grupoBusiness.ListarEventosAsistentesDeUnaPersona(buscador);

                return Ok(listaEventosAsistentesDeUnaPersona);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarGrupoEventoAsistente(GruposEventosAsistentes grupoEventoAsistenteParaEliminar)
        {
            if (grupoEventoAsistenteParaEliminar == null || grupoEventoAsistenteParaEliminar.CodigoPersona <= 0 || grupoEventoAsistenteParaEliminar.CodigoEvento <= 0)
            {
                return BadRequest("grupoEventoAsistenteParaEliminar vacio y/o invalido!.");
            }

            try
            {
                Tuple<WrapperSimpleTypesDTO,TimeLineNotificaciones> tupleWrapper = await _grupoBusiness.EliminarGrupoEventoAsistente(grupoEventoAsistenteParaEliminar);

                if (tupleWrapper.Item1.Exitoso && tupleWrapper.Item2 != null)
                {
                    NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

                    IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hubContext.Clients.Group(ChatHub._prefixChatGroupName + tupleWrapper.Item2.CodigoPersonaDestino.ToString()).receiveNotification(tupleWrapper.Item2);
                }

                return Ok(tupleWrapper.Item1);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
