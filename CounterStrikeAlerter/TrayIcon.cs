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
            AddContextmenu();
            NotifyIcon.Click += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            if (Click != null)
                Click(this, new EventArgs());
        }

        public event EventHandler Click;

        public void Dispose()
        {
            NotifyIcon.Icon = null;
            NotifyIcon.ContextMenuStrip.Dispose();
        }

        private void AddContextmenu()
        {
            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();

            NotifyIcon.ContextMenuStrip.Items.Add("GMail user&pass", null,
                delegate (object sender, EventArgs e)
                {
                    if (GmailUserPass != null)
                        GmailUserPass(sender, e);
                });

            NotifyIcon.ContextMenuStrip.Items.Add("Exit", null,
                delegate (object sender, EventArgs e)
                {
                    if (ExitHandler != null)
                        ExitHandler(sender, e);
                });
        }

        public event EventHandler ExitHandler;
        public event EventHandler GmailUserPass;
    }
}
