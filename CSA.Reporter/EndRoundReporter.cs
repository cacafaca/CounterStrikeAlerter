using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Reporter
{
    public class EndRoundReporter
    {
        public void Start()
        {
            CanWork = true;
            EndRoundWorker.RunWorkerAsync();
        }

        private BackgroundWorker EndRoundWorker;
        private bool CanWork;
        private CSA.ViewModel.Server Server;

        public EndRoundReporter()
        {
            CanWork = false;
            Server = new ViewModel.Server("192.162.0.147:27015:");
            EndRoundWorker = new BackgroundWorker();
            EndRoundWorker.DoWork += ListenEndRound;
        }

        private void ListenEndRound(object sender, DoWorkEventArgs e)
        {
            Model.BaseServer oldServer;
            Server.QueryServer();
            oldServer = Server.ServerModel.Copy();
            Sleep();
            while (CanWork)
            {
                try
                {
                    Server.QueryServer();
                    if (IsNewRound(oldServer, Server.ServerModel))
                        ReportEndRoundStats(oldServer);
                    oldServer = Server.ServerModel.Copy();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                Sleep();
                if (EndRoundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void Sleep()
        {
            System.Threading.Thread.Sleep(SleepTime);
        }

        private int SleepTime = 10000;

        private void ReportEndRoundStats(Model.BaseServer server)
        {
            CSA.ViewModel.SendMail sendMail = new ViewModel.SendMail();
            string to = "nemanja.simovic@brezna.info";
            string subject = string.Format("Statistic for server {0} at {1} on map {2}", server.Name, server.ToString(), server.Map);
            string body = string.Empty;
            foreach (var player in server.Players)
                body += player.ToString() + Environment.NewLine;
            sendMail.Send(to, subject, body);
        }

        private bool IsNewRound(Model.BaseServer oldServer, Model.BaseServer newServer)
        {
            if (oldServer == null || newServer == null)
                return false;

            if (oldServer.Map != newServer.Map)
                return true;

            var samePlayers = newServer.Players.Join(
                oldServer.Players,
                newPlayer => new { newPlayer.Name },
                oldPlayer => new { oldPlayer.Name },
                (np, op) => new { Name = np.Name, NewScore = np.Score, OldScore = op.Score });

            int samePlayersCount = samePlayers.Count();
            int samePlayersWithResetedScore = samePlayers.Where(player => (player.NewScore - player.OldScore < 0) || (player.NewScore == 0 && player.OldScore > 0)).Count();
            return samePlayersWithResetedScore / samePlayersCount * 100 > 70;
        }

        public void Stop()
        {
            CanWork = false;
        }
    }
}
