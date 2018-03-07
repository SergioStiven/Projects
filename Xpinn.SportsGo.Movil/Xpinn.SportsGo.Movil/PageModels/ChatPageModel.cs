using FreshMvvm;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ChatPageModel : BasePageModel
    {
        ChatsServices _chatsServices;
        ILockeable _lockeable;
        IAudioManager _audioManager;
        IDateTimeHelper _dateTimeHelper;

        ObservableRangeCollection<ChatsDTO> _chats;
        public ObservableRangeCollection<ChatsDTO> Chats
        {
            get
            {
                IEnumerable<ChatsDTO> listaChat = _chats;

                if (listaChat != null && listaChat.Count() > 0)
                {
                    listaChat = listaChat.OrderByDescending(x => x.EsNuevoMensaje).ThenByDescending(x => x.UltimoMensaje.FechaMensaje);
                }
                else
                {
                    return new ObservableRangeCollection<ChatsDTO>();
                }

                return new ObservableRangeCollection<ChatsDTO>(listaChat);
            }
            set
            {
                _chats = value;
            }
        }

        public bool NoHayNadaMasParaCargar { get; set; }
        public bool IsRefreshing { get; set; }
        public DateTime LastRefresh { get; set; }
        public bool NecesitaRefrescar { get; set; }
        public int? CodigoChatConElQueConverso { get; set; }

        public ChatPageModel()
        {
            _chatsServices = new ChatsServices();
            _lockeable = FreshIOC.Container.Resolve<ILockeable>();
            _audioManager = FreshIOC.Container.Resolve<IAudioManager>();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                if (_chats != null && _chats.Count > 0)
                {
                    await CargarConversaciones(0, 10, true);
                }
                else
                {
                    await CargarConversaciones(0, 10);
                }

                LastRefresh = DateTime.Now;
            }
            catch (Exception)
            {

            }

            ChatsServices.MensajeRecibido += async (obj, evt) =>
            {
                if (evt.ChatRecibido != null && evt.ChatRecibido.UltimoMensaje != null && !string.IsNullOrWhiteSpace(evt.ChatRecibido.UltimoMensaje.Mensaje))
                {
                    LastRefresh = DateTime.Now;
                    int numeroMensajesNuevos = 0;

                    using (await _lockeable.LockAsync())
                    {
                        if (_chats != null && _chats.Count > 0)
                        {
                            // Elimino el chat si ya existe para que no salga repetido
                            ChatsDTO chatExistente = _chats.Where(x => (x.CodigoPersonaOwner == evt.ChatRecibido.CodigoPersonaOwner && x.CodigoPersonaNoOwner == evt.ChatRecibido.CodigoPersonaNoOwner) || (x.CodigoPersonaOwner == evt.ChatRecibido.CodigoPersonaNoOwner && x.CodigoPersonaNoOwner == evt.ChatRecibido.CodigoPersonaOwner)).FirstOrDefault();

                            if (chatExistente != null)
                            {
                                _chats.Remove(chatExistente);
                                numeroMensajesNuevos = chatExistente.NumeroMensajesNuevos;
                            }
                        }

                        ChatsDTO chatRecibido = new ChatsDTO
                        {
                            Consecutivo = evt.ChatRecibido.Consecutivo,
                            ChatsEstado = evt.ChatRecibido.ChatsEstado,
                            ChatsMensajesEnvia = evt.ChatRecibido.ChatsMensajesEnvia,
                            ChatsMensajesRecibe = evt.ChatRecibido.ChatsMensajesRecibe,
                            CodigoChatRecibe = evt.ChatRecibido.CodigoChatRecibe,
                            CodigoEstado = evt.ChatRecibido.CodigoEstado,
                            CodigoPersonaNoOwner = evt.ChatRecibido.CodigoPersonaNoOwner,
                            CodigoPersonaOwner = evt.ChatRecibido.CodigoPersonaOwner,
                            Creacion = evt.ChatRecibido.Creacion,
                            EstadoChat = evt.ChatRecibido.EstadoChat,
                            PersonasNoOwner = evt.ChatRecibido.PersonasNoOwner,
                            PersonasOwner = evt.ChatRecibido.PersonasOwner,
                            UltimoMensaje = evt.ChatRecibido.UltimoMensaje
                        };

                        chatRecibido.UltimoMensaje.MensajeParaMostrar = chatRecibido.PersonasOwner.Nombres + ": " + chatRecibido.UltimoMensaje.Mensaje;

                        // Si este CodigoPersonaOwner es igual al consecutivo de App.Persona significa que ese mensaje es generado por mi
                        bool esMensajeGeneradoPorMi = chatRecibido.CodigoPersonaOwner == App.Persona.Consecutivo;

                        // Si recibo un mensaje de la persona con la que estoy hablando
                        bool esMensajeConElQueConverso = CodigoChatConElQueConverso.HasValue && CodigoChatConElQueConverso.Value == chatRecibido.Consecutivo;

                        // Si el mensaje no es generado por mi no lo marco como nuevo
                        chatRecibido.EsNuevoMensaje = !esMensajeGeneradoPorMi && !esMensajeConElQueConverso;
                        if (esMensajeGeneradoPorMi)
                        {
                            chatRecibido.UltimoMensaje.UltimoMensajeEsMio = true;
                        }
                        else
                        {
                            // Aqui entra cuando el mensaje no es mio, entonces hay que intercambiar las entidades
                            // Para que el chat sea tratado como es y se vea los mensajes con el nombre de quien hablo y eso

                            var personaNoOwnerTemporal = chatRecibido.PersonasNoOwner;
                            chatRecibido.PersonasNoOwner = chatRecibido.PersonasOwner;
                            chatRecibido.PersonasOwner = personaNoOwnerTemporal;

                            var codigoPersonaNoOwnerTemporal = chatRecibido.CodigoPersonaNoOwner;
                            chatRecibido.CodigoPersonaNoOwner = chatRecibido.CodigoPersonaOwner;
                            chatRecibido.CodigoPersonaOwner = codigoPersonaNoOwnerTemporal;

                            var codigoChatRecibidoTemporal = chatRecibido.CodigoChatRecibe;
                            chatRecibido.CodigoChatRecibe = chatRecibido.Consecutivo;
                            chatRecibido.Consecutivo = codigoChatRecibidoTemporal;
                        }

                        chatRecibido.NumeroMensajesNuevos = numeroMensajesNuevos + 1;
                        if (_chats == null)
                        {
                            _chats = new ObservableRangeCollection<ChatsDTO>(new List<ChatsDTO> { chatRecibido });
                        }
                        else
                        {
                            _chats.Add(chatRecibido);
                        }

                        Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(Chats)));

                        // Si el mensaje no es generado por mi (sea porque viene de otro cel o de la web)
                        // Si no estoy en la pantalla de conversacion
                        // Si el mensaje no es de la persona con la que estoy hablando
                        // Entonces emito sonido y subo notificacion
                        bool emiteSonido = !esMensajeGeneradoPorMi && (!App.EstoyEnPantallaConversacion || !esMensajeConElQueConverso);

                        // Si el mensaje es mio y estoy en pantalla de chat escribiendo no suena
                        if (emiteSonido)
                        {
                            App.InteractuarValorBadgeChat(1);

                            if (!App.AppIsInBackGround)
                            {
                                //Set or Get the state of the Effect sounds.
                                _audioManager.EffectsOn = true;

                                //Set the volume level of the Effects from 0 to 1.
                                _audioManager.EffectsVolume = esMensajeConElQueConverso ? 0.2f : 0.4f;

                                try
                                {
                                    //await _audioManager.PlaySound("notificationSound.mp3");
                                    await _audioManager.PlayNotificationDefaultSound();
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
            };

            PerfilPageModel.OnPersonaBorrada += async (obj, evt) =>
            {
                if (_chats != null && _chats.Count > 0)
                {
                    using (await _lockeable.LockAsync())
                    {
                        ChatsDTO conversacionParaBorrar = _chats.Where(x => x.CodigoPersonaNoOwner == evt.CodigoPersonaBorrada).FirstOrDefault();

                        if (conversacionParaBorrar != null)
                        {
                            _chats.Remove(conversacionParaBorrar);

                            Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(Chats)));
                        }
                    }
                }
            };

            ConnectionChanged += async (sender, args) =>
            {
                if (args.IsConnect && NecesitaRefrescar)
                {
                    try
                    {
                        await CargarConversaciones(0, 100, true, true);
                        NecesitaRefrescar = false;
                    }
                    catch (Exception)
                    {

                    }
                }

                if (!args.IsConnect)
                {
                    NecesitaRefrescar = true;
                }
            };
        }

        public override async void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            ChatsDTO chatBorrado = returnedData as ChatsDTO;

            if (chatBorrado != null && _chats != null && _chats.Count > 0)
            {
                using (await _lockeable.LockAsync())
                {
                    _chats.Remove(chatBorrado);
                    RaisePropertyChanged(nameof(Chats));
                }
            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            NoHayNadaMasParaCargar = false;
            CodigoChatConElQueConverso = null;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            NoHayNadaMasParaCargar = true;
        }

        public ICommand IrConversacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (App.Usuario.PlanesUsuarios.Planes.ServiciosChat != 1)
                    {
                        await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.ServiciosChat));
                    }
                    else
                    {
                        ChatsDTO chatSeleccionado = parameter as ChatsDTO;

                        bool esNuevoMensaje = chatSeleccionado.EsNuevoMensaje;
                        chatSeleccionado.EsNuevoMensaje = false;

                        await CoreMethods.PushPageModel<ConversacionChatPageModel>(parameter);

                        if (esNuevoMensaje)
                        {
                            Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(Chats)));

                            // Restamos la cantidad de mensajes nuevos para este chat
                            App.InteractuarValorBadgeChat(chatSeleccionado.NumeroMensajesNuevos * -1);
                            chatSeleccionado.NumeroMensajesNuevos = 0;
                        }

                        CodigoChatConElQueConverso = chatSeleccionado.Consecutivo;
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand NuevoChat
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (App.Usuario.PlanesUsuarios.Planes.ServiciosChat != 1)
                    {
                        await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.ServiciosChat));
                    }
                    else
                    {
                        await CoreMethods.PushPageModel<ContactosChatPageModel>();
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand BorrarChat
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    ChatsDTO chatParaBorrar = parameter as ChatsDTO;

                    if (chatParaBorrar != null && _chats != null && _chats.Count > 0)
                    {
                        using (await _lockeable.LockAsync())
                        {
                            _chats.Remove(chatParaBorrar);
                        }

                        RaisePropertyChanged("Chats");

                        try
                        {
                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            await _chatsServices.EliminarChat(chatParaBorrar);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        IsRefreshing = true;
                        await CargarConversaciones(0, 100, true);
                    }
                    catch (Exception)
                    {

                    }

                    IsRefreshing = false;
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand LoadMoreChats
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (_chats != null)
                        {
                            await CargarConversaciones(_chats.Count, 10);
                        }
                        else
                        {
                            await CargarConversaciones(0, 10);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task CargarConversaciones(int skipIndex, int takeIndex, bool isRefresh = false, bool aumentarBadge = false)
        {
            if (!NoHayNadaMasParaCargar || isRefresh)
            {
                ChatsDTO buscador = new ChatsDTO
                {
                    CodigoPersonaOwner = App.Persona.Consecutivo,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (isRefresh && LastRefresh != DateTime.MinValue)
                {
                    buscador.FechaFiltroBase = LastRefresh;
                    LastRefresh = DateTime.Now;
                }

                if (IsNotConnected) return;
                List<ChatsDTO> listaChats = await _chatsServices.ListarChats(buscador);

                if (listaChats != null)
                {
                    if (listaChats.Count > 0)
                    {
                        if (isRefresh && buscador.FechaFiltroBase != DateTime.MinValue)
                        {
                            foreach (ChatsDTO chat in listaChats)
                            {
                                if (chat.UltimoMensaje != null && chat.UltimoMensaje.FechaMensaje > buscador.FechaFiltroBase && !chat.UltimoMensaje.UltimoMensajeEsMio)
                                {
                                    chat.EsNuevoMensaje = true;
                                }
                            }
                        }

                        using (await _lockeable.LockAsync())
                        {
                            if (_chats == null)
                            {
                                _chats = new ObservableRangeCollection<ChatsDTO>(listaChats);
                            }
                            else
                            {
                                foreach (ChatsDTO chat in listaChats)
                                {
                                    ChatsDTO chatExistente = _chats.Where(x => x.Consecutivo == chat.Consecutivo).FirstOrDefault();

                                    if (chatExistente != null)
                                    {
                                        _chats.Remove(chatExistente);
                                    }

                                    _chats.Add(chat);
                                }
                            }

                            if (aumentarBadge)
                            {
                                App.InteractuarValorBadgeNotificacion(listaChats.Count);
                            }
                        }

                        RaisePropertyChanged("Chats");
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaChats.Count <= 0 && !isRefresh;
                    }
                }
            }
        }
    }
}
