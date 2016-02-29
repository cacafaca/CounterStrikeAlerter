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
        public MainViewModel()
        {
            _ServerMonitor = new ServerMonitor(Properties.Settings.Default.ServerAddressAndPort, Properties.Settings.Default.ServerReadInterval);
            _GmailSettings = new CSA.ViewModel.GmailSettings();
        }

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

        private CSA.ViewModel.GmailSettings _GmailSettings;

        public CSA.ViewModel.GmailSettings GmailSettings
        {
            get { return _GmailSettings; }
            set
            {
                _GmailSettings = value;
                RaisePropertyChanged(nameof(GmailSettings));
            }
        }

    }
}
