using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.MVC.Models;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class SettingsController : BaseController
    {
        #region Views
        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }
        // GET: Option pay manual plan
        public ActionResult PayManual()
        {
            return View();
        }
        // GET: List of plans
        public ActionResult ListOfPlans()
        {
            return View();
        }
        // GET: My Plan
        public ActionResult MyPlan()
        {
            return View();
        }
        // GET: History of my plans
        public ActionResult HistoryOfMyPlans()
        {
            return View();
        }
        #endregion

        #region Methods
        [HttpPost]
        public async Task<JsonResult> GetListPlansByProfile(PlanesDTO plansToSearch)
        {
            Result<PlanesDTO> result = new Result<PlanesDTO>();
            try
            {
                PlanesServices planService = new PlanesServices();
                plansToSearch.IdiomaBase = plansToSearch.IdiomaBase == 0 ? UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona : plansToSearch.IdiomaBase;
                plansToSearch.CodigoTipoPerfil = plansToSearch.CodigoTipoPerfil == 0 ? UserLoggedIn().CodigoTipoPerfil : plansToSearch.CodigoTipoPerfil;
                plansToSearch.CodigoPaisParaBuscarMoneda = plansToSearch.CodigoPaisParaBuscarMoneda == 0 ? UserLoggedIn().PersonaDelUsuario.CodigoPais : plansToSearch.CodigoPaisParaBuscarMoneda;
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
        public async Task<JsonResult> ChangeUser(UsuariosDTO user)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();
                user.Consecutivo = UserLoggedIn().Consecutivo;
                result.obj = await service.ModificarUsuario(user);
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
        public async Task<JsonResult> ChangeLanguage(PersonasDTO personToUpdate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PersonasServices personService = new PersonasServices();
                PersonasDTO myPerson = await personService.BuscarPersona(UserLoggedIn().PersonaDelUsuario);
                myPerson.CodigoIdioma = personToUpdate.CodigoIdioma;
                result.obj = await personService.ModificarPersona(myPerson);
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(personToUpdate.CodigoIdioma), JsonRequestBehavior.AllowGet);
                UserLoggedIn().PersonaDelUsuario.CodigoIdioma = personToUpdate.CodigoIdioma;
                return Json(Helper.returnSuccessObj(personToUpdate.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> SearchInvoiceFormat()
        {
            Result<FacturaFormatoDTO> result = new Result<FacturaFormatoDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                FacturaFormatoDTO invoiceFormat = new FacturaFormatoDTO();
                invoiceFormat.CodigoIdioma = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                invoiceFormat.CodigoPais = UserLoggedIn().PersonaDelUsuario.CodigoPais == 0 ? 1 : UserLoggedIn().PersonaDelUsuario.CodigoPais;
                result.obj = await payService.BuscarFacturaFormato(invoiceFormat);
                
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
        public async Task<JsonResult> SavePaymentHistory(HistorialPagosPersonasDTO paymentHistory)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                paymentHistory.CodigoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                paymentHistory.CodigoPais = UserLoggedIn().PersonaDelUsuario.CodigoPais;
                result.obj = await payService.CrearHistorialPagoPersona(paymentHistory);

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
        [HttpGet]
        public async Task<JsonResult> GetMyPlan()
        {
            try
            {
                PlanesServices planService = new PlanesServices();
                if(UserLoggedIn().CodigoPlanUsuario > 0)
                {
                    Result<PlanesUsuariosDTO> result = new Result<PlanesUsuariosDTO>();
                    PlanesUsuariosDTO myPlan = new PlanesUsuariosDTO();
                    myPlan.Consecutivo = UserLoggedIn().CodigoPlanUsuario;
                    myPlan.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                    result.obj = await planService.BuscarPlanUsuario(myPlan);
                    if (result.obj != null)
                        return Json(result, JsonRequestBehavior.AllowGet);
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Result<PlanesDTO> result = new Result<PlanesDTO>();
                    result.obj = await planService.BuscarPlanDefaultDeUnPerfil(new PlanesDTO() { TipoPerfil = UserLoggedIn().TipoPerfil });
                    if (result.obj != null)
                        return Json(result, JsonRequestBehavior.AllowGet);
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> ValidateIfIHaveAPlanInProcess()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices planService = new PagosServices();
                HistorialPagosPersonasDTO myPlan = new HistorialPagosPersonasDTO();
                myPlan.CodigoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj = await planService.VerificarQueNoExistaUnPagoEnTramite(myPlan);

                if (result.obj != null && result.obj.PagoEnTramite)
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
        public async Task<JsonResult> GetMyPlanInProcess(HistorialPagosPersonasDTO myPlan)
        {
            Result<HistorialPagosPersonasDTO> result = new Result<HistorialPagosPersonasDTO>();
            try
            {
                PagosServices planService = new PagosServices();
                myPlan.CodigoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                myPlan.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.obj = await planService.BuscarPagoEnTramiteDeUnaPersona(myPlan);

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
        public async Task<JsonResult> UpdatePaymentStatus(HistorialPagosPersonasDTO paymentHistory)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                paymentHistory.EstadoDelPago = EstadoDeLosPagos.PendientePorAprobar;
                result.obj = await payService.ModificarEstadoPagoPersona(paymentHistory);

                if (result.obj != null)
                    return Json(Helper.returnSuccessfulPayment(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                else
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> DeletePlanInProcess(HistorialPagosPersonasDTO paymentHistory)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                PagosServices payService = new PagosServices();
                result.obj = await payService.EliminarPagoPendientePorPagar(new HistorialPagosPersonasDTO()
                { CodigoPersona = paymentHistory.CodigoPersona, Consecutivo = paymentHistory .Consecutivo});

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
        public async Task<JsonResult> GetHistoryOfMyPlans(BuscadorDTO filter)
        {
            Result<HistorialPagosPersonasDTO> result = new Result<HistorialPagosPersonasDTO>();
            try
            {
                PagosServices planService = new PagosServices();
                filter.ConsecutivoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                filter.IdiomaBase = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                result.list = await planService.ListarHistorialPagosDeUnaPersona(filter);

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
        #endregion

    }
}