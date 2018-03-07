using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.DomainEntities;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class AdministracionBusinessTest
    {
        [TestMethod]
        public async Task AdministracionBusiness_AsignarImagenPerfilAdministrador_ShouldAssign()
        {
            AdministracionBusiness adminBuss = new AdministracionBusiness();

            using (Stream video = File.Open(@"C:\Users\Administrador.EXPINN-TEC-5\Documents\Visual Studio 2017\images.jpg", FileMode.Open, FileAccess.Read))
            {
                WrapperSimpleTypesDTO wrapper = await adminBuss.AsignarImagenPerfilAdministrador(1, video);
            }
        }

        [TestMethod]
        public async Task AdministracionBusiness_EliminarUsuario_ShouldDelete()
        {
            AdministracionBusiness adminBuss = new AdministracionBusiness();

            Usuarios usuario = new Usuarios
            {
                Consecutivo = 2166
            };

            WrapperSimpleTypesDTO wrapperEliminarUsuario = await adminBuss.EliminarUsuario(usuario);

            Assert.IsNotNull(wrapperEliminarUsuario);
        }
    }
}
