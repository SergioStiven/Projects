using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Movil.Models
{
    public class ChatMensajesModel
    {
        public int CodigoChatDestino { get; set; }
        public ChatsMensajesDTO ChatMensaje { get; set; }
        public int ConsecutivoMensaje { get; set; }

        // Si esto es true, significa que el mensaje fue enviado por la persona con la que estoy hablando
        public bool EsElOtroChatConElQueHablo
        {
            get
            {
                return CodigoChatDestino == ChatMensaje.CodigoChatEnvia;
            }
        }

        public ChatMensajesModel(ChatsMensajesDTO chatMensaje, int codigoChatDestino)
        {
            ChatMensaje = chatMensaje;
            CodigoChatDestino = codigoChatDestino;
            ConsecutivoMensaje = chatMensaje.Consecutivo;
        }

        public static List<ChatMensajesModel> CrearListaMensajes(ICollection<ChatsMensajesDTO> chatMensajes, int codigoChatDestino)
        {
            List<ChatMensajesModel> listaMensajes = new List<ChatMensajesModel>();

            if (chatMensajes != null && chatMensajes.Count > 0)
            {
                foreach (var mensaje in chatMensajes)
                {
                    listaMensajes.Add(new ChatMensajesModel(mensaje, codigoChatDestino));
                }
            }

            return listaMensajes;
        }
    }
}
