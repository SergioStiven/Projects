using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xpinn.SportsGo.MVC.Models;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.IO;
using SimpleFeedReader;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class AdministrationController : BaseController
    {

        #region Views
        // GET: Dashboard Users
        public ActionResult Index()
        {
            return View();
        }

        // GET: Dashboard Ads
        public ActionResult Ads()
        {
            return View();
        }                                                                                               

        // GET: Dashboard Events
        public ActionResult Events()
        {
            return View();
        }

        // GET: User settings
        public ActionResult Users()
        {
            return View();
        }

        // GET: List settings
        public ActionResult List()
        {
            return View();
        }

        // GET: Payment methods settings
        public ActionResult PaymentMethods()
        {
            return View();
        }
        
        // GET: Payment plans settings
        public ActionResult PaymentPlans()
        {
            return View();
        }

        // GET: Tips
        public ActionResult News()
        {
            return View();
        }

        // Get: List of Tips (Partial View)
        public ActionResult ListOfNews()
        {
            return View();
        }

        // Get: RSS Form (Partial View)
        public ActionResult RssForm()
        {
            return View();
        }
        
        #endregion

        #region Methods

        #region Menu
        [HttpPost]
        public async Task<ActionResult> UpdateImageProfileAdministrator()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices autenticateService = new AdministracionServices();
                int idAdministrator = UserLoggedIn().Consecutivo;
                
                result.obj = await autenticateService.AsignarImagenPerfilAdministrador(idAdministrator, Request.Files[0].InputStream);
                
                if (result.obj == null)
                    return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                {
                    UserLoggedIn().CodigoImagenPerfilAdmin = (int)result.obj.ConsecutivoArchivoCreado;
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                    
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }            
        }
        #endregion

        #region Dashboard Users
        [HttpPost]
        public async Task<JsonResult> GetInfoMetrica(MetricasDTO metricaParaBuscar)
        {
            Result<MetricasDTO> result = new Result<MetricasDTO>();
            try
            {
                MetricasServices metricasService = new MetricasServices();
                var metrica = await metricasService.MetricasUsuarios(metricaParaBuscar);
                result.obj = metrica;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> GetListCountries(PaisesDTO countryForSearch)
        {
            Result<PaisesDTO> result = new Result<PaisesDTO>();
            try
            {
                AdministracionServices countryService = new AdministracionServices();
                countryForSearch.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                result.list = await countryService.ListarPaisesPorIdioma(countryForSearch);
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
        [HttpGet]
        public async Task<JsonResult> GetRegisteredUsers()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var registeredUsers = await metricaService.NumeroUsuariosRegistrados();
                if (registeredUsers == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = registeredUsers;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetRegisteredUsersLastMonth()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var registeredUsersLastMonth = await metricaService.NumeroUsuariosRegistradosUltimoMes();
                if(registeredUsersLastMonth == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = registeredUsersLastMonth;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetPlansLastMonth()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var plansLastMonth = await metricaService.NumeroVentasUltimoMes(); // reemplazar por el de planes
                if (plansLastMonth == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = plansLastMonth;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetMovileDownloads()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var movileDownloads = await metricaService.NumeroDescargasMoviles();
                if (movileDownloads == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = movileDownloads;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Dashboard Ads
        [HttpPost]
        public async Task<JsonResult> GetInfoMetricaAds(MetricasDTO metricaParaBuscar)
        {
            Result<MetricasDTO> result = new Result<MetricasDTO>();
            try
            {
                MetricasServices metricasService = new MetricasServices();
                var metrica = await metricasService.MetricasAnuncios(metricaParaBuscar);
                if(metrica == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = metrica;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public async Task<JsonResult> GetRegisteredAdvertisers()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var registeredAdvertisers = await metricaService.NumeroAnunciantesRegistrados();
                if (registeredAdvertisers == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = registeredAdvertisers;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetRegisteredAdvertisersLastMonth()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var registeredAdvertisersLastMonth = await metricaService.NumeroAnunciantesRegistradosUltimoMes();
                if (registeredAdvertisersLastMonth == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = registeredAdvertisersLastMonth;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetAds()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var ads = await metricaService.NumeroAnunciosRegistrados();
                if (ads == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = ads;
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
                result.obj = await metricaService.NumeroAnunciosRegistradosUltimoMes();
                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Dashboard Events
        [HttpPost]
        public async Task<JsonResult> GetInfoMetricaEvents(MetricasDTO metricaParaBuscar)
        {
            Result<MetricasDTO> result = new Result<MetricasDTO>();
            try
            {
                MetricasServices metricasService = new MetricasServices();
                var metrica = await metricasService.MetricasEventos(metricaParaBuscar);
                if (metrica == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = metrica;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public async Task<JsonResult> GetRegisteredEvents()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var registeredEvents = await metricaService.NumeroEventosRegistrados();
                if (registeredEvents == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = registeredEvents;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetRegisteredEventsLastMonth()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                MetricasServices metricaService = new MetricasServices();
                var registeredEventsLastMonth = await metricaService.NumeroEventosRegistradosUltimoMes();
                if (registeredEventsLastMonth == null)
                {
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.obj = registeredEventsLastMonth;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Users settings
        [HttpPost]
        public async Task<JsonResult> GetUsers(MetricasDTO userToSearch)
        {
            Result<PersonasDTO> result = new Result<PersonasDTO>();
            try
            {
                MetricasServices plansService = new MetricasServices();
                userToSearch.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.list = await plansService.ListarUsuariosMetricas(userToSearch);
                if (result.list != null)
                    return Json(result, JsonRequestBehavior.AllowGet);                
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> BlockUser(UsuariosDTO userToBlock)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AuthenticateServices authenticateService = new AuthenticateServices();
                result.obj = await authenticateService.BloquearUsuario(new UsuariosDTO() { Consecutivo = userToBlock.Consecutivo});
                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UnlockUser(UsuariosDTO userToBlock)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AuthenticateServices authenticateService = new AuthenticateServices();
                result.obj = await authenticateService.ActivarUsuario(new UsuariosDTO() { Consecutivo = userToBlock.Consecutivo });
                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteUser(UsuariosDTO userToDelete)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices authenticateService = new AdministracionServices();
                result.obj = await authenticateService.EliminarUsuario(new UsuariosDTO() { Consecutivo = userToDelete.Consecutivo });
                if (result.obj != null)
                    return Json(Helper.returnDeleteSuccess(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateUser(UsuariosDTO userToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices authenticateService = new AdministracionServices();
                result.obj = await authenticateService.ModificarUsuario(userToUpdate);
                if (result.obj != null)
                    return Json(Helper.returnSuccessObj(1), JsonRequestBehavior.AllowGet);
                return Json(Helper.returnErrorSaveObj(1), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Plans settings
        [HttpPost]
        public async Task<JsonResult> GetListPlansForAdmin(PlanesDTO plansToSearch)
        {
            Result<PlanesDTO> result = new Result<PlanesDTO>();
            try
            {
                PlanesServices planService = new PlanesServices();
                plansToSearch.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona == Idioma.SinIdioma ? plansToSearch.IdiomaBase : UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                result.list = await planService.ListarPlanesAdministrador(plansToSearch);
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
        public async Task<JsonResult> GetListPlans(PlanesDTO plansToSearch)
        {
            Result<PlanesDTO> result = new Result<PlanesDTO>();
            try
            {
                PlanesServices planService = new PlanesServices();
                plansToSearch.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                plansToSearch.CodigoPaisParaBuscarMoneda = UserLoggedIn().PersonaDelUsuario.CodigoPais;
                result.list = await planService.ListarPlanesPorIdioma(plansToSearch);
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
        public async Task<JsonResult> SavePlan()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PlanesServices plansService = new PlanesServices();
                PlanesDTO planToSave = Newtonsoft.Json.JsonConvert.DeserializeObject<PlanesDTO>(Request.Form["planToSave"]);
                planToSave.Archivos = new ArchivosDTO();
                planToSave.Archivos.ArchivoContenido = Helper.getBytesFromFile(Request.Files[0]);
                
                result.obj = await plansService.CrearPlan(planToSave);
                
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
        public async Task<JsonResult> UpdatePlan(PlanesDTO planToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PlanesServices plansService = new PlanesServices();                
                result.obj = await plansService.ModificarPlan(planToUpdate);
                if (result.obj == null)
                    return Json(Helper.returnErrorUpdatePlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorUpdatePlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> DeletePlan(PlanesDTO plansToDelete)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PlanesServices plansService = new PlanesServices();

                result.obj = await plansService.EliminarPlan(new PlanesDTO { Consecutivo = plansToDelete.Consecutivo, CodigoArchivo = plansToDelete.CodigoArchivo });

                if(result.obj == null)
                    return Json(Helper.returnErrorDeletePlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                return Json(Helper.returnSuccessDeleteObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> UpdateImagePlan()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PlanesServices plansService = new PlanesServices();
                PlanesDTO plan = Newtonsoft.Json.JsonConvert.DeserializeObject<PlanesDTO>(Request.Form["plan"]);
                plan.ArchivoContenido = Helper.getBytesFromFile(Request.Files[0]);
                result.obj = await plansService.ModificarArchivoPlan(plan);

                if (result.obj != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region List settings
        #region Abilities
        [HttpPost]
        public async Task<JsonResult> GetListAbilitiesByCategoryAndLenguage(CategoriasDTO category)
        {
            Result<HabilidadesDTO> result = new Result<HabilidadesDTO>();
            try
            {
                HabilidadesDTO abilitieToSearch = new HabilidadesDTO();
                abilitieToSearch.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                abilitieToSearch.CodigoCategoria = category.Consecutivo;
                HabilidadesServices abilitiesService = new HabilidadesServices();
                var listAbilities = await abilitiesService.ListarHabilidadesPorCodigoCategoriaAndIdioma(abilitieToSearch);
                result.list = listAbilities;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> GetContentAbility(HabilidadesDTO ability)
        {
            Result<HabilidadesContenidosDTO> result = new Result<HabilidadesContenidosDTO>();
            try
            {
                HabilidadesServices abilitiesService = new HabilidadesServices();
                result.list = await abilitiesService.ListarContenidoDeUnaHabilidad(ability.HabilidadesContenidos.First());
                if (result.list != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                {
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> SaveAbilitie(HabilidadesDTO abilitieToSave)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                HabilidadesServices abilitiesService = new HabilidadesServices();
                result.obj = await abilitiesService.CrearHabilidad(abilitieToSave);
                if (result.obj.Exitoso)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                {
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateAbilitie(HabilidadesDTO abilitieToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                HabilidadesServices abilitiesService = new HabilidadesServices();
                //abilitieToUpdate.CodigoCategoria = 0;
                result.obj = await abilitiesService.ModificarHabilidad(abilitieToUpdate);
                if (result.obj != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> DeleteAbility(HabilidadesDTO abilityToDelete)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                HabilidadesServices abilitiesService = new HabilidadesServices();
                abilityToDelete.HabilidadesContenidos = null;
                var res = await abilitiesService.EliminarHabilidad(abilityToDelete);
                if (res.Exitoso)
                    return Json(Helper.returnDeleteSuccess(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Categories
        [HttpGet]
        public async Task<JsonResult> GetListCategories()
        {
            Result<CategoriasDTO> result = new Result<CategoriasDTO>();
            try
            {
                CategoriasServices categoriesService = new CategoriasServices();
                var listCategories = await categoriesService.ListarCategoriasPorIdioma(new CategoriasDTO { IdiomaBase = Idioma.Español });
                result.list = listCategories;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> GetCategoryById(CategoriasDTO categoryToSearch)
        {
            Result<CategoriasDTO> result = new Result<CategoriasDTO>();
            try
            {
                CategoriasServices categoriesService = new CategoriasServices();
                var listCategories = await categoriesService.BuscarCategoria(categoryToSearch);
                if (listCategories != null)
                {
                    result.obj = listCategories;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveCategorie()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CategoriasServices categorieService = new CategoriasServices();
                CategoriasDTO categoryForSave = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoriasDTO>(Request.Form["categoryForSave"]);
                categoryForSave.Archivos = new ArchivosDTO();
                categoryForSave.Archivos.ArchivoContenido = Helper.getBytesFromFile(Request.Files[0]);
                result.obj = await categorieService.CrearCategoria(categoryForSave);
                if (result.obj.Exitoso && result.obj != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateCategorie(CategoriasContenidosDTO categoryToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CategoriasServices categoriesService = new CategoriasServices();

                var res = await categoriesService.ModificarCategoriaContenido(categoryToUpdate);
                if (res.Exitoso)
                    return Json(result, JsonRequestBehavior.AllowGet);

                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> DeleteCategorie(CategoriasDTO categoryToDelete)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CategoriasServices categoriesService = new CategoriasServices();

                var res = await categoriesService.EliminarCategoria(categoryToDelete);
                if (res.Exitoso)
                    return Json(Helper.returnSuccessDeleteObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> uploadImageCategory()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CategoriasServices categoriesService = new CategoriasServices();
                CategoriasDTO categoryToUpdate = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoriasDTO>(Request.Form["Categorias"]);
                categoryToUpdate.ArchivoContenido = Helper.getBytesFromFile(Request.Files[0]);
                var res = await categoriesService.ModificarArchivoCategoria(categoryToUpdate);
                if (res.Exitoso)
                    return Json(result, JsonRequestBehavior.AllowGet);

                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Countries
        [HttpPost]
        public async Task<JsonResult> GetCountryById(PaisesDTO countryForSearch)
        {
            Result<PaisesDTO> result = new Result<PaisesDTO>();
            try
            {
                AdministracionServices countryService = new AdministracionServices();
                result.obj = await countryService.BuscarPais(countryForSearch);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> UpdateCountry(PaisesDTO countryForUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices countryService = new AdministracionServices();
                result.obj = await countryService.ModificarPais(countryForUpdate);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveCountry()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices countryService = new AdministracionServices();
                PaisesDTO countryForSave = Newtonsoft.Json.JsonConvert.DeserializeObject<PaisesDTO>(Request.Form["countryForSave"]);
                countryForSave.Archivos = new ArchivosDTO();
                countryForSave.Archivos.ArchivoContenido = Helper.getBytesFromFile(Request.Files[0]);
                countryForSave.Monedas = null;
                result.obj = await countryService.CrearPais(countryForSave);
                if (result.obj.Exitoso)
                {
                    result.Message = "Se guardó la información correctamente";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteCountry(PaisesDTO countryForDelete)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices countryService = new AdministracionServices();
                countryForDelete.Monedas = null;
                countryForDelete.PaisesContenidos = null;
                result.obj = await countryService.EliminarPais(countryForDelete);
                if (!result.obj.Exitoso || result.obj == null)
                {
                    return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.Message = "Eliminación exitosa";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> uploadImageCountry()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices categoriesService = new AdministracionServices();
                PaisesDTO countryToUpdate = Newtonsoft.Json.JsonConvert.DeserializeObject<PaisesDTO>(Request.Form["Paises"]);
                countryToUpdate.ArchivoContenido = Helper.getBytesFromFile(Request.Files[0]);
                var res = await categoriesService.ModificarArchivoPais(countryToUpdate);
                if (res.Exitoso)
                    return Json(result, JsonRequestBehavior.AllowGet);

                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Currencies
        [HttpGet]
        public async Task<JsonResult> GetCurrencies()
        {
            Result<MonedasDTO> result = new Result<MonedasDTO>();
            try
            {
                PagosServices currencyService = new PagosServices();
                result.list = await currencyService.ListarMonedas();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveCurrency(MonedasDTO currencyToSave)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices currencyService = new PagosServices();
                currencyToSave.Paises = null;
                currencyToSave.Descripcion = null;
                result.obj = await currencyService.ModificarMoneda(currencyToSave);
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Terms & Conditions
        [HttpPost]
        public async Task<ActionResult> GetTermsAndCondiions(TerminosCondicionesDTO termsAndConditions)
        {
            Result<TerminosCondicionesDTO> result = new Result<TerminosCondicionesDTO>();
            try
            {
                AdministracionServices autenticateService = new AdministracionServices();
                if (UserLoggedIn() != null)
                    termsAndConditions.IdiomaDeLosTerminos = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                
                result.obj = await autenticateService.BuscarTerminosCondiciones(termsAndConditions);
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
        public async Task<ActionResult> GetListTermsAndCondiions()
        {
            Result<TerminosCondicionesDTO> result = new Result<TerminosCondicionesDTO>();
            try
            {
                AdministracionServices autenticateService = new AdministracionServices();
                result.list = await autenticateService.ListarTerminosCondiciones();
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
        public async Task<ActionResult> UpdateTermsAndCondiions(List<TerminosCondicionesDTO> termsAndConditionsToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AdministracionServices autenticateService = new AdministracionServices();
                result.obj = await autenticateService.AsignarTerminosCondicionesLista(termsAndConditionsToUpdate);
                if (result.obj != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #endregion

        #region Tips
        [HttpPost]
        public async Task<JsonResult> CreatePost(NoticiasDTO news)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                NoticiasServices newsService = new NoticiasServices();
                news.CodigoUsuario = UserLoggedIn().Consecutivo;
                if(news.Consecutivo != 0)
                    result.obj = await newsService.ModificarNoticia(news);
                else
                    result.obj = await newsService.CrearNoticia(news);

                if(result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> DeletePost(NoticiasDTO news)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                NoticiasServices newsService = new NoticiasServices();

                result.obj = await newsService.EliminarNoticia(news);

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
        [HttpPost]
        public async Task<JsonResult> GetListNews(BuscadorDTO news)
        {
            Result<NoticiasDTO> result = new Result<NoticiasDTO>();
            try
            {
                news.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;

                NoticiasServices newsService = new NoticiasServices();
                result.list = await newsService.ListarNoticias(news);

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
        public async Task<JsonResult> UpdateFilePost(NoticiasDTO news)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices fileService = new ArchivosServices();
                result.obj = await fileService.ModificarArchivoNoticia(
                    Helper.getFileType(Request.Files[0].FileName),
                    int.Parse(Request.Form["Consecutivo"].ToString()),
                    int.Parse(Request.Form["CodigoArchivo"].ToString()),
                    Request.Files[0].InputStream);

                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public async Task<JsonResult> GetListRssFeed()
        {
            Result<RssFeedsDTO> result = new Result<RssFeedsDTO>();
            try
            {
                NoticiasServices newsService = new NoticiasServices();
                result.list = await newsService.ListarRssFeed();

                if (result.list != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> CreateRssFeed(List<RssFeedsDTO> rssFeedToSave)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            string urlFeedError = "";
            try
            {
                foreach (var rss in rssFeedToSave)
                {
                    urlFeedError = rss.UrlFeed;
                    FeedReader reader = new FeedReader(true);
                    var items = reader.RetrieveFeed(rss.UrlFeed);
                }

                NoticiasServices newsService = new NoticiasServices();
                result.obj = await newsService.InteractuarRssFeed(rssFeedToSave);

                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("XmlReader"))
                {
                    result.Message = "Error URL: " + urlFeedError;
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> DeleteRssFeed(RssFeedsDTO rssFeedToDelete)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                NoticiasServices newsService = new NoticiasServices();
                result.obj = await newsService.EliminarRssFeed(rssFeedToDelete);

                if (result.obj == null)
                    return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(Helper.returnDeleteSuccess(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Payment Methods
        [HttpGet]
        public async Task<JsonResult> GetInvoiceFormat()
        {
            Result<FacturaFormatoDTO> result = new Result<FacturaFormatoDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                result.list = await payService.ListarFacturasFormatos();

                List<FacturaFormatoDTO> facturaList = new List<FacturaFormatoDTO>();

                var newlist = result.list.GroupBy(x => x.CodigoPais)
                   .Select(pais => pais.First())
                   .ToList();
                
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
        public async Task<JsonResult> SaveInvoiceFormat(List<FacturaFormatoDTO> invoiceFormatToSave)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                result.obj = await payService.AsignarFacturaFormatoLista(invoiceFormatToSave);
                if (result.obj != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> UpdateInvoiceFormat(FacturaFormatoDTO invoiceFormatToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                result.obj = await payService.AsignarFacturaFormato(invoiceFormatToUpdate);
                if (result.obj != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> DeleteInvoiceFormat(FacturaFormatoDTO invoiceFormatToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                result.obj = await payService.AsignarFacturaFormato(invoiceFormatToUpdate);
                if (result.obj != null)
                    return Json(Helper.returnDeleteSuccess(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> GetPaymentHistory(BuscadorDTO searcher)
        {
            Result<HistorialPagosPersonasDTO> result = new Result<HistorialPagosPersonasDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                searcher.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.list = await payService.ListarHistorialPagosPersonas(searcher);
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
        public async Task<JsonResult> UpdatePaymentHistory(HistorialPagosPersonasDTO paymentHistory)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                result.obj = await payService.ModificarEstadoPagoPersona(paymentHistory);
                if (result.list != null)
                    return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        #endregion

    }
}