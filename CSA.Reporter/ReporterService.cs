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
            Common.Logger.TraceWriteLine("After InitializeComponent().");
            System.Threading.Thread.Sleep(10000);
            Common.Logger.TraceWriteLine("new EndRoundReporter();.");
            EndRoundReporter = new EndRoundReporter();
        }

        protected override void OnStart(string[] args)
        {
            //System.Threading.Thread.Sleep(30000);
            //if(EndRoundReporter == null)
                
            EndRoundReporter.Start();
        }

        EndRoundReporter EndRoundReporter;

        protected override void OnStop()
        {
            EndRoundReporter.Stop();
        }
    }
}
