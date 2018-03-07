using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.MVC.Models;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class AuthenticateController : BaseController
    {
        #region Views
        // GET: Authenticate
        public async Task<ActionResult> Index()
        {
            if (System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName.ToString()] != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName.ToString()].Value);
                AuthenticateServices service = new AuthenticateServices();
                var user = await service.VerificarUsuario(new UsuariosDTO() { Usuario = authTicket.Name, Clave = authTicket.UserData });
                if (user == null)
                {
                    ClearSession();
                    return RedirectToAction("Index", "Authenticate");
                }
                setUserLogin(user);
                return RedirectToAction("Index", "Home");
            }
            ClearSession();
            return View();
        }

        // GET: Confirmation Of Registration
        public async Task<ActionResult> ConfirmationOfRegistration(int ID, int Language)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();
                result.obj = await service.ActivarUsuario(new UsuariosDTO() { Consecutivo = ID });
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }
        #endregion

        #region Public Methods
        [HttpPost]
        public async Task<JsonResult> Login(UsuariosDTO userToValidate)
        {
            Result<UsuariosDTO> result = new Result<UsuariosDTO>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();
                result.obj = await service.VerificarUsuario(userToValidate);
                if (result.obj != null)
                {
                    WrapperSimpleTypesDTO isActive = await service.VerificarSiCuentaEstaActiva(userToValidate);
                    if (isActive.Existe)
                    {
                        setUserLogin(result.obj);
                        result.Path = pathByTypeProfile(result.obj.TipoPerfil);
                        /* If remember user and password save user in cookie encrypted */
                        if (userToValidate.EsRecordarClave) saveUserInCookie(result.obj);
                    }
                    else
                    {
                        result.Success = false;
                        result.obj = null;
                        result.Message = "NOTI_USER_BLOCK";
                    }
                }
                else
                {
                    return Json(Helper.returnErrorWrongUser(), JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorWrongUser(), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> ConfirmRegistration(UsuariosDTO userToValidate)
        {
            Result<Object> result = new Result<Object>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();

                WrapperSimpleTypesDTO emailExist = await service.VerificarSiEmailYaExiste(userToValidate);
                if (emailExist == null || emailExist.Existe)
                {
                    result.Success = false;
                    result.Message = "NOTI_EMAIL_REGISTERED";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                WrapperSimpleTypesDTO userExist = await service.VerificarSiUsuarioYaExiste(userToValidate);
                if (userExist == null || userExist.Existe)
                {
                    result.Success = false;
                    result.Message = "NOTI_USER_REGISTERED";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> ValidateIfEmailExist(UsuariosDTO emailToValidate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();

                result.obj = await service.VerificarSiEmailYaExiste(emailToValidate);
                if (result.obj == null || result.obj.Existe)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
               
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> ValidateIfUserExist(UsuariosDTO userToValidate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();

                result.obj = await service.VerificarSiUsuarioYaExiste(userToValidate);
                if (result.obj == null || result.obj.Existe)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SingInTemporaly(UsuariosDTO userToRegister)
        {
            Result<UsuariosDTO> result = new Result<UsuariosDTO>();
            try
            {
                setUserLogin(userToRegister);
                if(userToRegister.TipoPerfil == TipoPerfil.Anunciante)
                    result.Path = "Advertisements/SignInadvertiser/";
                else
                    result.Path = "Profile/";
                return Json(result, JsonRequestBehavior.AllowGet);
            }   
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetProfileTypes()
        {
            Result<TiposPerfilesDTO> result = new Result<TiposPerfilesDTO>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();

                result.list = await service.ListarTiposPerfiles();
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
        public async Task<JsonResult> RecoverPassword(UsuariosDTO userToValidate)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                AuthenticateServices service = new AuthenticateServices();

                result.obj = await service.RecuperarClave(userToValidate);
                if (result.obj == null || !result.obj.Exitoso)
                    return Json(Helper.returnSendMailError(), JsonRequestBehavior.AllowGet);
                return Json(Helper.returnSendMailSuccess(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnSendMailError(), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult Logoff()
        {
            ClearSession();
            return RedirectToAction("Index", "Authenticate");
        }
        #endregion

        #region Private Methods
        private void saveUserInCookie(UsuariosDTO user)
        {
            int timeout = 525600; // Timeout in minutes, 525600 = 365 days.
            var ticket = new FormsAuthenticationTicket(1, user.Usuario, DateTime.Now, DateTime.Now.AddYears(1), true, user.Clave);
            string encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
            cookie.Expires = DateTime.Now.AddMinutes(timeout);
            cookie.HttpOnly = true; // cookie not available in javascript.
            Response.Cookies.Add(cookie);
        }

        private void ClearSession()
        {
            // Delete the user details from cache.
            Session.Abandon();
            // Delete the authentication ticket and sign out.
            FormsAuthentication.SignOut();
            // Clear authentication cookie.
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);
        }

        private string pathByTypeProfile(TipoPerfil typeProfile)
        {
            switch (typeProfile)
            {
                case TipoPerfil.SinTipoPerfil:
                    return "Authenticate/";
                case TipoPerfil.Candidato:
                    return "Home/";
                case TipoPerfil.Grupo:
                    return "Home/";
                case TipoPerfil.Representante:
                    return "Home/";
                case TipoPerfil.Anunciante:
                    return "Advertisements/";
                case TipoPerfil.Administrador:
                    return "Administration/";
                default:
                    return "Authenticate/";
            }
        }
        #endregion
    }
}