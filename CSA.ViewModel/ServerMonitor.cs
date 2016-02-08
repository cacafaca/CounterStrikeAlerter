using System;
using System.Collections.Generic;
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
                NotifyText = GetServerInfoNotification();

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
