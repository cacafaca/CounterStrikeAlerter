using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class ServerMonitor
    {
        public ServerMonitor(Server server)
        {
            Server = server;

            InitializeMonitorWorker();
        }
        BackgroundWorker MonitorWorker;
        Server Server;

        private void InitializeMonitorWorker()
        {
            MonitorWorker = new BackgroundWorker();
            MonitorWorker.DoWork += MonitorWorker_DoWork;
        }

        private void MonitorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {

            }
        }

        public void StartMonitoring()
        {
            MonitorWorker.RunWorkerAsync();
        }
    }
}
