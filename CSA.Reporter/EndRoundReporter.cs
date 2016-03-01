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
            EndRoundWorker.DoWork += ListernEndRound;
        }

        private void ListernEndRound(object sender, DoWorkEventArgs e)
        {
            Model.BaseServer oldServer;
            Server.QueryServer();
            oldServer = Server.ServerModel.Copy();
            Sleep();
            while (CanWork)
            {
                Server.QueryServer();
                if (IsNewRound(oldServer, Server.ServerModel))
                    ReportEndRoundStats();
                oldServer = Server.ServerModel.Copy();
                Sleep();


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

        private void ReportEndRoundStats()
        {
            throw new NotImplementedException();
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

            int samoPlayersCount = samePlayers.Count();

            return samePlayers.Where(player => (player.NewScore - player.OldScore < 0 ) || (player.NewScore == 0 && player.OldScore > 0)
            }
        }

        public void Stop()
        {
            CanWork = false;
        }
    }
}
