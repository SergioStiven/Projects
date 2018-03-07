using System;
using System.Security.Cryptography;
using System.Text;

namespace Xpinn.SportsGo.Util
{
    public class HasherCryptoService<THasher> where THasher : HashAlgorithm, new()
    {
        public string GetStringHash(string stringToHashing)
        {
            if (string.IsNullOrEmpty(stringToHashing))
            {
                throw new ArgumentNullException("stringToHashing");
            }

            using (THasher hasherAlghorithm = new THasher())
            {
                byte[] data = hasherAlghorithm.ComputeHash(Encoding.Default.GetBytes(stringToHashing));

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
