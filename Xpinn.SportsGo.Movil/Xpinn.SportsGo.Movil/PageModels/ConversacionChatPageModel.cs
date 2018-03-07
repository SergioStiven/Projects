using FreshMvvm;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ConversacionChatPageModel : BasePageModel
    {
        ChatsServices _chatsServices;
        ILockeable _lockeable;
        IDateTimeHelper _dateTimeHelper;

        public ChatsDTO Chat { get; set; }
        public ObservableRangeCollection<ChatMensajesModel> Mensajes { get; set; }

        public bool NoHayNadaMasParaCargar { get; set; }
        public string TextoParaEnviar { get; set; }

        public string UrlMiImagenPerfil
        {
            get
            {
                string urlMiImagenPerfil = App.Persona.UrlImagenPerfil;

                if (string.IsNullOrWhiteSpace(urlMiImagenPerfil))
                {
                    urlMiImagenPerfil = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
                }

                return urlMiImagenPerfil;
            }
        }

        public string MiNombre
        {
            get
            {
                return App.Persona.NombreYApellido;
            }
        }

        public string UrlImagenPerfilDestino
        {
            get
            {
                string urlImagenPerfilDestino = string.Empty;

                if (Chat != null)
                {
                    urlImagenPerfilDestino = Chat.PersonasNoOwner.UrlImagenPerfil;
                }

                if (string.IsNullOrWhiteSpace(urlImagenPerfilDestino))
                {
                    urlImagenPerfilDestino = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
                }

                return urlImagenPerfilDestino;
            }
        }

        public string NombreDestino
        {
            get
            {
                string nombreDestino = string.Empty;

                if (Chat != null)
                {
                    nombreDestino = Chat.PersonasNoOwner.NombreYApellido;
                }

                return nombreDestino;
            }
        }

        public ConversacionChatPageModel()
        {
            _chatsServices = new ChatsServices();
            _lockeable = FreshIOC.Container.Resolve<ILockeable>();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Chat = initData as ChatsDTO;
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            try
            {
                if (Mensajes == null)
                {
                    await CargarMensajes(0, 10);
                }
                else
                {
                    await CargarMensajes(0, 10, true);
                }
            }
            catch (Exception)
            {

            }

            ChatsServices.MensajeRecibido += ChatsServicesOnMensajeRecibido;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            ChatsServices.MensajeRecibido -= ChatsServicesOnMensajeRecibido;
        }

        private async void ChatsServicesOnMensajeRecibido(object sender, ChatMensajeArgs e)
        {
            if (e.ChatRecibido != null && e.ChatRecibido.UltimoMensaje != null && !string.IsNullOrWhiteSpace(e.ChatRecibido.UltimoMensaje.Mensaje))
            {
                bool mensajeYaExiste = false;

                if (Mensajes != null && Mensajes.Count > 0)
                {
                    using (await _lockeable.LockAsync())
                    {
                        mensajeYaExiste = Mensajes.Where(x => x.ConsecutivoMensaje == e.ChatRecibido.UltimoMensaje.Consecutivo && e.ChatRecibido.UltimoMensaje.Consecutivo > 0).Any();
                    }
                }

                // Si el mensaje no existe entonces lo muestro en el chat, mas que todo se usa para evitar que el mensaje que yo envie en este dispositivo lo muestre doble, ya que lo recibo tambien
                // Lo recibo tambien porque si escribo en este dispositivo pero estoy logeado en otro dispositivo el tambien debe recibir el mensaje aunque los dos estemos en la misma cuenta
                if (!mensajeYaExiste)
                {
                    e.ChatRecibido.UltimoMensaje.CodigoChatRecibe = e.ChatRecibido.CodigoChatRecibe;
                    e.ChatRecibido.UltimoMensaje.CodigoChatEnvia = e.ChatRecibido.Consecutivo;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        using (await _lockeable.LockAsync())
                        {
                            if (Mensajes != null)
                            {
                                Mensajes.Insert(0, new ChatMensajesModel(e.ChatRecibido.UltimoMensaje, Chat.CodigoChatRecibe));
                            }
                            else
                            {
                                Mensajes = new ObservableRangeCollection<ChatMensajesModel>(new List<ChatMensajesModel> { new ChatMensajesModel(e.ChatRecibido.UltimoMensaje, Chat.CodigoChatRecibe) });
                            }
                        }
                    });
                }
            }
        }

        public ICommand BorrarChat
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (IsNotConnected)
                        {
                            tcs.SetResult(true);
                            return;
                        }
                        WrapperSimpleTypesDTO wrapper = await _chatsServices.EliminarChat(Chat);

                        if (wrapper != null && wrapper.Exitoso)
                        {
                            await CoreMethods.PopPageModel(Chat);
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorBorrarChat, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand EnviarMensaje
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(TextoParaEnviar) && !IsNotConnected)
                        {
                            ChatsDTO chatMensaje = new ChatsDTO
                            {
                                Consecutivo = Chat.Consecutivo,
                                CodigoPersonaOwner = Chat.CodigoPersonaOwner,
                                CodigoChatRecibe = Chat.CodigoChatRecibe,
                                CodigoPersonaNoOwner = Chat.CodigoPersonaNoOwner,
                                PersonasNoOwner = Chat.PersonasNoOwner,
                                PersonasOwner = new PersonasDTO
                                {
                                    Consecutivo = App.Persona.Consecutivo,
                                    Nombres = App.Persona.Nombres,
                                    Apellidos = App.Persona.Apellidos,
                                    CiudadResidencia = App.Persona.CiudadResidencia,
                                    CodigoPais = App.Persona.CodigoPais,
                                    CodigoIdioma = App.Persona.CodigoIdioma,
                                    CodigoTipoPerfil = App.Persona.CodigoTipoPerfil,
                                    CodigoArchivoImagenBanner = App.Persona.CodigoArchivoImagenBanner,
                                    CodigoArchivoImagenPerfil = App.Persona.CodigoArchivoImagenPerfil,
                                    CodigoUsuario = App.Persona.CodigoUsuario,
                                    Telefono = App.Persona.Telefono,
                                    Skype = App.Persona.Skype
                                },
                                Creacion = Chat.Creacion,
                                UltimoMensaje = new ChatsMensajesDTO
                                {
                                    CodigoChatEnvia = Chat.Consecutivo,
                                    CodigoChatRecibe = Chat.CodigoChatRecibe,
                                    Mensaje = TextoParaEnviar,
                                    FechaMensaje = DateTime.Now,
                                }
                            };

                            ChatMensajesModel mensaje = new ChatMensajesModel(chatMensaje.UltimoMensaje, Chat.CodigoChatRecibe);

                            using (await _lockeable.LockAsync())
                            {
                                if (Mensajes != null)
                                {
                                    Mensajes.Insert(0, mensaje);
                                }
                                else
                                {
                                    Mensajes = new ObservableRangeCollection<ChatMensajesModel>(new List<ChatMensajesModel> { mensaje });
                                }
                            }

                            TextoParaEnviar = string.Empty;

                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapper = await _chatsServices.CrearChatMensaje(chatMensaje.UltimoMensaje);

                            if (wrapper != null && wrapper.Exitoso)
                            {
                                chatMensaje.UltimoMensaje.Consecutivo = Convert.ToInt32(wrapper.ConsecutivoCreado);
                                mensaje.ConsecutivoMensaje = chatMensaje.UltimoMensaje.Consecutivo;
                            }

                            tcs.SetResult(true);

                            await Task.Run(async () =>
                            {
                                for (int i = 1; i < 31; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(App.ConnectionIDChatHub))
                                    {
                                        if (!IsNotConnected)
                                        {
                                            try
                                            {
                                                await _chatsServices.EnviarMensaje(chatMensaje);
                                                break;
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                    }

                                    await Task.Delay(2000);
                                }
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }

                    // Dejarlo siempre en Try, porque antes hay un set result, si no explota
                    tcs.TrySetResult(true);
                });
            }
        }

        public ICommand LoadMoreMensajes
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (Mensajes != null)
                        {
                            await CargarMensajes(Mensajes.Count, 10);
                        }
                        else
                        {
                            await CargarMensajes(0, 10);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task CargarMensajes(int skipIndex, int takeIndex, bool isRefreshing = false)
        {
            if (!NoHayNadaMasParaCargar)
            {
                ChatsMensajesDTO chatMensaje = new ChatsMensajesDTO
                {
                    CodigoChatEnvia = Chat.Consecutivo,
                    CodigoChatRecibe = Chat.CodigoChatRecibe,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (isRefreshing && Mensajes != null)
                {
                    chatMensaje.FechaFiltroBase = Mensajes.Max(x => x.ChatMensaje.FechaMensaje);
                }

                if (IsNotConnected) return;
                List<ChatsMensajesDTO> listaMensajes = await _chatsServices.ListarChatsMensajes(chatMensaje);

                if (listaMensajes != null)
                {
                    if (listaMensajes.Count > 0)
                    {
                        using (await _lockeable.LockAsync())
                        {
                            if (isRefreshing)
                            {
                                if (Mensajes == null)
                                {
                                    Mensajes = new ObservableRangeCollection<ChatMensajesModel>(ChatMensajesModel.CrearListaMensajes(listaMensajes, Chat.CodigoChatRecibe));
                                }
                                else
                                {
                                    List<ChatMensajesModel> listaMensajesParaInsertar = ChatMensajesModel.CrearListaMensajes(listaMensajes, Chat.CodigoChatRecibe);

                                    foreach (ChatMensajesModel mensaje in listaMensajesParaInsertar)
                                    {
                                        Mensajes.Insert(0, mensaje);
                                    }
                                }
                            }
                            else
                            {
                                listaMensajes.Reverse();

                                if (Mensajes == null)
                                {
                                    Mensajes = new ObservableRangeCollection<ChatMensajesModel>(ChatMensajesModel.CrearListaMensajes(listaMensajes, Chat.CodigoChatRecibe));
                                }
                                else
                                {
                                    Mensajes.AddRange(ChatMensajesModel.CrearListaMensajes(listaMensajes, Chat.CodigoChatRecibe));
                                }
                            }
                        }
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaMensajes.Count <= 0;
                    }
                }
            }
        }
    }
}
