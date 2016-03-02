using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

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
            LoadGmailSettingsFromConfig();

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

        public bool CancellationPending { get { return EndRoundWorker.CancellationPending; } }

        private void Sleep()
        {
            System.Threading.Thread.Sleep(Properties.Settings.Default.QueryServerSeconds * 1000);
        }


        private string GmailUser;
        private string EncryptedPassword;

        private void LoadGmailSettingsFromConfig()
        {
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = @".\EmailSettings.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

        }

        private void ReportEndRoundStats(Model.BaseServer server)
        {
            CSA.ViewModel.SendMail sendMail = new ViewModel.SendMail();
            string to = "nemanja.simovic@brezna.info";
            string subject = string.Format("Statistic for server {0} at {1} on map {2}", server.Name, server.AddressAndPort(), server.Map);
            string body = GenerateBody(server);
            sendMail.Send(to, subject, body);
        }

        private string GenerateBody(Model.BaseServer server)
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
            foreach (var player in server.Players.OrderByDescending(player => player.Score))
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
                return samePlayersWithResetedScore / samePlayersCount * 100 > 70;
            }
            else
                return false;
        }

        public void Stop()
        {
            CanWork = false;
        }
    }
}
