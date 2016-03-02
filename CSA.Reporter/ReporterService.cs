using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Reporter
{
    public partial class ReporterService : ServiceBase
    {
        public ReporterService()
        {
            InitializeComponent();
            EndRoundReporter = new EndRoundReporter();
        }

        protected override void OnStart(string[] args)
        {
            System.Threading.Thread.Sleep(1000);
            EndRoundReporter.Start();
        }

        EndRoundReporter EndRoundReporter;

        protected override void OnStop()
        {
            EndRoundReporter.Stop();
        }
    }
}
