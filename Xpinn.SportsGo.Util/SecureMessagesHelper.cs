using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Util
{
    public class SecureMessagesHelper : ISecureableMessage
    {
        readonly SymmetricCryptoService<AesManaged, SHA256Managed> _symmetricCryptoService;

        public SecureMessagesHelper()
        {
            _symmetricCryptoService = new SymmetricCryptoService<AesManaged, SHA256Managed>();
        }

        public async Task<string> EncryptEntity<TEntity>(TEntity entidad, bool useHashing = true) where TEntity : class
        {
            if (entidad == null) throw new ArgumentNullException("Entidad a encriptar no puede estar nula!.");

            string jsonEntity = await Task.Run(() => JsonConvert.SerializeObject(entidad, JsonSerializerOptionsConfiguration.ReturnJsonSerializerSettings()));

            return EncryptJson(jsonEntity, useHashing);
        }

        public string EncryptJson(string jsonEntity, bool useHashing = true) 
        {
            if (string.IsNullOrWhiteSpace(jsonEntity)) throw new ArgumentNullException("Json a encriptar no puede estar vació!.");

            return _symmetricCryptoService.Encrypt(jsonEntity, useHashing);
        }

        public async Task<TEntity> DecryptMessageToEntity<TEntity>(string messageToDecrypt, bool useHashing = true) where TEntity : class
        {
            if (string.IsNullOrWhiteSpace(messageToDecrypt)) throw new ArgumentNullException("Mensaje a desencriptar no puede estar vació!.");

            string messageDecrypted = DecryptJson(messageToDecrypt, useHashing);

            return await Task.Run(() => JsonConvert.DeserializeObject<TEntity>(messageDecrypted));
        }

        public string DecryptJson(string jsonEntity, bool useHashing = true)
        {
            if (string.IsNullOrWhiteSpace(jsonEntity)) throw new ArgumentNullException("Json a encriptar no puede estar vació!.");

            return _symmetricCryptoService.Decrypt(jsonEntity, useHashing);
        }
    }
}
