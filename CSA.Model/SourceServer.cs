using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Model
{
    public class SourceServer : BaseServer
    {
        public SourceServer (string address, int port)
            : base(address, port)
        {
        }

        private SteamApplicationId _Id;
        /// <summary>
        /// Steam Application ID of game. https://developer.valvesoftware.com/wiki/Steam_Application_ID
        /// </summary>
        public SteamApplicationId Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private string _Version;
        /// <summary>
        /// Version of the game installed on the server. 
        /// </summary>
        public string Version
        {
            get { return _Version; }
            set
            {
                _Version = value;
                RaisePropertyChanged(nameof(Version));
            }
        }
        
        private byte _Edf;
        /// <summary>
        /// Extra Data Flag (EDF) 	byte 	If present, this specifies which additional data fields will be included. 
        /// </summary>
        public byte Edf
        {
            get { return _Edf; }
            set { _Edf = value; }
        }


        public override string ToString()
        {
            return string.Format("Server name:{0}; Map:{1}; Players/Max:{2}/{3}", Name, Map, ActualPlayers, MaxPlayers);
        }

        public override BaseServer Copy()
        {
            SourceServer newInstance = new SourceServer(Address, Port);
            newInstance.Protocol = Protocol;
            newInstance.Name = Name;
            newInstance.Map = Map;
            newInstance.Folder = Folder;
            newInstance.Game = Game;
            newInstance.Id = Id;
            newInstance.ActualPlayers = ActualPlayers;
            newInstance.MaxPlayers = MaxPlayers;
            newInstance.Bots = Bots;
            newInstance.ServerType = ServerType;
            newInstance.Environment = Environment;
            newInstance.Visibility = Visibility;
            newInstance.Vac = Vac;
            newInstance.Version = Version;
            newInstance.Players = PlayersCopy();
            return newInstance;
        }
    }
}
