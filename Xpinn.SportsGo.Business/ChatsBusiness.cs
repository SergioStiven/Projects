using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Business
{
    public class ChatsBusiness
    {


        #region Metodos Contactos


        public async Task<Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones>> CrearContacto(Contactos contactoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);

                int codigoPersonaContacto = contactoParaCrear.CodigoPersonaContacto;
                int codigoPersonaOwner = contactoParaCrear.CodigoPersonaOwner;

                chatsRepo.CrearContacto(contactoParaCrear);

                Contactos contactoSegundoParaCrear = new Contactos
                {
                    CodigoPersonaContacto = codigoPersonaOwner,
                    CodigoPersonaOwner = codigoPersonaContacto
                };

                chatsRepo.CrearContacto(contactoSegundoParaCrear);

                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                Notificaciones notificacion = new Notificaciones
                {
                    CodigoTipoNotificacion = (int)TipoNotificacionEnum.PersonaAgregada,
                    CodigoPersonaOrigenAccion = codigoPersonaOwner,
                    CodigoPersonaDestinoAccion = codigoPersonaContacto,
                    Creacion = DateTime.Now
                };

                noticiasRepo.CrearNotificacion(notificacion);

                WrapperSimpleTypesDTO wrapperCrearContacto = new WrapperSimpleTypesDTO();
                TimeLineNotificaciones timeLineNotificacion = null;

                wrapperCrearContacto.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearContacto.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearContacto.Exitoso = true;
                    wrapperCrearContacto.ConsecutivoCreado = contactoParaCrear.Consecutivo;

                    if (notificacion.Consecutivo > 0)
                    {
                        timeLineNotificacion = new TimeLineNotificaciones(await noticiasRepo.BuscarNotificacion(notificacion));
                    }
                }

                return Tuple.Create(wrapperCrearContacto, timeLineNotificacion);
            }
        }

        public async Task<Contactos> BuscarContacto(Contactos contactoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);
                Contactos contactoBuscado = await chatsRepo.BuscarContacto(contactoParaBuscar);

                return contactoBuscado;
            }
        }

        public async Task<Contactos> VerificarSiLaPersonaEstaAgregadaContactos(Contactos contactoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);
                Contactos contactoBuscado = await chatsRepo.VerificarSiLaPersonaEstaAgregadaContactos(contactoParaBuscar);

                return contactoBuscado;
            }
        }

        public async Task<List<ContactosDTO>> ListarContactos(Contactos contactoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);
                List<ContactosDTO> listaContactos = await chatsRepo.ListarContactos(contactoParaListar);

                return listaContactos;
            }
        }

        public async Task<Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones>> EliminarContacto(Contactos contactoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);

                Contactos contactoFiltro = new Contactos
                {
                    Consecutivo = contactoParaEliminar.Consecutivo
                };

                chatsRepo.EliminarContacto(contactoFiltro);

                Tuple<Contactos, int?> tupleBusqueda = await chatsRepo.BuscarConsecutivoContactoContrario(contactoParaEliminar);

                Contactos contactoOwner = tupleBusqueda.Item1;
                int? consecutivoContrarioBuscado = tupleBusqueda.Item2;

                if (!consecutivoContrarioBuscado.HasValue)
                {
                    throw new InvalidOperationException("No existe el contacto contrario de esta persona!.");
                }

                Contactos contactoParaBorrar = new Contactos
                {
                    Consecutivo = consecutivoContrarioBuscado.Value
                };

                chatsRepo.EliminarContacto(contactoParaBorrar);

                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                Notificaciones notificacion = new Notificaciones
                {
                    CodigoTipoNotificacion = (int)TipoNotificacionEnum.PersonaEliminada,
                    CodigoPersonaOrigenAccion = contactoOwner.CodigoPersonaOwner,
                    CodigoPersonaDestinoAccion = contactoOwner.CodigoPersonaContacto,
                    Creacion = DateTime.Now
                };

                noticiasRepo.CrearNotificacion(notificacion);

                WrapperSimpleTypesDTO wrapperEliminarContacto = new WrapperSimpleTypesDTO();
                TimeLineNotificaciones timeLineNotificacion = null;

                wrapperEliminarContacto.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarContacto.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarContacto.Exitoso = true;

                    if (notificacion.Consecutivo > 0)
                    {
                        timeLineNotificacion = new TimeLineNotificaciones(await noticiasRepo.BuscarNotificacion(notificacion));
                    }
                }

                return Tuple.Create(wrapperEliminarContacto, timeLineNotificacion);
            }
        }


        #endregion


        #region Metodos Chats


        public async Task<WrapperSimpleTypesDTO> CrearChat(Chats chatParaCrearOwner)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);

                // Verifico si ya existe mi chat principal, si existe lo reactivo, si no lo creo
                bool existeChatOwner = await chatsRepo.VerificarSiYaExisteChat(chatParaCrearOwner);
                if (existeChatOwner)
                {
                    chatParaCrearOwner = await chatsRepo.ReactivarChat(chatParaCrearOwner);
                }
                else
                {
                    chatParaCrearOwner.EstadoChat = EstadosChat.Activo;
                    chatParaCrearOwner.Creacion = DateTime.Now;
                    chatsRepo.CrearChat(chatParaCrearOwner);
                }

                Chats chatNoOwner = new Chats
                {
                    CodigoPersonaOwner = chatParaCrearOwner.CodigoPersonaNoOwner,
                    CodigoPersonaNoOwner = chatParaCrearOwner.CodigoPersonaOwner
                };

                // Verifico si ya existe mi chat secundario de la persona a que le voy a enviar, si existe no hago nada, si no lo creo
                // Lo dejo en estado inactivo para que crear el chat principal no le cree un chat a la persona secundaria
                // Solo cuando un mensaje es mandado es que ambos son reactivados
                bool existeChatNoOwner = await chatsRepo.VerificarSiYaExisteChat(chatNoOwner);
                if (!existeChatNoOwner)
                {
                    chatNoOwner.EstadoChat = EstadosChat.PendienteParaBorrarMensajes;
                    chatNoOwner.Creacion = DateTime.Now;
                    chatsRepo.CrearChat(chatNoOwner);
                }

                WrapperSimpleTypesDTO wrapperCrearChat = new WrapperSimpleTypesDTO();

                wrapperCrearChat.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearChat.NumeroRegistrosAfectados > 0 || chatParaCrearOwner.Consecutivo > 0)
                {
                    wrapperCrearChat.Exitoso = true;
                    wrapperCrearChat.ConsecutivoCreado = chatParaCrearOwner.Consecutivo;

                    if (chatNoOwner.Consecutivo > 0)
                    {
                        wrapperCrearChat.ConsecutivoChatRecibe = chatNoOwner.Consecutivo;
                    }
                    else
                    {
                        wrapperCrearChat.ConsecutivoChatRecibe = await chatsRepo.BuscarConsecutivoChat(chatNoOwner);
                    }
                }

                return wrapperCrearChat;
            }
        }

        public async Task<ChatsDTO> BuscarChat(Chats chatParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);
                ChatsDTO chatBuscado = await chatsRepo.BuscarChat(chatParaBuscar);

                return chatBuscado;
            }
        }

        public async Task<ChatsDTO> BuscarChatEntreDosPersonas(Chats chatParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);
                ChatsDTO chatBuscado = await chatsRepo.BuscarChatEntreDosPersonas(chatParaBuscar);

                if (chatBuscado == null)
                {
                    chatBuscado = new ChatsDTO();
                }

                return chatBuscado;
            }
        }

        public async Task<List<ChatsDTO>> ListarChats(Chats chatParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (chatParaListar.FechaFiltroBase != DateTime.MinValue)
                {
                    chatParaListar.FechaFiltroBase = helper.ConvertDateTimeFromAnotherTimeZone(chatParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, chatParaListar.FechaFiltroBase);
                }

                List<ChatsDTO> listaChats = await chatsRepo.ListarChats(chatParaListar);

                if (listaChats != null && listaChats.Count > 0)
                {    
                    foreach (var chat in listaChats)
                    {
                        chat.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(chatParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, chat.Creacion);

                        if (chat.UltimoMensaje != null)
                        {
                            chat.UltimoMensaje.FechaMensaje = helper.ConvertDateTimeFromAnotherTimeZone(chatParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, chat.UltimoMensaje.FechaMensaje);
                        }
                    }
                }

                return listaChats;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarChat(Chats contactoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);

                bool verificarSiSePuedenBorrarMensajesNoOwner = await chatsRepo.VerificarSiSePuedenBorrarMensajesNoOwner(contactoParaEliminar);

                if (verificarSiSePuedenBorrarMensajesNoOwner)
                {
                    ChatsMensajes chatMensajes = new ChatsMensajes
                    {
                        CodigoChatEnvia = contactoParaEliminar.Consecutivo,
                        CodigoChatRecibe = contactoParaEliminar.CodigoChatRecibe
                    };
                    chatsRepo.EliminarTodosChatMensaje(chatMensajes);

                    Chats chatRecibe = new Chats
                    {
                        Consecutivo = contactoParaEliminar.CodigoChatRecibe
                    };
                    chatsRepo.EliminarChat(chatRecibe);

                    chatsRepo.EliminarChat(contactoParaEliminar);
                }
                else
                {
                    Chats chatExistente = await chatsRepo.MarcarChatComoPendienteParaBorrarMensajes(contactoParaEliminar);
                }

                WrapperSimpleTypesDTO wrapperEliminarChat = new WrapperSimpleTypesDTO();

                wrapperEliminarChat.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarChat.NumeroRegistrosAfectados > 0) wrapperEliminarChat.Exitoso = true;

                return wrapperEliminarChat;
            }
        }


        #endregion


        #region Metodos ChatMensajes


        public async Task<WrapperSimpleTypesDTO> CrearChatMensaje(ChatsMensajes chatMensajeParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);

                Chats chatOwner = new Chats
                {
                    Consecutivo = chatMensajeParaCrear.CodigoChatEnvia
                };

                Chats chatNoOwner = new Chats
                {
                    Consecutivo = chatMensajeParaCrear.CodigoChatRecibe
                };

                await chatsRepo.ReactivarChatPorConsecutivo(chatOwner);
                await chatsRepo.ReactivarChatPorConsecutivo(chatNoOwner);

                chatMensajeParaCrear.FechaMensaje = DateTime.Now;
                chatsRepo.CrearChatMensaje(chatMensajeParaCrear);

                WrapperSimpleTypesDTO wrapperCrearChatMensaje = new WrapperSimpleTypesDTO();

                wrapperCrearChatMensaje.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearChatMensaje.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearChatMensaje.Exitoso = true;
                    wrapperCrearChatMensaje.ConsecutivoCreado = chatMensajeParaCrear.Consecutivo;
                }

                return wrapperCrearChatMensaje;
            }
        }

        public async Task<ChatsMensajes> BuscarChatMensaje(ChatsMensajes chatMensajeParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);
                ChatsMensajes chatMensajeBuscado = await chatsRepo.BuscarChatMensaje(chatMensajeParaBuscar);

                if (chatMensajeBuscado != null)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    chatMensajeBuscado.FechaMensaje = helper.ConvertDateTimeFromAnotherTimeZone(chatMensajeParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, chatMensajeBuscado.FechaMensaje);
                }

                return chatMensajeBuscado;
            }
        }

        public async Task<List<ChatsMensajes>> ListarChatsMensajes(ChatsMensajes chatMensajeParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                ChatsRepository chatsRepo = new ChatsRepository(context);

                if (chatMensajeParaListar.FechaFiltroBase != DateTime.MinValue)
                {
                    chatMensajeParaListar.FechaFiltroBase = helper.ConvertDateTimeFromAnotherTimeZone(chatMensajeParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, chatMensajeParaListar.FechaFiltroBase);
                }

                List<ChatsMensajes> listaMensajes = await chatsRepo.ListarChatsMensajes(chatMensajeParaListar);

                if (listaMensajes != null && listaMensajes.Count > 0)
                {
                    foreach (var mensaje in listaMensajes)
                    {
                        mensaje.FechaMensaje = helper.ConvertDateTimeFromAnotherTimeZone(chatMensajeParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, mensaje.FechaMensaje);
                    }
                }

                return listaMensajes;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarChatMensaje(ChatsMensajes chatMensajeParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ChatsRepository chatsRepo = new ChatsRepository(context);

                chatsRepo.EliminarChatMensaje(chatMensajeParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarChatMensaje = new WrapperSimpleTypesDTO();

                wrapperEliminarChatMensaje.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarChatMensaje.NumeroRegistrosAfectados > 0) wrapperEliminarChatMensaje.Exitoso = true;

                return wrapperEliminarChatMensaje;
            }
        }


        #endregion


    }
}
