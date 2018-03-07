using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.DomainEntities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class ChatsBusinessTest
    {
        [TestMethod]
        public async Task ChatBusiness_CrearChat_ShouldCreate()
        {
            ChatsBusiness chatBusiness = new ChatsBusiness();

            Chats chatParaCrear = new Chats
            {
                CodigoPersonaOwner = 3,
                CodigoPersonaNoOwner = 31
            };

            WrapperSimpleTypesDTO wrapper = await chatBusiness.CrearChat(chatParaCrear);

            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task ChatBusiness_CrearContacto_ShouldCreate()
        {
            ChatsBusiness chatBusiness = new ChatsBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            Contactos contactoParaCrear = await secure.DecryptMessageToEntity<Contactos>(@"myavxScWEWalZPmYHfCSEb1s0RemPqBgUeQL0jQnVdOX7+lOcyU1ZnnuY1svL6dCx5UDnLp7IhHH08lYhsAC7kS6af5RQ5VHTYEOSGaID7Rc3LbKHeKi15nGxQNoXJhyxLsF8WyrPKE1x0Hzzr3Au8p6Rjx/c90ip3M+k4l+ooKzo6O1yq+FX/2Ri+X8rCuHxoqnMfxW3p9rTCAln+dXlL3oCaSPfrVyp9wnanEXp/J//PcN2V/q7LexmmEDac6bIhU+K4eaQc5y2p6MRpCdRTWqeik3n6xVDayImhLvnh/VYBTayCz1Svd80CIgC0tT");

            Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones> wrapper = await chatBusiness.CrearContacto(contactoParaCrear);

            Assert.IsNotNull(wrapper.Item1);
        }

        [TestMethod]
        public async Task ChatBusiness_EliminarContacto_ShouldDelete()
        {
            ChatsBusiness chatBusiness = new ChatsBusiness();

            Contactos contactoParaEliminar = new Contactos
            {
                Consecutivo = 2105
            };

            Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones> wrapper = await chatBusiness.EliminarContacto(contactoParaEliminar);

            Assert.IsNotNull(wrapper.Item1);
        }


        [TestMethod]
        public async Task ChatBusiness_ListarChats_ShouldList()
        {
            ChatsBusiness chatBusiness = new ChatsBusiness();

            Chats chats = new Chats
            {
                SkipIndexBase = 0,
                TakeIndexBase = 5,
                CodigoPersonaOwner = 3
            };

            List<ChatsDTO> listaChats = await chatBusiness.ListarChats(chats);

            Assert.IsNotNull(listaChats);
        }

        [TestMethod]
        public async Task ChatBusiness_ListarChatsMensajes_ShouldList()
        {
            ChatsBusiness chatBusiness = new ChatsBusiness();

            ChatsMensajes chats = new ChatsMensajes
            {
                SkipIndexBase = 0,
                TakeIndexBase = 4,
                CodigoChatEnvia = 1,
                CodigoChatRecibe = 2,
                ZonaHorariaGMTBase = -5
            };

            List<ChatsMensajes> listaChats = await chatBusiness.ListarChatsMensajes(chats);

            Assert.IsNotNull(listaChats);
        }
    }
}