using System;
using System.Security.Cryptography;
using System.Text;

namespace Xpinn.SportsGo.Util
{
    public class SymmetricCryptoService<TCrypt, THasher>
        where TCrypt : SymmetricAlgorithm, new()
        where THasher : HashAlgorithm, new()
    {
        //@"Fs2y9EM9vYpJwD8DpgNbu+3KfOQjelaXowfTNsDYLgU="
        readonly string CryptoKey = @"Fs2y9EM9vYpJwD8DpgNbu+3KfOQjelaXowfTNsDYLgU=";

        // @"XezL35y3vQrxUkqXf4SBPQ=="
        readonly string CryptoIVKey = @"XezL35y3vQrxUkqXf4SBPQ==";

        public SymmetricCryptoService()
        {
        }

        public string Encrypt(string toEncrypt, bool useHashing = true)
        {
            if (string.IsNullOrWhiteSpace(toEncrypt)) throw new ArgumentNullException("Mensaje a encriptar no puede estar vacio!.");

            byte[] cryptoKeyArray;
            byte[] cryptoIVKeyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            BuildKeysAndIV(out cryptoKeyArray, out cryptoIVKeyArray, useHashing);

            using (TCrypt cryptoSimmetricProvider = new TCrypt())
            {
                // Must resize IV because the lenght is 2x than supported 
                Array.Resize(ref cryptoKeyArray, cryptoSimmetricProvider.Key.Length);
                Array.Resize(ref cryptoIVKeyArray, cryptoSimmetricProvider.IV.Length);

                cryptoSimmetricProvider.Key = cryptoKeyArray;
                cryptoSimmetricProvider.IV = cryptoIVKeyArray;
                cryptoSimmetricProvider.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)
                cryptoSimmetricProvider.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform cTransform = cryptoSimmetricProvider.CreateEncryptor())
                {
                    //transform the specified region of bytes array to resultArray
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    //Release resources held by AesCng Encryptor
                    cryptoSimmetricProvider.Clear();

                    //Return the encrypted data into unreadable string format
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
            }
        }

        public string Decrypt(string cipherString, bool useHashing = true)
        {
            if (string.IsNullOrWhiteSpace(cipherString)) throw new ArgumentNullException("Mensaje a desencriptar no puede ser nulo!.");

            byte[] cryptoKeyArray;
            byte[] cryptoIVKeyArray;
            byte[] toDecryptArray = Convert.FromBase64String(cipherString);

            BuildKeysAndIV(out cryptoKeyArray, out cryptoIVKeyArray, useHashing);

            using (TCrypt cryptoSymmetricProvider = new TCrypt())
            {
                // Must resize IV because the lenght is 2x than supported 
                Array.Resize(ref cryptoKeyArray, cryptoSymmetricProvider.Key.Length);
                Array.Resize(ref cryptoIVKeyArray, cryptoSymmetricProvider.IV.Length);

                cryptoSymmetricProvider.Key = cryptoKeyArray;
                cryptoSymmetricProvider.IV = cryptoIVKeyArray;
                //We choose ECB(Electronic code Book)

                cryptoSymmetricProvider.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)
                cryptoSymmetricProvider.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform cTransform = cryptoSymmetricProvider.CreateDecryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);

                    //Release resources held by AesCng Encryptor                
                    cryptoSymmetricProvider.Clear();

                    //return the Clear decrypted TEXT
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
        }

        void BuildKeysAndIV(out byte[] cryptoKeyArray, out byte[] cryptoIVKeyArray, bool useHashing = true)
        {
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                using (THasher hasherAlghorithm = new THasher())
                {
                    cryptoKeyArray = hasherAlghorithm.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                    cryptoIVKeyArray = hasherAlghorithm.ComputeHash(Encoding.UTF8.GetBytes(CryptoIVKey));

                    //Always release the resources and flush data
                    // of the Cryptographic service provide. Best Practice
                    hasherAlghorithm.Clear();
                }
            }
            else
            {
                cryptoKeyArray = Encoding.UTF8.GetBytes(CryptoKey);
                cryptoIVKeyArray = Encoding.UTF8.GetBytes(CryptoIVKey);
            }
        }
    }
}
