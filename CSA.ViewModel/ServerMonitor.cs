using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSA.Common;

namespace CSA.ViewModel
{
    public class ServerMonitor : BaseViewModel, INotifyPropertyChanged
    {
        public ServerMonitor(Server server, TimeSpan monitorSleepInterval)
        {
            DefaultMonitorSleepInterval = new TimeSpan(0, 0, 5);
            if (monitorSleepInterval != null && monitorSleepInterval.TotalSeconds == 0)
                MonitorSleepInterval = monitorSleepInterval;
            else
                MonitorSleepInterval = DefaultMonitorSleepInterval;
            _Server = server;
            _PlayersChange = new ObservableCollection<Player>();
            MonitorWorker = new BackgroundWorker();
            MonitorWorker.DoWork += MonitorWorker_DoWork;
        }

        public ServerMonitor(Server server)
            : this(server, new TimeSpan())
        {
        }

        public ServerMonitor(string addressAndPort, TimeSpan monitorSleepInterval)
            : this(new Server(addressAndPort), monitorSleepInterval)
        {
        }

        public ServerMonitor(string addressAndPort)
            : this(addressAndPort, new TimeSpan())
        {
        }

        BackgroundWorker MonitorWorker;
        TimeSpan DefaultMonitorSleepInterval;
        TimeSpan MonitorSleepInterval;

        private void MonitorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                _PlayersChange = GetPlayersChange();
                if (_PlayersChange != null && _PlayersChange.Count > 0)
                {
                    RaisePropertyChanged(nameof(PlayersChange));
                    SendMailThatPlayersAreChanged();
                }

                System.Threading.Thread.Sleep(MonitorSleepInterval);
                if (MonitorWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void SendMailThatPlayersAreChanged()
        {
            if (_PlayersChange.Count > 0)
            {
                StringBuilder messageBody = new StringBuilder();
                messageBody.AppendLine("<html>");
                messageBody.AppendLine(@"
    <head>
        <style type=""text/css"">
            table, th, td {
                border: 1px solid black;
                border-collapse: collapse;
            }
            th, td {
                padding: 2px;
            }
        </style>
    </head>
"
                );
                messageBody.AppendLine(_Server.ServerModel.ToString() + "<br/><br/>New players:");
                messageBody.AppendLine(@"
<table>
    <thead>
        <tr>
            <th>Name</th>
            <th>Score</th>
        </tr>
    </thead>
");
                messageBody.AppendLine("<tbody>");
                foreach (var player in _PlayersChange)
                {                     
                    messageBody.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", player.Name, player.Score));
                }
                messageBody.AppendLine("</tbody>");
                messageBody.AppendLine("</table>");
                messageBody.AppendLine("</html>");
                SendMail sm = new SendMail();
                sm.Send("nemanja.simovic@brezna.info",
                    string.Format("Players changed on server '{0}'. Map '{1}", _Server.ServerModel.Name, _Server.ServerModel.Map),
                    messageBody.ToString());
            }
        }

        public void RefreshPlayers()
        {
            _PlayersChange = new ObservableCollection<Player>(_Server.Players.ToList());
            RaisePropertyChanged(nameof(PlayersChange));
        }

        private ObservableCollection<Player> _PlayersChange;
        public ObservableCollection<Player> PlayersChange
        {
            get { return _PlayersChange; }
        }

        private ObservableCollection<Player> GetPlayersChange()
        {
            var playersChanged = new ObservableCollection<Player>();
            try
            {
                if (_Server.QueryServer())
                {
                    if (_Server.ServerModel.ActualPlayers > 0)
                    {
                        IEnumerable<string> newPlayerDifferences = _Server.ServerModel.Players.Select(player => player.Name).Except(OldPlayerNamesList);

                        // Look for a new players.
                        if (newPlayerDifferences.Count() > 0)
                        {
                            byte index = 1;
                            foreach (var player in _Server.ServerModel.Players.Where(player => newPlayerDifferences.Any(playerName => playerName == player.Name)).OrderByDescending(player => player.Score))
                            {
                                playersChanged.Add(new Player()
                                {
                                    Index = index++,
                                    Name = player.Name,
                                    Score = player.Score,
                                    Duration = player.Duration
                                });
                            }
                            OldPlayerNamesList.Clear();
                            foreach (var player in _Server.ServerModel.Players)
                                OldPlayerNamesList.Add(player.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "GetPlayersChange()");
            }

            return playersChanged;
        }

        List<string> OldPlayerNamesList = new List<string>();

        public void StartMonitoring()
        {
            MonitorWorker.RunWorkerAsync();
        }

        Server _Server;
        public Server Server
        {
            get { return _Server; }
            set
            {
                _Server = value;
                RaisePropertyChanged(nameof(Server));
            }
        }
    }
}
