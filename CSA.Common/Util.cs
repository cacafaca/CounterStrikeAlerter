using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
