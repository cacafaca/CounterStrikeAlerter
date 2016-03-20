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
            Server = new ViewModel.Server(Properties.Settings.Default.CounterStrikeServerAndPort);
            GmailSettings = new GmailSettings();
            GmailSettings.Load();

            EndRoundWorker = new BackgroundWorker();
            EndRoundWorker.DoWork += ListenEndRound;
        }

        private void ListenEndRound(object sender, DoWorkEventArgs e)
        {
            Model.BaseServer oldServer = null;
            int failCount = 0;
            while (failCount < 3)
            {
                try
                {
                    Server.QueryServer();
                    oldServer = Server.ServerModel.Copy();
                    break;
                }
                catch (Exception ex)
                {
                    Common.Logger.TraceWriteLine(ex.Message, ex.GetType().Name);
                    failCount++;
                }
            }
            List<Model.Player> allTimePlayers = new List<Model.Player>();
            UpdatePlayersStats(allTimePlayers, Server.ServerModel.Players);

            Sleep();

            while (CanWork)
            {
                try
                {
                    Server.QueryServer();
                    if (oldServer.Map == Server.Map)
                        UpdatePlayersStats(allTimePlayers, Server.ServerModel.Players);
                    if (IsNewRound(oldServer, Server.ServerModel))
                    {
                        ReportEndRoundStats(oldServer, allTimePlayers);
                        allTimePlayers.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Common.Logger.TraceWriteLine(ex.Message);
                }
                finally
                {
                    oldServer = Server.ServerModel.Copy();
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
            if (newPlayers != null && newPlayers.Count() > 0)
            {
                // Update existing players
                foreach (var player in allTimePlayers.Join(newPlayers,
                    allTimePlayer => new { Name = allTimePlayer.Name },
                    newPlayer => new { Name = newPlayer.Name },
                    (allTimePlayer, newPlayer) => new { Name = allTimePlayer.Name, OldScore = allTimePlayer.Score, NewScore = newPlayer.Score })
                    .Where(player => player.NewScore > player.OldScore))
                {
                    allTimePlayers.Where(allTimePlayer => allTimePlayer.Name == player.Name).First().Score = player.NewScore;
                }

                // Add new players
                allTimePlayers.AddRange(newPlayers.Except(allTimePlayers, new PlayerComparer()));
            }
        }

        public bool CancellationPending { get { return EndRoundWorker.CancellationPending; } }

        private void Sleep()
        {
            System.Threading.Thread.Sleep(Properties.Settings.Default.QueryServerSeconds * 1000);
        }

        GmailSettings GmailSettings;

        private void ReportEndRoundStats(Model.BaseServer server, List<Model.Player> allTimePlayers)
        {
            if (allTimePlayers != null && allTimePlayers.Count > 0)
            {
                try
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
                catch (Exception ex)
                {
                    Common.Logger.TraceWriteLine(ex.Message, ex.GetType().Name);
                }
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
            {
                Common.Logger.TraceWriteLine("New round!! Map changed."); 
                return true;
            }

            var samePlayers = newServer.Players.Join(
                oldServer.Players,
                newPlayer => new { newPlayer.Name },
                oldPlayer => new { oldPlayer.Name },
                (np, op) => new { Name = np.Name, NewScore = np.Score, OldScore = op.Score });

            int samePlayersCount = samePlayers.Count();
            if (samePlayersCount > 0)
            {
                int samePlayersWithResetedScore = samePlayers.Where(player => player.NewScore - player.OldScore < 0).Count();
                //Common.Logger.TraceWriteLine("samePlayersWithResetedScore = " + samePlayersWithResetedScore.ToString());
                double newPlayersResetedScoreRate = (double)samePlayersWithResetedScore / (double)samePlayersCount * 100D;
                Common.Logger.TraceWriteLine("New Players Reseted Score Rate = " + newPlayersResetedScoreRate.ToString("N"));
                bool newRound = newPlayersResetedScoreRate >= Properties.Settings.Default.PlayersResetedScoreRateMin;
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
