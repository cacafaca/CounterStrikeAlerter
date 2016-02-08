using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class Server
    {
        private CSA.Model.Server _ServerModel;
        private Socket SocketUDP;

        public Server(string address, int port)
        {
            _ServerModel = new CSA.Model.Server(address, port);
            SocketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketUDP.Blocking = false;
        }

        public void QueryServerHeader()
        {
            string serverQueryStr = "\xFF\xFF\xFF\xFFTSource Engine Query\x00";
            string output = AskServer(serverQueryStr);
            ParseServerInfo(output);
        }

        public Model.Server ServerModel { get { return _ServerModel; } }

        private void ParseServerInfo(string rawServerInfo)
        {
            if (!string.IsNullOrEmpty(rawServerInfo))
            {
                // Remove unnecessary header data
                rawServerInfo = rawServerInfo.Remove(0, 6);

                _ServerModel.Name = GetNextStringAndRemove(ref rawServerInfo);
                _ServerModel.Map = GetNextStringAndRemove(ref rawServerInfo);
                _ServerModel.GameDir = GetNextStringAndRemove(ref rawServerInfo);
                _ServerModel.GameDescription = GetNextStringAndRemove(ref rawServerInfo);
                _ServerModel.AppId = GetNextShortAndRemove(ref rawServerInfo);
                _ServerModel.CurrentNumberOfPlayers = GetNextByteAndRemove(ref rawServerInfo);
                _ServerModel.MaxPlayers = GetNextByteAndRemove(ref rawServerInfo);
                _ServerModel.Bots = GetNextByteAndRemove(ref rawServerInfo);
                _ServerModel.ServerType = (Model.ServerType)GetNextByteAndRemove(ref rawServerInfo);
                _ServerModel.OperatingSystem = (Model.OperatingSystem)GetNextByteAndRemove(ref rawServerInfo);
                _ServerModel.Password = GetNextByteAndRemove(ref rawServerInfo) > 0;
                _ServerModel.Secure = GetNextByteAndRemove(ref rawServerInfo) > 0;
                if (_ServerModel.AppId == 2400)
                    for (int i = 0; i < 3; i++)
                        GetNextByteAndRemove(ref rawServerInfo);
                _ServerModel.Version = GetNextStringAndRemove(ref rawServerInfo);
                /*var edf = GetNextByteAndRemove(ref rawServerInfo);
                short gamePort;
                if ((edf & 0x80) > 0)
                    gamePort = GetNextShortAndRemove(ref rawServerInfo); */
            }
        }

        private string GetNextStringAndRemove(ref string raw)
        {
            var endPosition = raw.IndexOf('\x0');
            string value = raw.Substring(0, endPosition);
            raw = raw.Remove(0, endPosition + 1);
            return value;
        }

        private short GetNextShortAndRemove(ref string raw)
        {
            short value;
            int size = sizeof(System.Int16);
            byte[] b = Encoding.Default.GetBytes(raw.Substring(0, size));
            value = BitConverter.ToInt16(b, 0);
            raw = raw.Remove(0, size);
            return value;
        }

        private int GetNextIntAndRemove(ref string raw)
        {
            int value;
            int size = sizeof(System.Int32);
            byte[] b = Encoding.Default.GetBytes(raw.Substring(0, size));
            value = BitConverter.ToInt32(b, 0);
            raw = raw.Remove(0, size);
            return value;
        }

        private float GetNextFloatAndRemove(ref string raw)
        {
            float value;
            int size = sizeof(System.Single);
            byte[] b = Encoding.Default.GetBytes(raw.Substring(0, size));
            value = BitConverter.ToSingle(b, 0);
            raw = raw.Remove(0, size);
            return value;
        }

        private byte GetNextByteAndRemove(ref string raw)
        {
            byte value;
            int size = sizeof(System.Byte);
            byte[] b = Encoding.Default.GetBytes(raw.Substring(0, size));
            value = b[0];
            raw = raw.Remove(0, size);
            return value;
        }

        private void ParsePlayersInfo(string rawPlayers)
        {
            if (!string.IsNullOrEmpty(rawPlayers))
            {
                // Strip header
                rawPlayers = rawPlayers.Remove(0, 5);

                byte numberOfPlayers = GetNextByteAndRemove(ref rawPlayers);
                for (byte i = 0; i < numberOfPlayers; i++)
                {
                    var player = new Model.Player();
                    GetNextByteAndRemove(ref rawPlayers);
                    player.Name = GetNextStringAndRemove(ref rawPlayers);
                    player.Frags = GetNextIntAndRemove(ref rawPlayers);
                    player.Time = TimeSpan.FromSeconds(GetNextFloatAndRemove(ref rawPlayers));
                    _ServerModel.Players.Add(player);
                }
            }
        }

        public void QueryPlayers()
        {
            _ServerModel.Players.Clear();

            // Challenge server before fetching players list.
            string challengeQueryStr = "\xFF\xFF\xFF\xFFU\xFF\xFF\xFF\xFF";
            string output = AskServer(challengeQueryStr);
            if (!string.IsNullOrEmpty(output))
            {
                string challenge = output.Remove(0, 5);

                // Players
                string playersQueryStr = "\xFF\xFF\xFF\xFFU" + challenge;
                output = AskServer(playersQueryStr);
                ParsePlayersInfo(output);
            }
        }

        private string AskServer(string query)
        {
            byte[] serverQueryByte = Encoding.Default.GetBytes(query);
            var endPoint = new IPEndPoint(IPAddress.Parse(_ServerModel.Address), _ServerModel.Port);
            var x = SocketUDP.SendTo(serverQueryByte, endPoint);

            byte[] receive = new byte[10240];
            var endPoint2 = endPoint as EndPoint;
            string output = string.Empty;
            System.Threading.Thread.Sleep(1000); // Give server one second to respond.
            int count;
            try
            {
                count = SocketUDP.ReceiveFrom(receive, ref endPoint2);
                if (count > 0)
                {
                    output = Encoding.Default.GetString(receive).Remove(count);
                }
            }
            catch (Exception ex)
            {

            }
            return output;
        }

        public void QueryServer()
        {
            QueryServerHeader();
            QueryPlayers();
        }
    }

}
