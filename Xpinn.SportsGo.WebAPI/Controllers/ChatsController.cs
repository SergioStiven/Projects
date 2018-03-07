using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using Microsoft.AspNet.SignalR;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class ChatsController : ApiController
    {
        ChatsBusiness _chatBusiness;

        public ChatsController()
        {
            _chatBusiness = new ChatsBusiness();
        }


        #region Metodos Contactos


        public async Task<IHttpActionResult> CrearContacto(Contactos contactoParaCrear)
        {
            if (contactoParaCrear == null || contactoParaCrear.CodigoPersonaContacto <= 0 || contactoParaCrear.CodigoPersonaOwner <= 0)
            {
                return BadRequest("contactoParaCrear vacio y/o invalido!.");
            }

            try
            {
                Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones> tupleWrapper = await _chatBusiness.CrearContacto(contactoParaCrear);

                if (tupleWrapper.Item1.Exitoso && tupleWrapper.Item2 != null)
                {
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

        public async Task<IHttpActionResult> BuscarContacto(Contactos contactoParaBuscar)
        {
            if (contactoParaBuscar == null || contactoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("contactoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Contactos contactoBuscado = await _chatBusiness.BuscarContacto(contactoParaBuscar);

                return Ok(contactoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarSiLaPersonaEstaAgregadaContactos(Contactos contactoParaBuscar)
        {
            if (contactoParaBuscar == null || contactoParaBuscar.CodigoPersonaContacto <= 0 || contactoParaBuscar.CodigoPersonaOwner <= 0)
            {
                return BadRequest("contactoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Contactos contactoBuscado = await _chatBusiness.VerificarSiLaPersonaEstaAgregadaContactos(contactoParaBuscar);

                return Ok(contactoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarContactos(Contactos contactoParaListar)
        {
            if (contactoParaListar == null || contactoParaListar.CodigoPersonaOwner <= 0 || contactoParaListar.SkipIndexBase < 0 || contactoParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("contactoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<ContactosDTO> listaContactos = await _chatBusiness.ListarContactos(contactoParaListar);

                return Ok(listaContactos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarContacto(Contactos contactoParaEliminar)
        {
            if (contactoParaEliminar == null || contactoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("contactoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones> tupleWrapper = await _chatBusiness.EliminarContacto(contactoParaEliminar);

                if (tupleWrapper.Item1.Exitoso && tupleWrapper.Item2 != null)
                {
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


        #region Metodos Chats


        public async Task<IHttpActionResult> CrearChat(Chats chatParaCrear)
        {
            if (chatParaCrear == null || chatParaCrear.CodigoPersonaOwner <= 0 || chatParaCrear.CodigoPersonaNoOwner <= 0)
            {
                return BadRequest("chatParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearChat = await _chatBusiness.CrearChat(chatParaCrear);

                return Ok(wrapperCrearChat);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarChat(Chats chatParaBuscar)
        {
            if (chatParaBuscar == null || chatParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("chatParaBuscar vacio y/o invalido!.");
            }

            try
            {
                ChatsDTO chatBuscado = await _chatBusiness.BuscarChat(chatParaBuscar);

                return Ok(chatBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarChatEntreDosPersonas(Chats chatParaBuscar)
        {
            if (chatParaBuscar == null || chatParaBuscar.CodigoPersonaOwner <= 0 || chatParaBuscar.CodigoPersonaNoOwner <= 0)
            {
                return BadRequest("chatParaBuscar vacio y/o invalido!.");
            }

            try
            {
                ChatsDTO chatBuscado = await _chatBusiness.BuscarChatEntreDosPersonas(chatParaBuscar);

                return Ok(chatBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarChats(Chats chatsParaListar)
        {
            if (chatsParaListar == null || chatsParaListar.CodigoPersonaOwner <= 0
                || chatsParaListar.SkipIndexBase < 0 || chatsParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("chatsParaListar vacio y/o invalido!.");
            }

            try
            {
                List<ChatsDTO> listaChats = await _chatBusiness.ListarChats(chatsParaListar);

                return Ok(listaChats);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarChat(Chats chatParaEliminar)
        {
            if (chatParaEliminar == null || chatParaEliminar.Consecutivo <= 0 || chatParaEliminar.CodigoChatRecibe <= 0
                || chatParaEliminar.CodigoPersonaOwner <= 0 || chatParaEliminar.CodigoPersonaNoOwner <= 0)
            {
                return BadRequest("chatParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarChat = await _chatBusiness.EliminarChat(chatParaEliminar);

                return Ok(wrapperEliminarChat);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos ChatsMensajes


        public async Task<IHttpActionResult> CrearChatMensaje(ChatsMensajes chatMensajeParaCrear)
        {
            if (chatMensajeParaCrear == null || string.IsNullOrWhiteSpace(chatMensajeParaCrear.Mensaje)
                || chatMensajeParaCrear.CodigoChatEnvia <= 0 || chatMensajeParaCrear.CodigoChatRecibe <= 0)
            {
                return BadRequest("chatMensajeParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearChat = await _chatBusiness.CrearChatMensaje(chatMensajeParaCrear);

                return Ok(wrapperCrearChat);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarChatMensaje(ChatsMensajes chatMensajeParaBuscar)
        {
            if (chatMensajeParaBuscar == null || chatMensajeParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("chatMensajeParaBuscar vacio y/o invalido!.");
            }

            try
            {
                ChatsMensajes chatMensajeBuscado = await _chatBusiness.BuscarChatMensaje(chatMensajeParaBuscar);

                return Ok(chatMensajeBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarChatsMensajes(ChatsMensajes chatMensajeParaListar)
        {
            if (chatMensajeParaListar == null || chatMensajeParaListar.CodigoChatEnvia <= 0 || chatMensajeParaListar.CodigoChatRecibe <= 0
                || chatMensajeParaListar.SkipIndexBase < 0 || chatMensajeParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("chatMensajeParaListar vacio y/o invalido!.");
            }

            try
            {
                List<ChatsMensajes> listaChatMensajes = await _chatBusiness.ListarChatsMensajes(chatMensajeParaListar);

                return Ok(listaChatMensajes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarChatMensaje(ChatsMensajes chatMensajeParaEliminar)
        {
            if (chatMensajeParaEliminar == null || chatMensajeParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("chatMensajeParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarChatMensaje = await _chatBusiness.EliminarChatMensaje(chatMensajeParaEliminar);

                return Ok(wrapperEliminarChatMensaje);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
