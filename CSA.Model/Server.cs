using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Model
{
    public class Server : BaseModel
    {
        public Server (string address, int port)
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

        private string _Map;
        public string Map
        {
            get { return _Map; }
            set
            {
                _Map = value;
                RaisePropertyChanged(nameof(Map));
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string _Game;
        public string GameDir
        {
            get { return _Game; }
            set
            {
                _Game = value;
                RaisePropertyChanged(nameof(GameDir));
            }
        }

        private string _GameType;
        public string GameDescription
        {
            get { return _GameType; }
            set
            {
                _GameType = value;
                RaisePropertyChanged(nameof(GameDescription));
            }
        }

        private int _AppId;
        public int AppId
        {
            get { return _AppId; }
            set
            {
                _AppId = value;
                RaisePropertyChanged(nameof(AppId));
            }
        }

        private int _CurrentNumberOfPlayers;
        public int CurrentNumberOfPlayers
        {
            get { return _CurrentNumberOfPlayers; }
            set
            {
                _CurrentNumberOfPlayers = value;
                RaisePropertyChanged(nameof(CurrentNumberOfPlayers));
            }
        }

        private int _MaxPlayers;
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

        private OperatingSystem _OperatingSystem;
        public OperatingSystem OperatingSystem
        {
            get { return _OperatingSystem; }
            set
            {
                _OperatingSystem = value;
                RaisePropertyChanged(nameof(OperatingSystem));
            }
        }

        private bool _Password;
        public bool Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        private bool _Secure;
        public bool Secure
        {
            get { return _Secure; }
            set
            {
                _Secure = value;
                RaisePropertyChanged(nameof(Secure));
            }
        }

        private string _Version;
        public string Version
        {
            get { return _Version; }
            set
            {
                _Version = value;
                RaisePropertyChanged(nameof(Version));
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

        public override string ToString()
        {
            return string.Format("Server name:{0}; Map:{1}; Players/Max:{2}/{3}", _Name, _Map, _CurrentNumberOfPlayers, _MaxPlayers);
        }
    }
}
