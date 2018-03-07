using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.MVC.Models;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class SearchController : BaseController
    {
        #region Views
        // GET: Search
        public async Task<ActionResult> Index()
        {
            if (await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.ConsultaCandidatos)
                || await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.ConsultaEventos)
                || await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.ConsultaGrupos))
                return View();
            ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            return View("Unauthorized");
        }
        #endregion

        #region Methods
        [HttpPost]
        public async Task<JsonResult> listarCandidates(BuscadorDTO filter)
        {
            try
            {
                if (!await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.ConsultaCandidatos))
                    return Json(Helper.returnErrorUnauthorizedByPlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                Result<CandidatosDTO> result = new Result<CandidatosDTO>();
                CandidatosServices candidatoService = new CandidatosServices();
                filter.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.list = await candidatoService.ListarCandidatos(filter);

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
        public async Task<JsonResult> listarGrupos(BuscadorDTO filter)
        {
            try
            {
                if (!await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.ConsultaGrupos))
                    return Json(Helper.returnErrorUnauthorizedByPlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                Result<GruposDTO> result = new Result<GruposDTO>();
                GruposServices groupService = new GruposServices();
                result.list = await groupService.ListarGrupos(filter);

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
        public async Task<JsonResult> listarEvents(BuscadorDTO filter)
        {
            try
            {
                if (!await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.ConsultaEventos))
                    return Json(Helper.returnErrorUnauthorizedByPlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                Result<GruposEventosDTO> result = new Result<GruposEventosDTO>();
                GruposServices groupService = new GruposServices();
                filter.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
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
        public async Task<JsonResult> SaveSearchIdInSession(PersonasDTO person)
        {
            try
            {
                Result<PersonasDTO> result = new Result<PersonasDTO>();

                PersonasServices personService = new PersonasServices();
                person.CodigoIdioma = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.obj = await personService.BuscarPersona(person);
                if (result.obj != null)
                {
                    SetPersonToSearch(result.obj);
                    result.Path = "Profile/";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }                    
                else
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
    }
}