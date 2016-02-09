﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class ServerMonitor : INotifyPropertyChanged
    {
        public ServerMonitor(Server server)
        {
            Server = server;
            InitializeMonitorWorker();
        }

        public ServerMonitor(string address, int port)
            : this(new Server(address, port))
        {
        }

        BackgroundWorker MonitorWorker;
        Server Server;
        TimeSpan MonitorSleepInterval;

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeMonitorWorker()
        {
            MonitorSleepInterval = new TimeSpan(0, 0, 5);
            MonitorWorker = new BackgroundWorker();
            MonitorWorker.DoWork += MonitorWorker_DoWork;
        }

        private void MonitorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                //NotifyText = GetServerInfoNotification();

                // Player change.
                string newNotification = GetPlayerChanges();
                if (IsPlayersChanged)
                    NotifyText = newNotification;

                System.Threading.Thread.Sleep(MonitorSleepInterval);
                if (MonitorWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private string GetServerInfoNotification()
        {
            Server.QueryServer();
            StringBuilder notification = new StringBuilder();
            notification.AppendLine(Server.ServerModel.ToString());
            foreach (var player in Server.ServerModel.Players.OrderByDescending(player => player.Frags))
                notification.AppendLine(player.ToString());
            return notification.ToString();
        }

        private string GetPlayerChanges()
        {
            Server.QueryServer();
            StringBuilder notification = new StringBuilder();

            IEnumerable<string> newPlayerDifferences = Server.ServerModel.Players.Select(player => player.Name).Except(OldPlayerNamesList);

            // Look for a new players.
            if (newPlayerDifferences.Count() > 0)
            {
                IsPlayersChanged = true;
                notification.AppendLine(Server.ServerModel.ToString());
                foreach (var player in Server.ServerModel.Players.Where(player => newPlayerDifferences.Any(playerName => playerName == player.Name)).OrderByDescending(player => player.Frags))
                {
                    notification.AppendLine("New player> " + player.ToString());
                }
                OldPlayerNamesList.Clear();
                foreach (var player in Server.ServerModel.Players)
                    OldPlayerNamesList.Add(player.Name);
            }
            else
                IsPlayersChanged = false;

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

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
