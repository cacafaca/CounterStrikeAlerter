using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using CSA.Model;

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
            Server = new ViewModel.Server("192.168.0.147:27015");
            GmailSettings = new GmailSettings();
            GmailSettings.Load();

            EndRoundWorker = new BackgroundWorker();
            EndRoundWorker.DoWork += ListenEndRound;
        }

        private void ListenEndRound(object sender, DoWorkEventArgs e)
        {
            Model.BaseServer oldServer;
            Server.QueryServer();
            oldServer = Server.ServerModel.Copy();
            List<Model.Player> allTimePlayers = new List<Model.Player>();
            Sleep();
            while (CanWork)
            {
                try
                {
                    Server.QueryServer();
                    UpdatePlayersStats(allTimePlayers, oldServer.Players);
                    UpdatePlayersStats(allTimePlayers, Server.ServerModel.Players);
                    if (IsNewRound(oldServer, Server.ServerModel))
                    {
                        ReportEndRoundStats(oldServer, allTimePlayers);
                        allTimePlayers.Clear();
                    }
                    oldServer = Server.ServerModel.Copy();
                }
                catch (Exception ex)
                {
                    Common.Logger.TraceWriteLine(ex.Message);
                }

                Sleep();
                if (EndRoundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void UpdatePlayersStats(List<Model.Player> allTimePlayers, IEnumerable<Model.Player> newPlayers)
        {
            // Update existing players
            foreach (var player in allTimePlayers.Intersect(newPlayers, new PlayerComparer()))
            {
                int oldScore = newPlayers.Where(allPlayer => allPlayer.Name == player.Name).First().Score;
                if (oldScore > 0 && oldScore > player.Score)
                    player.Score = oldScore;
            }

            // Add new players
            allTimePlayers.AddRange(newPlayers.Except(allTimePlayers, new PlayerComparer()));
        }

        public bool CancellationPending { get { return EndRoundWorker.CancellationPending; } }

        private void Sleep()
        {
            System.Threading.Thread.Sleep(Properties.Settings.Default.QueryServerSeconds * 1000);
        }

        GmailSettings GmailSettings;

        private void ReportEndRoundStats(Model.BaseServer server, List<Model.Player> allTimePlayers)
        {
            CSA.ViewModel.SendMail sendMail = new ViewModel.SendMail(GmailSettings.GmailUser, GmailSettings.GmailEncryptedPassword);
            string subject = string.Format("Statistic for server {0} at {1} on map {2} at the time {3} {4}", server.Name, server.AddressAndPort(),
                server.Map, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
            string body = GenerateBody(server, allTimePlayers);
            foreach (var address in GmailSettings.SendToAddressesList())
            {
                sendMail.Send(address, subject, body);
            }

        }

        private string GenerateBody(Model.BaseServer server, List<Model.Player> allTimePlayers)
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
            messageBody.AppendLine(server.ToString() + "<br/><br/>End round stats:");
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
            foreach (var player in allTimePlayers.OrderByDescending(player => player.Score))
            {
                messageBody.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", player.Name, player.Score));
            }
            messageBody.AppendLine("</tbody>");
            messageBody.AppendLine("</table>");
            messageBody.AppendLine("</html>");

            return messageBody.ToString();
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
            if (samePlayersCount > 0)
            {
                int samePlayersWithResetedScore = samePlayers.Where(player => (player.NewScore - player.OldScore < 0) || (player.NewScore == 0 && player.OldScore > 0)).Count();
                Common.Logger.TraceWriteLine("samePlayersWithResetedScore = " + samePlayersWithResetedScore.ToString());
                double newPlayersResetedScoreRate = (double)samePlayersWithResetedScore / (double)samePlayersCount * 100;
                Common.Logger.TraceWriteLine("newPlayersResetedScoreRate = " + newPlayersResetedScoreRate.ToString("N"));
                bool newRound = newPlayersResetedScoreRate >= 50;
                return newRound;
            }
            else
                return false;
        }

        public void Stop()
        {
            CanWork = false;
        }
    }

    class PlayerComparer : IEqualityComparer<Model.Player>
    {
        public bool Equals(Player x, Player y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Player obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Calculate the hash code for the product.
            return obj.Name == null ? 0 : obj.Name.GetHashCode();
        }
    }
}
