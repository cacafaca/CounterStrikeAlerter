﻿using System;
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
        /**
        https://developer.valvesoftware.com/wiki/Server_queries
        */

        const string A2S_INFO = "TSource Engine Query\x00"; // Basic information about the server. 
        const string A2S_PLAYER = "U\xFF\xFF\xFF\xFF";


        private CSA.Model.BaseServer _ServerModel;
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

        private void ConstructiorInitialization(string address, int port)
        {
            Address = address;
            Port = port;
            SocketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketUDP.Blocking = false;
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
            return true;
        }

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

        private void ParseBasicInfo(Response response)
        {
            if (response != null)
            {

                byte engineIndicator = response.GetNextByte();
                if (engineIndicator == (byte)Model.Header.Source)
                {
                    _ServerModel = new Model.SourceServer(Address, Port);
                    _ServerModel.Header = Model.Header.Source;
                    ParseBasicInfoForSource(response);
                }
                else if (engineIndicator == (byte)Model.Header.GoldSource)
                {
                    _ServerModel = new Model.GoldSourceServer(Address, Port);
                    ServerModel.Header = Model.Header.Source;
                    ParseBasicInfoForGoldSource(response);
                }
                else
                {
                    throw new Exception("Unknown server header.");
                }
            }
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

        private void ParsePlayersInfo(Response response)
        {
            if (response != null)
            {
                // Strip header
                byte header = response.GetNextByte();
                if (header == 0x44)
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
                var endPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
                var x = SocketUDP.SendTo(request, endPoint);

                byte[] wholeResponse = new byte[10240];
                int count = 0;
                var endPoint2 = endPoint as EndPoint;
                System.Threading.Thread.Sleep(1000); // Give server one second to respond.
                try
                {
                    count = SocketUDP.ReceiveFrom(wholeResponse, ref endPoint2);
                    if (count > 0 && count <= wholeResponse.Length)
                    {
                        response = new byte[count];
                        Array.Copy(wholeResponse, response, count);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public bool QueryServer()
        {
            if (QueryServerHeader() && ServerModel.ActualPlayers > 0)
                return QueryPlayers();
            else
                return false;
        }
    }

}
