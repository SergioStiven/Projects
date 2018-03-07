using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Tests
{
    [TestClass]
    public class SecureMessageHelperTest
    {
        SecureMessagesHelper _secureHelper;
        UsuariosDTO _usuarioEntity;
        string _usuario;
        string _clave;

        public SecureMessageHelperTest()
        {
            _secureHelper = new SecureMessagesHelper();
            _usuario = "Nombre";
            _clave = "Clave";

            _usuarioEntity = new UsuariosDTO
            {
                Usuario = _usuario,
                Clave = _clave
            };
        }
        [TestMethod]
        public async Task EntityEncrypted()
        {
            var encryptedEntity = await _secureHelper.EncryptEntity(_usuarioEntity);

            UsuariosDTO decryptedEntity = await _secureHelper.DecryptMessageToEntity<UsuariosDTO>(encryptedEntity);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(encryptedEntity));
            Assert.IsNotNull(decryptedEntity);
            Assert.AreEqual(decryptedEntity.Usuario, _usuario);
            Assert.AreEqual(decryptedEntity.Clave, _clave);
        }

        [TestMethod]
        public async Task DecryptMessage()
        {
            string decryptedEntity = await _secureHelper.DecryptMessageToEntity<string>("XMVx7pL7BchyP06dbGfu91E41P/T78zl9i+na7bfldNNzRzmyC8HHWrBZY0mQ0iAMBhSy4RSGm9pwsBViozN4P+uZ3G4TGqTr5vIp0obwpE=");

            Assert.IsNotNull(decryptedEntity);
        }
    }
}
