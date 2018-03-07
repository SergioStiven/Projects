using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.MVC.Models;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class EventsController : BaseController
    {
        #region Views
        // GET: Events index
        public ActionResult Index()
        {
            return View();
        }
        
        // GET: All Events
        public ActionResult ListOfAllEvents()
        {
            return View();
        }

        // GET: Events by Group
        public ActionResult ListOfMyEvents()
        {
            return View();
        }
        
        // GET: Create Events
        public ActionResult CreateEvents()
        {
            return View();
        }

        // Get: Assistant Events 
        public ActionResult AssistantEvents()
        {
            return View();
        }

        // Get: Event Detail 
        public ActionResult EventDetail()
        {
            return View();
        }


        // Get: Filter Event 
        public ActionResult FiltersEvents()
        {
            return View();
        }

        #endregion

        #region Methods
        
        [HttpPost]
        public async Task<JsonResult> GetAllEvents(BuscadorDTO filter)
        {
            try
            {
                filter.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                Result<GruposEventosDTO> result = new Result<GruposEventosDTO>();
                GruposServices groupService = new GruposServices();
                result.list = await groupService.ListarEventos(filter);

                if (result.list != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetListEventsByGroup(BuscadorDTO filter)
        {
            Result<GruposEventosDTO> result = new Result<GruposEventosDTO>();
            try
            {
                GruposServices categoryService = new GruposServices();
                filter.ConsecutivoPerfil = UserLoggedIn().PersonaDelUsuario.GrupoDeLaPersona.Consecutivo;
                result.list = await categoryService.ListarEventosDeUnGrupo(filter);
                if (result.list == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> CreateEvent(GruposEventosDTO post)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                GruposServices categoryService = new GruposServices();
                post.CodigoGrupo = UserLoggedIn().PersonaDelUsuario.GrupoDeLaPersona.Consecutivo;
                post.CodigoIdioma = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                if (post.Consecutivo != 0)
                    result.obj = await categoryService.ModificarInformacionGrupoEvento(post);
                else
                    result.obj = await categoryService.CrearGrupoEvento(post);

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateEventFile(GruposEventosDTO post)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices fileService = new ArchivosServices();
                GruposEventosDTO eventToUpdate = Newtonsoft.Json.JsonConvert.DeserializeObject<GruposEventosDTO>(Request.Form["eventToUpdate"]);

                result.obj = await fileService.ModificarArchivoEventos(
                    Helper.getFileType(Request.Files[0].FileName), eventToUpdate.Consecutivo, eventToUpdate.CodigoArchivo, Request.Files[0].InputStream);

                if (result.obj != null && result.obj.Exitoso)
                    return Json(result, JsonRequestBehavior.AllowGet);

                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetListAssistantEvents(BuscadorDTO filter)
        {
            try
            {
                Result<GruposEventosAsistentesDTO> result = new Result<GruposEventosAsistentesDTO>();
                GruposServices groupService = new GruposServices();
                filter.ConsecutivoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                filter.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                result.list = await groupService.ListarEventosAsistentesDeUnaPersona(filter);

                if (result.list != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetEventDetail(GruposEventosDTO eventGroup)
        {
            try
            {
                Result<GruposEventosDTO> result = new Result<GruposEventosDTO>();
                GruposServices groupService = new GruposServices();
                result.obj = await groupService.BuscarGrupoEventoPorConsecutivo(eventGroup);

                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ValidateIfIWillAttend(GruposEventosAsistentesDTO eventGroup)
        {
            try
            {
                if (eventGroup.CodigoPersona == UserLoggedIn().PersonaDelUsuario.Consecutivo)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
                GruposServices groupService = new GruposServices();
                eventGroup.CodigoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj = await groupService.BuscarSiPersonaAsisteAGrupoEvento(eventGroup);

                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        public JsonResult IsGroup()
        {
            try
            {
                if (UserLoggedIn().PersonaDelUsuario.GrupoDeLaPersona != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSesion(), JsonRequestBehavior.AllowGet);
                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateAssistantEvent(GruposEventosAsistentesDTO eventGroup)
        {
            try
            {
                Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
                GruposServices groupService = new GruposServices();
                eventGroup.CodigoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj = await groupService.CrearGruposEventosAsistentes(eventGroup);

                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteAssistantEvent(GruposEventosAsistentesDTO eventGroup)
        {
            try
            {
                Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
                GruposServices groupService = new GruposServices();
                eventGroup.CodigoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj = await groupService.EliminarGrupoEventoAsistente(eventGroup);

                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeletetEvent(GruposEventosDTO eventGroup)
        {
            try
            {
                Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
                GruposServices groupService = new GruposServices();
                result.obj = await groupService.EliminarGrupoEvento(eventGroup);

                if (result.obj != null)
                    return Json(Helper.returnSuccessDeleteObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}