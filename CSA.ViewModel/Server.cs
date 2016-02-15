using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class Server : BaseViewModel
    {
        /**
        https://developer.valvesoftware.com/wiki/Server_queries
        */

        private CSA.Model.BaseServer _ServerModel;

        public Model.BaseServer ServerModel
        {
            get
            {
                if (_ServerModel != null)
                    return _ServerModel;
                else
                    throw new Exception("Read server first.");
            }
        }

        private Socket SocketUDP;

        public Server(string address, int port)
        {
            ConstructiorInitialization(address, port);
        }

        public Server(string addressAndPort)
        {
            string address;
            int port;
            ParseServerAndPort(addressAndPort.Trim(), out address, out port);
            ConstructiorInitialization(address, port);
        }

        private string Address;
        private int Port;
        IPEndPoint EndPoint;

        private void ConstructiorInitialization(string address, int port)
        {
            Address = address;
            Port = port;
            SocketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketUDP.Blocking = false;
            EndPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
        }

        private void ParseServerAndPort(string addressAndPort, out string address, out int port)
        {
            int delimiterPosition = addressAndPort.IndexOf(':');
            address = addressAndPort.Substring(0, delimiterPosition);
            int.TryParse(addressAndPort.Substring(delimiterPosition + 1), out port);
        }

        public bool QueryServerHeader()
        {
            byte[] basicInfo = null;
            AskServer(Request.ServerInfo, out basicInfo);
            if (basicInfo == null)
                return false;
            Response response = new Response(basicInfo);
            ParseBasicInfo(response);

            Name = _ServerModel.Name;
            Map = _ServerModel.Map;
            CurrentPlayers = string.Format("{0}/{1}", _ServerModel.ActualPlayers, _ServerModel.MaxPlayers);

            return true;
        }

        private void ParseBasicInfo(Response response)
        {
            if (response != null)
            {

                byte engineIndicator = response.GetNextByte();
                if (engineIndicator == (byte)Model.Header.Source)
                {
                    _ServerModel = new Model.SourceServer(Address, Port);
                    _ServerModel.Header = Model.Header.Source;
                    //_ServerModel.Players.CollectionChanged += ServerModelPlayers_CollectionChanged;
                    ParseBasicInfoForSource(response);
                }
                else if (engineIndicator == (byte)Model.Header.GoldSource)
                {
                    _ServerModel = new Model.GoldSourceServer(Address, Port);
                    _ServerModel.Header = Model.Header.GoldSource;
                    ParseBasicInfoForGoldSource(response);
                }
                else
                {
                    throw new Exception("Unknown server header.");
                }
            }
        }

        private void ServerModelPlayers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Players));
        }

        private void ParseBasicInfoForSource(Response response)
        {
            var serverModel = _ServerModel as Model.SourceServer;
            if (serverModel != null)
            {
                serverModel.Protocol = response.GetNextByte();
                serverModel.Name = response.GetNextString();
                serverModel.Map = response.GetNextString();
                serverModel.Folder = response.GetNextString();
                serverModel.Game = response.GetNextString();
                serverModel.Id = (Model.SteamApplicationId)response.GetNextShort();
                serverModel.ActualPlayers = response.GetNextByte();
                serverModel.MaxPlayers = response.GetNextByte();
                serverModel.Bots = response.GetNextByte();
                serverModel.ServerType = (Model.ServerType)response.GetNextByte();
                serverModel.Environment = (Model.Environment)response.GetNextByte();
                serverModel.Visibility = (Model.Visibility)response.GetNextByte();
                serverModel.Vac = (Model.Vac)response.GetNextByte();
                if (serverModel.Id == Model.SteamApplicationId.TheShip)
                {
                    // TODO: For The Ship game.
                }
                serverModel.Version = response.GetNextString();
            }
        }

        private void ParseBasicInfoForGoldSource(Response response)
        {
            var serverModel = _ServerModel as Model.GoldSourceServer;
            if (serverModel != null)
            {
                serverModel.InternalAddress = response.GetNextString();
                serverModel.Name = response.GetNextString();
                serverModel.Map = response.GetNextString();
                serverModel.Folder = response.GetNextString();
                serverModel.Game = response.GetNextString();
                serverModel.ActualPlayers = response.GetNextByte();
                serverModel.MaxPlayers = response.GetNextByte();
                serverModel.Protocol = response.GetNextByte();
                serverModel.ServerType = (Model.ServerType)response.GetNextByte();
                serverModel.Environment = (Model.Environment)response.GetNextByte();
                serverModel.Visibility = (Model.Visibility)response.GetNextByte();
                serverModel.Mod = (Model.GoldSourceMod)response.GetNextByte();
                if (serverModel.Mod == Model.GoldSourceMod.HalfLifeMod)
                {
                    serverModel.Link = response.GetNextString();
                    serverModel.DownloadLink = response.GetNextString();
                    if (response.GetNextByte() != 0x00)
                        throw new ArgumentException("Expect NULL (0x00).");
                    serverModel.Version = response.GetNextInt();
                    serverModel.Size = response.GetNextInt();
                    serverModel.Type = (Model.GoldSourceModType)response.GetNextByte();
                    serverModel.Dll = (Model.GoldSourceModDll)response.GetNextByte();
                }
                serverModel.Vac = (Model.Vac)response.GetNextByte();
                serverModel.Bots = response.GetNextByte();
            }
        }

        private byte GetNextByte(ref byte[] data, ref int position)
        {
            return data[position++];
        }

        private char GetNextCharAndRemove(ref string raw)
        {
            char value = raw[0];
            raw = raw.Remove(0, 1);
            return value;
        }

        const byte ResponseHeader = 0x44;

        private void ParsePlayersInfo(Response response)
        {
            if (response != null)
            {
                // Strip header
                byte header = response.GetNextByte();
                if (header == ResponseHeader)
                {
                    byte numberOfPlayers = response.GetNextByte();
                    for (byte i = 0; i < numberOfPlayers; i++)
                    {
                        var player = new Model.Player();
                        player.Index = response.GetNextByte();
                        player.Name = response.GetNextString();
                        player.Score = response.GetNextInt();
                        player.Duration = TimeSpan.FromSeconds(response.GetNextFloat());
                        _ServerModel.Players.Add(player);
                    }
                }
                else
                    throw new ArgumentException("Expect 0x44");
            }
        }

        public bool QueryPlayers()
        {
            bool headerWasRead = _ServerModel != null;
            if (!headerWasRead)
                headerWasRead = QueryServerHeader();
            if (headerWasRead)
            {
                _ServerModel.Players.Clear();

                // Challenge server before fetching players list.
                byte[] challenge = null;
                AskServer(Request.PlayerInfoChallenge, out challenge);
                if (challenge == null)
                    return false;
                Response response = new Response(challenge);

                // Get Players
                byte[] playerInfo = null;
                AskServer(Request.GetPlayersRequest(response.GetChallenge()), out playerInfo);
                if (playerInfo == null)
                    return false;
                response = new Response(playerInfo);
                ParsePlayersInfo(response);
                RaisePropertyChanged(nameof(Players));
                return true;
            }
            else
                return false;
        }

        Request Request = new Request();

        private void AskServer(byte[] request, out byte[] response)
        {
            response = null;
            if (request != null)
            {
                var x = SocketUDP.SendTo(request, EndPoint);

                int count = 0;
                var receiveEndPoint = EndPoint as EndPoint;
                try
                {
                    int retry = 0;
                    while (SocketUDP.Available == 0 && retry < 3)
                    {
                        System.Threading.Thread.Sleep(100); // Wait for an answer.
                        retry++;
                    }
                    if (SocketUDP.Available > 0)
                    {
                        response = new byte[SocketUDP.Available];
                        count = SocketUDP.ReceiveFrom(response, ref receiveEndPoint);
                        if (SocketUDP.Available > 0)
                        {
                            throw new Exception("More to read.");
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public bool QueryServer()
        {
            if (QueryServerHeader() && _ServerModel.ActualPlayers > 0)
                return QueryPlayers();
            else
                return false;
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

        private string _CurrentPlayers;

        public string CurrentPlayers
        {
            get { return _CurrentPlayers; }
            set
            {
                _CurrentPlayers = value;
                RaisePropertyChanged(nameof(CurrentPlayers));
            }
        }

        private ObservableCollection<Player> _Players;

        public ObservableCollection<Player> Players
        {
            get
            {
                var oc = new ObservableCollection<Player>();
                if (_ServerModel != null && _ServerModel.Players != null)
                {
                    var pl = _ServerModel.Players.ToList();
                    foreach (var player in pl)
                        if (player != null)
                            oc.Add(new Player()
                            {
                                Index = player.Index,
                                Name = player.Name,
                                Score = player.Score,
                                Duration = player.Duration
                            });
                }
                return oc;
            }
        }

    }

}
