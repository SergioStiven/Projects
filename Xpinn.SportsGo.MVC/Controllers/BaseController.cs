using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public abstract class BaseController : Controller
    {

        #region Route Security
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Authenticate")
                && !filterContext.ActionDescriptor.ActionName.Equals("GetTermsAndCondiions"))
            {
                if (Session["UserLogin"] != null)
                {
                    if (UserLoggedIn().PersonaDelUsuario.Consecutivo == 0)
                    {
                        switch (UserLoggedIn().TipoPerfil)
                        {
                            case TipoPerfil.Candidato:
                            case TipoPerfil.Grupo:
                            case TipoPerfil.Representante:
                                if ((!filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Profile") && filterContext.ActionDescriptor.ActionName.Equals("Index"))
                                    || (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Administration") && ValidateActionControllerAdministration(filterContext))
                                    || (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Advertisements")
                                        && (ValidateActionControllerAdvertisements(filterContext) || filterContext.ActionDescriptor.ActionName.Equals("SignInadvertiser"))))
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Profile" }, { "action", "Index" } });
                                break;
                            case TipoPerfil.Anunciante:
                                if ((!filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Advertisements") && filterContext.ActionDescriptor.ActionName.Equals("Index"))
                                    || (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Administration") && ValidateActionControllerAdministration(filterContext))
                                    || (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Advertisements") && ValidateActionControllerAdvertisements(filterContext)))
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Advertisements" }, { "action", "SignInadvertiser" } });
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (UserLoggedIn().TipoPerfil)
                        {
                            case TipoPerfil.Candidato:
                            case TipoPerfil.Grupo:
                            case TipoPerfil.Representante:
                                if ((filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Administration") && ValidateActionControllerAdministration(filterContext))
                                    || (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Advertisements")
                                    && (ValidateActionControllerAdvertisements(filterContext) || filterContext.ActionDescriptor.ActionName.Equals("SignInadvertiser"))))
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                                break;
                            case TipoPerfil.Anunciante:
                                if ((!filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Advertisements") && filterContext.ActionDescriptor.ActionName.Equals("Index"))
                                    || (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Administration") && ValidateActionControllerAdministration(filterContext)))
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Advertisements" }, { "action", "Index" } });
                                break;
                            default:
                                break;
                        }
                    }

                }
                else
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Authenticate" }, { "action", "Index" } });
            }
        }

        private bool ValidateActionControllerAdministration(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.Equals("Index") || filterContext.ActionDescriptor.ActionName.Equals("Ads")
                || filterContext.ActionDescriptor.ActionName.Equals("Events") || filterContext.ActionDescriptor.ActionName.Equals("Users")
                || filterContext.ActionDescriptor.ActionName.Equals("List") || filterContext.ActionDescriptor.ActionName.Equals("PaymentMethods")
                || filterContext.ActionDescriptor.ActionName.Equals("PaymentApproval") || filterContext.ActionDescriptor.ActionName.Equals("PaymentPlans")
                || filterContext.ActionDescriptor.ActionName.Equals("News"))
                return true;
            else
                return false;
        }

        private bool ValidateActionControllerAdvertisements(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.Equals("Index")
                || filterContext.ActionDescriptor.ActionName.Equals("Dashboard") || filterContext.ActionDescriptor.ActionName.Equals("MyAds")
                || filterContext.ActionDescriptor.ActionName.Equals("CreateAdvertiser") || filterContext.ActionDescriptor.ActionName.Equals("Posts")
                || filterContext.ActionDescriptor.ActionName.Equals("Pricing") || filterContext.ActionDescriptor.ActionName.Equals("Settings")
                || filterContext.ActionDescriptor.ActionName.Equals("HistoryOfMyPlans"))
                return true;
            else
                return false;
        }
        
        public async Task<bool> ValidateIfOperationIsSupportedByPlan(TipoOperacion typeOperation)
        {
            try
            {
                PlanesServices planService = new PlanesServices();
                WrapperSimpleTypesDTO result = await planService.VerificarSiPlanSoportaLaOperacion(new PlanesUsuariosDTO()
                {
                    Consecutivo = UserLoggedIn().CodigoPlanUsuario,
                    TipoOperacionBase = typeOperation
                });
                if (result != null && result.EsPosible)
                    return true;
                return false;
                
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Variables in session
        public UsuariosDTO UserLoggedIn()
        {
            return (UsuariosDTO)Session["UserLogin"];
        }

        public void setUserLogin(UsuariosDTO userLoggedIn)
        {
            Session["UserLogin"] = userLoggedIn;
        }

        public PersonasDTO GetPersonToSearch()
        {
            return (PersonasDTO)Session["PersonToSearch"];
        }

        public PersonasDTO GetPersonToSearchTemp()
        {
            return (PersonasDTO)Session["PersonToSearchTemp"];
        }

        public void SetPersonToSearch(PersonasDTO PersonToSearch)
        {
            if (PersonToSearch != null)
                PersonToSearch.CodigoIdiomaUsuarioBase = 1;
            Session["PersonToSearch"] = PersonToSearch;
            Session["PersonToSearchTemp"] = PersonToSearch;
        }
        
        #endregion
        
    }
}