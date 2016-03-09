using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CSA.Common
{
    public static class DataProtectionApiWrapper
    {
        /// <summary>
        /// Specifies the data protection scope of the DPAPI.
        /// </summary>
        private const DataProtectionScope Scope = DataProtectionScope.LocalMachine;

        public static string Encrypt(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            //encrypt data
            var data = Encoding.Default.GetBytes(text);
            byte[] encrypted = ProtectedData.Protect(data, null, Scope);

            //return as base64 string
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(this string cipher)
        {
            if (cipher == null)
            {
                throw new ArgumentNullException("cipher");
            }

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, Scope);
            return Encoding.Default.GetString(decrypted);
        }

    }
}
