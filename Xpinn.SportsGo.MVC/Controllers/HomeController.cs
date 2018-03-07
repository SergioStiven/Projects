using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System;
using System.IO;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.MVC.Models;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            if (UserLoggedIn() == null)
                return RedirectToAction("Index", "Authenticate");
            
            return View();
        }

        public ActionResult News()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult saveNew(IEnumerable<HttpPostedFileBase> fileNew, string description)
        {
            return RedirectToAction("Index", "Home");

        }
        
        [HttpPost]
        public async Task<JsonResult> GetNewsTimeLine(BuscadorDTO search)
        {
            Result<TimeLineNoticias> result = new Result<TimeLineNoticias>();
            try
            {
                NoticiasServices newsService = new NoticiasServices();
                search.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                result.list = await newsService.ListarTimeLine(search);
                if(result.list != null)
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
        public async Task<JsonResult> GetNotifications(BuscadorDTO search)
        {
            Result<TimeLineNotificaciones> result = new Result<TimeLineNotificaciones>();
            try
            {
                NoticiasServices newsService = new NoticiasServices();
                search.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                search.ConsecutivoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                search.TipoDePerfil = UserLoggedIn().TipoPerfil;
                search.CodigoPlanUsuario = UserLoggedIn().CodigoPlanUsuario;
                result.list = await newsService.ListaTimeLineNotificaciones(search);
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
        public async Task<JsonResult> UpdateCounterAdSeen(AnunciosDTO ad)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AnunciantesServices advertiserService = new AnunciantesServices();
                result.obj = await advertiserService.AumentarContadorClickDeUnAnuncio(ad);
                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
    }
}