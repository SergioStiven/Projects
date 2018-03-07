//using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.MVC.Models;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class ChatController : BaseController
    {
        // GET: Chat
        public async Task<ActionResult> Index()
        {
            if (await ValidateIfOperationIsSupportedByPlan(Util.Portable.Enums.TipoOperacion.ServiciosChat))
                return View();
            ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            return View("Unauthorized");
        }

        #region Messages
        [HttpPost]
        public async Task<JsonResult> SaveChat(ChatsDTO chat)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                chat.CodigoPersonaOwner = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj = await chatService.CrearChat(chat);

                if (result.list == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveMessage(ChatsMensajesDTO chatMessage)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                result.obj = await chatService.CrearChatMensaje(chatMessage);

                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetChatMessages(ChatsMensajesDTO chat)
        {
            Result<ChatsMensajesDTO> result = new Result<ChatsMensajesDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                result.list = await chatService.ListarChatsMensajes(chat);

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
        public async Task<JsonResult> DeleteConversation(ChatsDTO chat)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                result.obj = await chatService.EliminarChat(chat);

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

        #region Contacts
        [HttpPost]
        public async Task<JsonResult> GetListContacs(ContactosDTO contacts)
        {
            Result<ContactosDTO> result = new Result<ContactosDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                contacts.CodigoPersonaOwner = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                contacts.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdiomaUsuarioBase;
                result.list = await chatService.ListarContactos(contacts);

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
        public async Task<JsonResult> GetChatFromContact(ChatsDTO chat)
        {
            Result<ChatsDTO> result = new Result<ChatsDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                chat.CodigoPersonaOwner = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj = await chatService.BuscarChatEntreDosPersonas(chat);

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

        [HttpPost]
        public async Task<JsonResult> GetAllChats(ChatsDTO chat)
        {
            Result<ChatsDTO> result = new Result<ChatsDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                chat.CodigoPersonaOwner = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.list = await chatService.ListarChats(chat);

                if (result.list == null)
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}