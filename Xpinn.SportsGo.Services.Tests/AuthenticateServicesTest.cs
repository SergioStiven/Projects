using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;
using System.Collections.Generic;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class AuthenticateServicesTest
    {
        [TestMethod]
        public async Task AuthenticateService_VerificarUsuarios_ShouldPass()
        {
            AuthenticateServices authenticateService = new AuthenticateServices();

            UsuariosDTO usuarioParaVerificar = new UsuariosDTO
            {
                Usuario = "Sergio",
                Clave = "Sergio"
            };

            UsuariosDTO usuarioVerificado = await authenticateService.VerificarUsuario(usuarioParaVerificar);

            Assert.IsNotNull(usuarioVerificado);
            Assert.AreNotEqual(usuarioVerificado.Consecutivo, 0);
        }


        [TestMethod]
        public async Task AuthenticateService_ListarTiposPerfiles_ShouldList()
        {
            AuthenticateServices authenticateService = new AuthenticateServices();

            List<TiposPerfilesDTO> listaTipoPerfiles = await authenticateService.ListarTiposPerfiles();

            Assert.IsNotNull(listaTipoPerfiles);
            Assert.IsTrue(listaTipoPerfiles.Count > 0);
            Assert.IsTrue(listaTipoPerfiles.TrueForAll(x => x.Planes.Count == 0));
            Assert.IsTrue(listaTipoPerfiles.TrueForAll(x => x.Usuarios.Count == 0));
        }
    }
}