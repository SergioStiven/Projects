using Microsoft.AspNet.SignalR.Client;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.Args;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services
{
    public class ChatMensajeArgs : EventArgs
    {
        public ChatsDTO ChatRecibido { get; set; }
        public string MensajeRecibido { get; set; }

        public ChatMensajeArgs(ChatsDTO chatRecibido)
        {
            ChatRecibido = chatRecibido;
            MensajeRecibido = chatRecibido.UltimoMensaje.Mensaje;
        }
    }

    public class TimeLineNotificacionesArgs : EventArgs
    {
        public TimeLineNotificaciones NotificacionRecibida { get; set; }

        public TimeLineNotificacionesArgs(TimeLineNotificaciones timeLineNotificacionRecibido)
        {
            NotificacionRecibida = timeLineNotificacionRecibido;
        }
    }

    public class ChatsServices : BaseService
    {
        public static event EventHandler<TimeLineNotificacionesArgs> NotificacionRecibida;
        public static event EventHandler<ChatMensajeArgs> MensajeRecibido;
        public static event EventHandler ConexionPerdida;
        public static event EventHandler<ExceptionArgs> ErrorConexion;

        Policy _policy = Policy
                              .Handle<WebException>()
                              .Or<HttpRequestException>()
                              .Or<IOException>()
                              .WaitAndRetryAsync(new[] {
                                                             TimeSpan.FromSeconds(1),
                                                             TimeSpan.FromSeconds(1),
                                                             TimeSpan.FromSeconds(1)
                              });

        public static HubConnection _hubConnection;
        public static IHubProxy _hubProxy;

        public static string ConnectionIDChatHub
        {
            get
            {
                if (_hubConnection == null || _hubConnection.State != ConnectionState.Connected)
                {
                    return string.Empty;
                }

                return _hubConnection.ConnectionId;
            }
        }


        #region Metodos SignalR


        // Aqui entran muchos Threads es complicado de sincronizar, asi que se usa una combinacion de Locks y extra checks null para evitar excepciones
        // Explota si no pude tener el lock luego de 5 segundos
        public async Task ConectarChatHub(int consecutivoPersona)
        {
            // Testo si la conexion de verdad necesita una reconexion, si no la necesita entonces salgo
            if (_hubConnection == null || _hubConnection.State != ConnectionState.Connected)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using (await Lockeable.LockAsync(cancellationTokenSource.Token))
                {
                    // Por si acaso dos thread adquirieron el lock casi al mismo tiempo
                    // Testo si ya la conexion es valida, si es valida entonces salgo porque ya un thread anterior la inicializo
                    if (_hubConnection != null && _hubConnection.State == ConnectionState.Connected) return;

                    DisposeChatHub();

                    Dictionary<string, string> diccionarioQueryString = new Dictionary<string, string>
                    {
                        { "myUserId", consecutivoPersona.ToString() },
                    };

                    _hubConnection = new HubConnection(URL.UrlHostSite, diccionarioQueryString, true);

                    _hubConnection.Error += HubConnection_Error;
                    _hubConnection.StateChanged += HubConnection_StateChanged;

                    _hubProxy = _hubConnection.CreateHubProxy("chatHub");

                    _hubProxy.On<ChatsDTO>("receiveMessage", (chatRecibido) =>
                    {
                        if (MensajeRecibido != null)
                        {
                            MensajeRecibido(this, new ChatMensajeArgs(chatRecibido));
                        }
                    });

                    _hubProxy.On<TimeLineNotificaciones>("receiveNotification", (timeLineNotificacionRecibido) =>
                    {
                        if (NotificacionRecibida != null)
                        {
                            NotificacionRecibida(this, new TimeLineNotificacionesArgs(timeLineNotificacionRecibido));
                        }
                    });

                    //Start connection with Polly policy
                    await _policy.ExecuteAsync(async () => await _hubConnection.Start());
                }
            }
        }

        public async Task EnviarMensaje(ChatsDTO chatParaEnviar)
        {
            if (_hubConnection != null && _hubConnection.State == ConnectionState.Connected)
            {
                await _policy.ExecuteAsync(async () => await _hubProxy.Invoke("SendMessageToPerson", chatParaEnviar));
            }
        }

        public async Task EnviarNotificacion(TimeLineNotificaciones notificacionParaEnviar)
        {
            if (_hubConnection != null && _hubConnection.State == ConnectionState.Connected)
            {
                await _policy.ExecuteAsync(async () => await _hubProxy.Invoke("SendNotification", notificacionParaEnviar));
            }
        }

        public void ReproducirNotificacionFake(TimeLineNotificaciones notificacionParaEnviarFake)
        {
            if (NotificacionRecibida != null)
            {
                NotificacionRecibida(this, new TimeLineNotificacionesArgs(notificacionParaEnviarFake));
            }
        }

        public async Task<string> ObtenerIdConexion(int consecutivoPersona)
        {
            if (_hubConnection != null && _hubConnection.State == ConnectionState.Connected)
            {
                return await _policy.ExecuteAsync(async () => await _hubProxy.Invoke<string>("get_connectionid", consecutivoPersona));
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task SuscribeToNotifications(int consecutivoPersona, TipoPerfil tipoPerfil)
        {
            if (_hubConnection != null && _hubConnection.State == ConnectionState.Connected)
            {
                await _policy.ExecuteAsync(async () => await _hubProxy.Invoke<string>("SuscribeToNotifications", consecutivoPersona, (int)tipoPerfil));
            }
        }

        static void HubConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Disconnected)
            {
                ConnectionParada();
            }
        }

        static void HubConnection_Error(Exception obj)
        {
            if (ErrorConexion != null)
            {
                ErrorConexion(null, new ExceptionArgs(obj));
            }
        }

        static void ConnectionParada()
        {
            if (ConexionPerdida != null)
            {
                ConexionPerdida(null, EventArgs.Empty);
            }
        }

        public static void DisposeChatHub()
        {
            if (_hubConnection != null)
            {
                try
                {
                    _hubConnection.Error -= HubConnection_Error;
                    _hubConnection.StateChanged -= HubConnection_StateChanged;

                    _hubConnection?.Stop();
                    _hubConnection?.Dispose();

                    _hubProxy = null;
                    _hubConnection = null;
                }
                catch (Exception)
                {

                }
            }
        }


        #endregion


        #region Metodos Contactos


        public async Task<WrapperSimpleTypesDTO> CrearContacto(ContactosDTO contactoParaCrear)
        {
            if (contactoParaCrear == null) throw new ArgumentNullException("No puedes crear un contacto si contactoParaCrear es nula!.");
            if (contactoParaCrear.CodigoPersonaContacto <= 0 || contactoParaCrear.CodigoPersonaOwner <= 0)
            {
                throw new ArgumentException("contactoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearContacto = await client.PostAsync<ContactosDTO, WrapperSimpleTypesDTO>("Chats/CrearContacto", contactoParaCrear);

            return wrapperCrearContacto;
        }

        public async Task<ContactosDTO> BuscarContacto(ContactosDTO contactoParaBuscar)
        {
            if (contactoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un contacto si contactoParaBuscar es nulo!.");
            if (contactoParaBuscar.Consecutivo <= 0) throw new ArgumentException("contactoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            ContactosDTO contactoBuscado = await client.PostAsync("Chats/BuscarContacto", contactoParaBuscar);

            return contactoBuscado;
        }

        public async Task<ContactosDTO> VerificarSiLaPersonaEstaAgregadaContactos(ContactosDTO contactoParaBuscar)
        {
            if (contactoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un contacto si contactoParaBuscar es nulo!.");
            if (contactoParaBuscar.CodigoPersonaContacto <= 0 || contactoParaBuscar.CodigoPersonaOwner <= 0) throw new ArgumentException("contactoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            ContactosDTO contactoBuscado = await client.PostAsync("Chats/VerificarSiLaPersonaEstaAgregadaContactos", contactoParaBuscar);

            return contactoBuscado;
        }

        public async Task<List<ContactosDTO>> ListarContactos(ContactosDTO contactoParaListar)
        {
            if (contactoParaListar == null) throw new ArgumentNullException("No puedes listar los contactos si contactoParaListar es nulo!.");
            if (contactoParaListar.CodigoPersonaOwner <= 0 || contactoParaListar.SkipIndexBase < 0 || contactoParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("contactoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<ContactosDTO> listaContactos = await client.PostAsync<ContactosDTO, List<ContactosDTO>>("Chats/ListarContactos", contactoParaListar);

            return listaContactos;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarContacto(ContactosDTO contactoParaEliminar)
        {
            if (contactoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un contacto si contactoParaEliminar es nulo!.");
            if (contactoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("contactoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarContacto = await client.PostAsync<ContactosDTO, WrapperSimpleTypesDTO>("Chats/EliminarContacto", contactoParaEliminar);

            return wrapperEliminarContacto;
        }


        #endregion


        #region Metodos Chats


        public async Task<WrapperSimpleTypesDTO> CrearChat(ChatsDTO chatParaCrear)
        {
            if (chatParaCrear == null) throw new ArgumentNullException("No puedes crear un chat si chatParaCrear es nula!.");
            if (chatParaCrear.CodigoPersonaOwner <= 0 || chatParaCrear.CodigoPersonaNoOwner <= 0)
            {
                throw new ArgumentException("chatParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearChat = await client.PostAsync<ChatsDTO, WrapperSimpleTypesDTO>("Chats/CrearChat", chatParaCrear);

            return wrapperCrearChat;
        }

        public async Task<ChatsDTO> BuscarChat(ChatsDTO chatParaBuscar)
        {
            if (chatParaBuscar == null) throw new ArgumentNullException("No puedes buscar un chat si chatParaBuscar es nulo!.");
            if (chatParaBuscar.Consecutivo <= 0) throw new ArgumentException("chatParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            ChatsDTO chatBuscado = await client.PostAsync("Chats/BuscarChat", chatParaBuscar);

            return chatBuscado;
        }

        public async Task<ChatsDTO> BuscarChatEntreDosPersonas(ChatsDTO chatParaBuscar)
        {
            if (chatParaBuscar == null) throw new ArgumentNullException("No puedes buscar un chat si chatParaBuscar es nulo!.");
            if (chatParaBuscar.CodigoPersonaOwner <= 0 || chatParaBuscar.CodigoPersonaNoOwner <= 0)
            {
                throw new ArgumentException("chatParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            ChatsDTO chatBuscado = await client.PostAsync("Chats/BuscarChatEntreDosPersonas", chatParaBuscar);

            return chatBuscado;
        }

        public async Task<List<ChatsDTO>> ListarChats(ChatsDTO chatsParaListar)
        {
            if (chatsParaListar == null) throw new ArgumentNullException("No puedes listar los chats si chatsParaListar es nulo!.");
            if (chatsParaListar.CodigoPersonaOwner <= 0 || chatsParaListar.SkipIndexBase < 0 || chatsParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("chatsParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<ChatsDTO> listaChats = await client.PostAsync<ChatsDTO, List<ChatsDTO>>("Chats/ListarChats", chatsParaListar);

            return listaChats;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarChat(ChatsDTO chatParaEliminar)
        {
            if (chatParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un chat si chatParaEliminar es nulo!.");
            if (chatParaEliminar.Consecutivo <= 0 || chatParaEliminar.CodigoChatRecibe <= 0
                || chatParaEliminar.CodigoPersonaOwner <= 0 || chatParaEliminar.CodigoPersonaNoOwner <= 0)
            {
                throw new ArgumentException("chatParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarChat = await client.PostAsync<ChatsDTO, WrapperSimpleTypesDTO>("Chats/EliminarChat", chatParaEliminar);

            return wrapperEliminarChat;
        }


        #endregion


        #region Metodos ChatsMensajes


        public async Task<WrapperSimpleTypesDTO> CrearChatMensaje(ChatsMensajesDTO chatMensajeParaCrear)
        {
            if (chatMensajeParaCrear == null) throw new ArgumentNullException("No puedes crear un chatMensaje si chatMensajeParaCrear es nula!.");
            if (string.IsNullOrWhiteSpace(chatMensajeParaCrear.Mensaje)
                || chatMensajeParaCrear.CodigoChatEnvia <= 0 || chatMensajeParaCrear.CodigoChatRecibe <= 0)
            {
                throw new ArgumentException("chatMensajeParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearChatMensaje = await client.PostAsync<ChatsMensajesDTO, WrapperSimpleTypesDTO>("Chats/CrearChatMensaje", chatMensajeParaCrear);

            return wrapperCrearChatMensaje;
        }

        public async Task<ChatsMensajesDTO> BuscarChatMensaje(ChatsMensajesDTO chatMensajeParaBuscar)
        {
            if (chatMensajeParaBuscar == null) throw new ArgumentNullException("No puedes buscar un chatMensaje si chatMensajeParaBuscar es nulo!.");
            if (chatMensajeParaBuscar.Consecutivo <= 0) throw new ArgumentException("chatMensajeParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            ChatsMensajesDTO chatMensajeBuscado = await client.PostAsync("Chats/BuscarChatMensaje", chatMensajeParaBuscar);

            return chatMensajeBuscado;
        }

        public async Task<List<ChatsMensajesDTO>> ListarChatsMensajes(ChatsMensajesDTO chatMensajeParaListar)
        {
            if (chatMensajeParaListar == null) throw new ArgumentNullException("No puedes listar los chatsMensaje si chatMensajeParaListar es nulo!.");
            if (chatMensajeParaListar.CodigoChatEnvia <= 0 || chatMensajeParaListar.CodigoChatRecibe <= 0
                || chatMensajeParaListar.SkipIndexBase < 0 || chatMensajeParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("chatMensajeParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<ChatsMensajesDTO> listaChatsMensajes = await client.PostAsync<ChatsMensajesDTO, List<ChatsMensajesDTO>>("Chats/ListarChatsMensajes", chatMensajeParaListar);

            return listaChatsMensajes;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarChatMensaje(ChatsMensajesDTO chatMensajeParaEliminar)
        {
            if (chatMensajeParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un chatMensaje si chatMensajeParaEliminar es nulo!.");
            if (chatMensajeParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("chatMensajeParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarChatMensaje = await client.PostAsync<ChatsMensajesDTO, WrapperSimpleTypesDTO>("Chats/EliminarChatMensaje", chatMensajeParaEliminar);

            return wrapperEliminarChatMensaje;
        }


        #endregion


    }
}
