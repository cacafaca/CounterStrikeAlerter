using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeAlerter
{
    public class LullabyWorker : BackgroundWorker
    {
        public LullabyWorker()
        {
            DoWork += LullabyWorker_DoWork;
        }

        private void LullabyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(15000);
        }
    }
}
