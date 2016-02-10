using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Model
{
    public class GoldSourceServer : BaseServer
    {
        public GoldSourceServer(string address, int port)
            : base(address, port)
        {
        }

        private string _InternalAddress;
        /// <summary>
        /// IP address and port of the server. 
        /// </summary>
        public string InternalAddress
        {
            get { return _InternalAddress; }
            set { _InternalAddress = value; }
        }

        private GoldSourceMod _Mod;
        /// <summary>
        ///  	Indicates whether the game is a mod:
        ///         0 for Half-Life
        ///         1 for Half-Life mod
        /// </summary>
        public GoldSourceMod Mod
        {
            get { return _Mod; }
            set { _Mod = value; }
        }


        public override string ToString()
        {
            return string.Format("Server name:{0}; Map:{1}; Players/Max:{2}/{3}", Name, Map, ActualPlayers, MaxPlayers);
        }
    }
}
