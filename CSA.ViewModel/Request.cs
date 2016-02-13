using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class Request
    {
        /**
        https://developer.valvesoftware.com/wiki/Server_queries
        */

        const int SimpleResponseFormat = -1; // Precedes every server query.
        const string A2S_INFO = "TSource Engine Query\x00"; // Basic information about the server. 
        readonly byte[] A2S_PLAYER = { 0x55, 0xFF, 0xFF, 0xFF, 0xFF };


        public byte[] ServerInfo
        {
            get { return GetServerInfoRequest(); }
        }

        private byte[] GetServerInfoRequest()
        {
            return JoinArrays(BitConverter.GetBytes(SimpleResponseFormat), Encoding.Default.GetBytes(A2S_INFO));
        }

        public byte[] PlayerInfoChallenge
        {
            get { return GetPlayerInfoChellengeRequest(); }
        }

        private byte[] GetPlayerInfoChellengeRequest()
        {
            return JoinArrays(BitConverter.GetBytes(SimpleResponseFormat), A2S_PLAYER);
        }

        private byte[] JoinArrays(byte[] array1, byte[] array2)
        {
            byte[] result = new byte[array1.Length + array2.Length];
            Array.Copy(array1, result, array1.Length);
            Array.Copy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }

        public byte[] GetPlayersRequest(byte[] challenge)
        {
            if (challenge != null)
            {
                return JoinArrays(JoinArrays(BitConverter.GetBytes(SimpleResponseFormat), new byte[1] { A2S_PLAYER[0] }),
                    challenge);
            }
            else
                return null;
        }
    }
}
