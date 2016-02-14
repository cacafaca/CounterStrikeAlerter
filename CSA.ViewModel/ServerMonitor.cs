using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class ServerMonitor : BaseViewModel, INotifyPropertyChanged
    {
        public ServerMonitor(Server server, TimeSpan monitorSleepInterval)
        {
            if (monitorSleepInterval != null && monitorSleepInterval.TotalSeconds == 0)
                MonitorSleepInterval = monitorSleepInterval;
            else
                MonitorSleepInterval = DefaultMonitorSleepInterval;
            _Server = server;
            InitializeMonitorWorker();
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

        private void InitializeMonitorWorker()
        {
            DefaultMonitorSleepInterval = new TimeSpan(0, 0, 5);
            MonitorWorker = new BackgroundWorker();
            MonitorWorker.DoWork += MonitorWorker_DoWork;
        }

        private void MonitorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                string newNotification = GetPlayerChanges();
                if (IsPlayersChanged)
                {
                    IsPlayersChanged = false;
                    NotifyText = newNotification;
                }

                System.Threading.Thread.Sleep(MonitorSleepInterval);
                if (MonitorWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private string GetPlayerChanges()
        {
            StringBuilder notification = new StringBuilder();

            if (_Server.QueryServer())
            {
                if (_Server.ServerModel.ActualPlayers > 0)
                {
                    IEnumerable<string> newPlayerDifferences = _Server.ServerModel.Players.Select(player => player.Name).Except(OldPlayerNamesList);

                    // Look for a new players.
                    if (newPlayerDifferences.Count() > 0)
                    {
                        IsPlayersChanged = true;
                        notification.AppendLine(_Server.ServerModel.ToString());
                        foreach (var player in _Server.ServerModel.Players.Where(player => newPlayerDifferences.Any(playerName => playerName == player.Name)).OrderByDescending(player => player.Score))
                        {
                            notification.AppendLine("New player> " + player.ToString());
                        }
                        OldPlayerNamesList.Clear();
                        foreach (var player in _Server.ServerModel.Players)
                            OldPlayerNamesList.Add(player.Name);
                    }
                    else
                        IsPlayersChanged = false;
                }
                else
                {
                    notification.AppendLine(_Server.ServerModel.ToString());
                    notification.AppendLine("No players.");
                }
            }
            else
            {
                notification.Append(string.Format("Can't read server info at " + _Server.ServerModel.AddressAndPort()));
            }

            return notification.ToString();
        }

        List<string> OldPlayerNamesList = new List<string>();
        bool IsPlayersChanged = false;

        public void StartMonitoring()
        {
            MonitorWorker.RunWorkerAsync();
        }

        private string _NotifyText;

        public string NotifyText
        {
            get { return _NotifyText; }
            set
            {
                _NotifyText = value;
                RaisePropertyChanged(nameof(NotifyText));
            }
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
