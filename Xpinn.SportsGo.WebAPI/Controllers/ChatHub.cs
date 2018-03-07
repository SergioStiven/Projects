using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xpinn.SportsGo.Business;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public const string _prefixChatGroupName = "prefixChatGroupName";
        public const string _prefixNotificationsGroupName = "prefixNotificationsGroupName";
        public const string UrlFCM = @"https://fcm.googleapis.com/fcm/send";
        readonly HttpClient _httpClient;

        public ChatHub()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Se necesita el "="
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + AppConstants.ServerAuthKeyFireBase);
        }

        #region Methods

        [HubMethodName("SendMessageToPerson")]
        public async Task SendMessageToPerson(ChatsDTO chatsDestino)
        {
            try
            {
                chatsDestino.UltimoMensaje.FechaMensaje = DateTime.Now;
                chatsDestino.EsNuevoMensaje = true;

                Clients.Group(_prefixChatGroupName + chatsDestino.CodigoPersonaNoOwner.ToString()).receiveMessage(chatsDestino);
                Clients.Group(_prefixChatGroupName + chatsDestino.CodigoPersonaOwner.ToString()).receiveMessage(chatsDestino);

                FireBasePayLoad fireBasePayLoad = new FireBasePayLoad
                {
                    priority = "high",
                    to = "/topics/" + chatsDestino.CodigoPersonaNoOwner.ToString(),
                    notification = new NotificationFireBaseModel
                    {
                        title = chatsDestino.PersonasOwner.NombreYApellido,
                        body = chatsDestino.UltimoMensaje.Mensaje,
                        sound = "default",
                        content_available = "true"
                    }
                };

                await _httpClient.PostAsJsonAsync(UrlFCM, fireBasePayLoad);
            }
            catch (Exception)
            {
                // The error may be because the target person is not connected
            }
        }

        [HubMethodName("SendNotification")]
        public void SendNotification(TimeLineNotificaciones timeLineNotificaciones)
        {
            try
            {
                Clients.Group(_prefixChatGroupName + timeLineNotificaciones.CodigoPersonaDestino.ToString()).receiveNotification(timeLineNotificaciones);
            }
            catch (Exception)
            {
                // The error may be because the target person is not connected
            }
        }

        [HubMethodName("get_connectionid")]
        public string GetConnectioId(string userId)
        {
            var connectionId = _connections.GetConnections(userId);
            return connectionId.Count() > 0 ? connectionId.First().ToString() : string.Empty;
        }

        [HubMethodName("SuscribeToNotifications")]
        public async Task SuscribeToNotifications(string userId, string tipoPerfil)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId no puede estar vacio!,");
            if (string.IsNullOrWhiteSpace(tipoPerfil)) throw new ArgumentNullException("tipoPerfil no puede estar vacio!,");

            TipoPerfil tipoPerfilEnum = tipoPerfil.ToEnum<TipoPerfil>();
            CategoriasBusiness categoriaBusiness = new CategoriasBusiness();
            List<int> listaCategorias = null;

            // Se consultan las categorias de la persona conectandose
            switch (tipoPerfilEnum)
            {
                case TipoPerfil.Candidato:
                    listaCategorias = await categoriaBusiness.ListarCodigoCategoriasDeUnCandidato(Convert.ToInt32(userId));
                    break;
                case TipoPerfil.Grupo:
                    listaCategorias = await categoriaBusiness.ListarCodigoCategoriasDeUnGrupo(Convert.ToInt32(userId));
                    break;
                case TipoPerfil.Representante:
                    listaCategorias = await categoriaBusiness.ListarCodigoCategoriasDeUnRepresentante(Convert.ToInt32(userId));
                    break;
            }

            // Se valida que no halla error al consultar
            if (listaCategorias == null) throw new InvalidOperationException("Fallo al consultar las categorias!.");

            // Verifico que tengo categorias a las que me suscribire por cambios
            if (listaCategorias.Count > 0)
            {
                // Por cada categoria verifico que ya no este suscrito, si lo estoy la remuevo (Evito escuchar doble) 
                // Y procedo a suscribirme
                foreach (var categorias in listaCategorias)
                {
                    await Groups.Remove(Context.ConnectionId, _prefixNotificationsGroupName + categorias);
                    await Groups.Add(Context.ConnectionId, _prefixNotificationsGroupName + categorias);
                }
            }
        }

        #endregion

        #region Lifecycle

        public override async Task OnConnected()
        {
            // Codigo de persona del que se quiere conectar, usado como key para la conexion
            string userId = Context.QueryString["myUserId"];

            // Validaciones de parametros
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId no puede estar vacio!,");

            // Se busca a ver si esta conexion ya existe
            var currentConection = _connections.GetConnections(userId);

            // Si existe se remueve (Evita escuchar doble)
            if (currentConection.Count() > 0)
            {
                _connections.Remove(userId, currentConection.First());
            }

            // Se añade la conexion
            _connections.Add(userId, Context.ConnectionId);

            // Se verifica que no exista el grupo unico de la persona, si existe se borra (Evito escuchar doble) 
            await Groups.Remove(Context.ConnectionId, _prefixChatGroupName + userId);

            // Se crea un grupo unico que solo va a estar la persona, de esta manera si la misma persona se conecta en diferentes dispositivos
            // El mensaje se transmitira a todos los dispositivos al que este conectado
            await Groups.Add(Context.ConnectionId, _prefixChatGroupName + userId);

            await base.OnConnected();
        }

        // You should not manually remove the user from the group when the user disconnects.This action is automatically performed by the SignalR framework.
        // https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections
        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = Context.QueryString["myUserId"];

            _connections.Remove(userId, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            //if (!_connections.GetConnections(Context.ConnectionId).Contains(Context.ConnectionId))
            //{
            //    _connections.Add(Context.ConnectionId, Context.ConnectionId);
            //}

            return base.OnReconnected();
        }

        #endregion

        class FireBasePayLoad
        {
            public NotificationFireBaseModel notification { get; set; }
            public string priority { get; set; }
            public string to { get; set; }
        }

        class NotificationFireBaseModel
        {
            public string body { get; set; }
            public string title { get; set; }
            public string sound { get; set; }
            public string content_available { get; set; }
        }
    }
}