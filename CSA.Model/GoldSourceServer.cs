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

        private string _Link;
        /// <summary>
        /// URL to mod website.
        /// </summary>
        public string Link
        {
            get { return _Link; }
            set { _Link = value; }
        }

        private string _DownloadLink;
        /// <summary>
        /// URL to download the mod.
        /// </summary>
        public string DownloadLink
        {
            get { return _DownloadLink; }
            set { _DownloadLink = value; }
        }

        private int _Version;
        /// <summary>
        /// Version of mod installed on server.
        /// </summary>
        public int Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private int _Size;
        /// <summary>
        /// Space(in bytes) the mod takes up.
        /// </summary>
        public int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        private GoldSourceModType _Type;
        /// <summary>
        /// Indicates the type of mod:
        ///     0 for single and multiplayer mod
        ///     1 for multiplayer only mod
        /// </summary>
        public GoldSourceModType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private GoldSourceModDll _Dll;
        /// <summary>
        /// Indicates whether mod uses its own DLL:
        ///     0 if it uses the Half-Life DLL
        ///     1 if it uses its own DLL
        /// </summary>
        public GoldSourceModDll Dll
        {
            get { return _Dll; }
            set { _Dll = value; }
        }

        public override string ToString()
        {
            return string.Format("Server name:{0}; Map:{1}; Players/Max:{2}/{3}", Name, Map, ActualPlayers, MaxPlayers);
        }

        public override BaseServer Copy()
        {
            GoldSourceServer newInstance = new GoldSourceServer(Address, Port);
            newInstance.InternalAddress = InternalAddress;
            newInstance.Name = Name;
            newInstance.Map = Map;
            newInstance.Folder = Folder;
            newInstance.Game = Game;
            newInstance.ActualPlayers = ActualPlayers;
            newInstance.MaxPlayers = MaxPlayers;
            newInstance.Protocol = Protocol;
            newInstance.ServerType = ServerType;
            newInstance.Environment = Environment;
            newInstance.Visibility = Visibility;
            newInstance.Mod = Mod;
            newInstance.Link = Link;
            newInstance.DownloadLink = DownloadLink;
            newInstance.Version = Version;
            newInstance.Size = Size;
            newInstance.Type = Type;
            newInstance.Dll = Dll;
            newInstance.Vac = Vac;
            newInstance.Bots = Bots;
            newInstance.Players = PlayersCopy();
            return newInstance;
        }
    }
}
