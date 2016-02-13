using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Model
{
    /// <summary>
    /// Base class that holds common data for Source and GoldSource of server type.
    /// </summary>
    public abstract class BaseServer : BaseModel
    {
        public BaseServer(string address, int port)
        {
            _Address = address;
            _Port = port;
            _Players = new ObservableCollection<Player>();
        }

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                RaisePropertyChanged(nameof(Address));
            }
        }

        int _Port;
        public int Port
        {
            get { return _Port; }
            set
            {
                _Port = value;
                RaisePropertyChanged(nameof(Port));
            }
        }

        private ObservableCollection<Player> _Players;
        public ObservableCollection<Player> Players
        {
            get { return _Players; }
            set
            {
                _Players = value;
                RaisePropertyChanged(nameof(Players));
            }
        }

        private Header _Header;
        /// <summary>
        /// Source: Always equal to 'I' (0x49) 
        /// GoldSource: Always equal to 'm' (0x6D) 
        /// </summary>
        public Header Header
        {
            get { return _Header; }
            set { _Header = value; }
        }

        private byte _Protocol;
        /// <summary>
        /// Protocol version used by the server. 
        /// </summary>
        public byte Protocol
        {
            get { return _Protocol; }
            set { _Protocol = value; }
        }

        private string _Name;
        /// <summary>
        /// Name of the server. 
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string _Map;
        /// <summary>
        /// Map the server has currently loaded. 
        /// </summary>
        public string Map
        {
            get { return _Map; }
            set
            {
                _Map = value;
                RaisePropertyChanged(nameof(Map));
            }
        }

        private string _Folder;
        /// <summary>
        /// Name of the folder containing the game files. 
        /// </summary>
        public string Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
                RaisePropertyChanged(nameof(Folder));
            }
        }

        private string _Game;
        /// <summary>
        /// Full name of the game. 
        /// </summary>
        public string Game
        {
            get { return _Game; }
            set
            {
                _Game = value;
                RaisePropertyChanged(nameof(Game));
            }
        }

        private int _CurrentNumberOfPlayers;
        /// <summary>
        /// Number of players on the server. 
        /// </summary>
        public int ActualPlayers
        {
            get { return _CurrentNumberOfPlayers; }
            set
            {
                _CurrentNumberOfPlayers = value;
                RaisePropertyChanged(nameof(ActualPlayers));
            }
        }

        private int _MaxPlayers;
        /// <summary>
        /// Maximum number of players the server reports it can hold.
        /// </summary>
        public int MaxPlayers
        {
            get { return _MaxPlayers; }
            set
            {
                _MaxPlayers = value;
                RaisePropertyChanged(nameof(MaxPlayers));
            }
        }

        private byte _Bots;
        /// <summary>
        /// Number of bots on the server. 
        /// </summary>
        public byte Bots
        {
            get { return _Bots; }
            set
            {
                _Bots = value;
                RaisePropertyChanged(nameof(Bots));
            }
        }

        private ServerType _ServerType;
        public ServerType ServerType
        {
            get { return _ServerType; }
            set
            {
                _ServerType = value;
                RaisePropertyChanged(nameof(ServerType));
            }
        }

        private Environment _OperatingSystem;
        public Environment Environment
        {
            get { return _OperatingSystem; }
            set
            {
                _OperatingSystem = value;
                RaisePropertyChanged(nameof(Environment));
            }
        }

        private Visibility _Visibility;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                _Visibility = value;
                RaisePropertyChanged(nameof(Visibility));
            }
        }

        private Vac _Vac;
        /// <summary>
        /// Specifies whether the server uses VAC.
        /// </summary>
        public Vac Vac
        {
            get { return _Vac; }
            set { _Vac = value; }
        }

        public string AddressAndPort()
        {
            return _Address + ":" + _Port.ToString();
        }

        public override string ToString()
        {
            return string.Format("Address:{0}; Port:{1}", _Address, _Port);
        }
    }
}
