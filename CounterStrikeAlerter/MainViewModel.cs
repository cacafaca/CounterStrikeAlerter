using CSA.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeAlerter
{
    public class MainViewModel : BaseViewModel
    {
        private ServerMonitor _ServerMonitor;
        public ServerMonitor ServerMonitor
        {
            get { return _ServerMonitor; }
            set
            {
                _ServerMonitor = value;
                RaisePropertyChanged(nameof(ServerMonitor));
            }
        }
    }
}
