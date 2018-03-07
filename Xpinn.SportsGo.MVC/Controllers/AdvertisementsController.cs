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
    public class AdvertisementsController : BaseController
    {
        #region Views
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignInadvertiser()
        {
            return View();
        }

        public async Task<ActionResult> Dashboard()
        {
            if (await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.EstadisticasAnuncios))
                return View();
            ViewBag.Layout = "~/Views/Shared/Advertiser_Layout.cshtml";
            return View("Unauthorized");
        }

        public ActionResult MyAds()
        {
            return View();
        }
        
        public async Task<ActionResult> CreateAdvertiser()
        {
            if (await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.CreacionAnuncios))
                return View();
            ViewBag.Layout = "~/Views/Shared/Advertiser_Layout.cshtml";
            return View("Unauthorized");
        }

        public ActionResult Posts()
        {
            return View();
        }

        public ActionResult Pricing()
        {
            return View();
        }

        public ActionResult HistoryOfMyPlans()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }
        #endregion

        #region Methods
        [HttpGet]
        public async Task<JsonResult> GetAds()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                result.obj = await metricaService.NumeroAnunciosRegistrados(
                    new MetricasDTO() { CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo });
                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAdsLastMonth()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var ss = UserLoggedIn();
                result.obj = await metricaService.NumeroAnunciosRegistradosUltimoMes(
                    new MetricasDTO() { CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo });

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetClicks()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                result.obj = await metricaService.NumeroVecesClickeados(
                    new MetricasDTO() { CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo });

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetClicksLastMonth()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                result.obj = await metricaService.NumeroVecesClickeadosUltimoMes(
                    new MetricasDTO() { CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo });

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSeen()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                result.obj = await metricaService.NumeroVecesVistos(
                    new MetricasDTO() { CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo });

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSeenLastMonth()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                result.obj = await metricaService.NumeroVecesVistosUltimoMes(
                    new MetricasDTO() { CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo });

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public async Task<JsonResult> SaveAdvertiser(AnunciantesDTO advertiser)
        {
            Result<AnunciantesDTO> result = new Result<AnunciantesDTO>();
            try
            {
                AnunciantesServices advertiserService = new AnunciantesServices();
                advertiser.Personas.Usuarios = UserLoggedIn();
                advertiser.Personas.TipoPerfil = advertiser.Personas.Usuarios.TipoPerfil;
                advertiser.Personas.CodigoIdioma = advertiser.Personas.Usuarios.PersonaDelUsuario.CodigoIdioma;
                WrapperSimpleTypesDTO res = new WrapperSimpleTypesDTO();
                if (advertiser.Consecutivo != 0)
                {
                    PersonasServices personService = new PersonasServices();
                    res = await personService.ModificarPersona(advertiser.Personas);
                    if(res != null)
                    {
                        res = await advertiserService.ModificarInformacionAnunciante(advertiser);
                        result.obj = advertiser;
                    }                        
                }
                else
                {
                    advertiser.Anuncios = null;
                    advertiser.Personas.Usuarios.Personas = null;
                    advertiser.Personas.Usuarios.PersonaDelUsuario = null;
                    res = await advertiserService.CrearAnunciante(advertiser);
                    if (res != null)
                    {
                        advertiser.Consecutivo = (int)res.ConsecutivoCreado;
                        advertiser.CodigoPersona = res.ConsecutivoPersonaCreado;
                        advertiser.Personas.CodigoUsuario = res.ConsecutivoUsuarioCreado;
                        result.obj = advertiser;
                        AuthenticateServices AuthenticateService = new AuthenticateServices();
                        var newUser = await AuthenticateService.VerificarUsuario(UserLoggedIn());
                        setUserLogin(newUser);
                    }
                }

                if (res == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        
        [HttpPost]
        public async Task<JsonResult> GetListPostsByAdvertiser(AnunciosDTO posts)
        {
            Result<AnunciosDTO> result = new Result<AnunciosDTO>();
            try
            {
                AnunciantesServices categoryService = new AnunciantesServices();
                var ass = UserLoggedIn();
                posts.CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo;
                posts.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.list = await categoryService.ListarAnunciosDeUnAnunciante(posts);
                if (result.list == null)
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> CreatePostsByAdvertiser(AnunciosDTO posts)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AnunciantesServices advertiserService = new AnunciantesServices();
                
                posts.CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo;
                posts.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;

                if (posts.Consecutivo != 0)
                {
                    posts.AnunciosPaises.ToList().ForEach(c => c.CodigoAnuncio = posts.Consecutivo);
                    posts.CategoriasAnuncios.ToList().ForEach(c => c.CodigoAnuncio = posts.Consecutivo);
                    result.obj = await advertiserService.ModificarAnuncio(posts);
                }
                else
                    result.obj = await advertiserService.CrearAnuncio(posts);
                
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetPostsByAdvertiser(AnunciosDTO posts)
        {
            Result<AnunciosDTO> result = new Result<AnunciosDTO>();
            try
            {
                AnunciantesServices advertiserService = new AnunciantesServices();
                AnunciosDTO postToSearch = new AnunciosDTO();
                postToSearch.Consecutivo = posts.Consecutivo;
                
                result.obj = await advertiserService.BuscarAnuncioPorConsecutivo(postToSearch);
                
                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> DeletePostsByAdvertiser(AnunciosDTO ad)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AnunciantesServices advertiserService = new AnunciantesServices();
                result.obj = await advertiserService.EliminarAnuncio(new AnunciosDTO { Consecutivo = ad.Consecutivo });

                if (result.obj == null)
                    return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(Helper.returnSuccessDeleteObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        
        [HttpPost]
        public async Task<JsonResult> GetProfile()
        {
            Result<AnunciantesDTO> result = new Result<AnunciantesDTO>();
            try
            {
                AnunciantesServices categoryService = new AnunciantesServices();
                AnunciantesDTO advertiser = new AnunciantesDTO();
                advertiser.CodigoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj =  await categoryService.BuscarAnunciantePorCodigoPersona(advertiser);

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetDashboard(MetricasDTO metrica)
        {
            Result<MetricasDTO> result = new Result<MetricasDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                metrica.CodigoAnunciante = UserLoggedIn().PersonaDelUsuario.AnuncianteDeLaPersona.Consecutivo;
                result.obj = await metricaService.MetricasAnuncios(metrica);

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> UpdateFile(AnunciantesDTO advertiser)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices personService = new ArchivosServices();
                result.obj = await personService.ModificarArchivoAnuncio(
                    Helper.getFileType(Request.Files[0].FileName), 
                    int.Parse(Request.Form["Consecutivo"].ToString()), 
                    int.Parse(Request.Form["CodigoArchivo"].ToString()), 
                    Request.Files[0].InputStream);
                
                if (result.obj == null)
                    return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }

        #endregion
    }
}