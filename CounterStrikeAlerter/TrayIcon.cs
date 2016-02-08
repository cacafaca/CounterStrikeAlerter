using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CounterStrikeAlerter
{
    public class TrayIcon : IDisposable
    {
        NotifyIcon NotifyIcon;
        public TrayIcon()
        {
            NotifyIcon = new NotifyIcon();
            NotifyIcon.Icon = Properties.Resources.cstrike;
            NotifyIcon.Visible = true;
        }

        public void Dispose()
        {
            NotifyIcon.Icon = null;
        }
    }
}
