using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Common
{
    public static class Util
    {
        public static string ByteArrayToString(byte[] ba)
        {
            if (ba != null)
            {
                StringBuilder hex = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                    hex.AppendFormat("\\x{0:X2} ", b);
                return hex.ToString().TrimEnd();
            }
            else
                return string.Empty;
        }

        public static string Encrypt(string value)
        {
            return value.Encrypt();
        }

        public static string Decrypt(string value)
        {
            return value.Decrypt();
        }

        public static string GetAppPath()
        {
            //return System.IO.Path.GetDirectoryName(
            //    AppDomain.CurrentDomain.BaseDirectory);
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
