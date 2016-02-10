﻿using System;
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
            NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClick != null)
                DoubleClick(this, new EventArgs());
        }

        public event EventHandler DoubleClick;

        public void Dispose()
        {
            NotifyIcon.Icon = null;
            NotifyIcon.ContextMenuStrip.Dispose();
        }

        private void AddContextmenu()
        {
            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.Add("Exit", null,
                delegate (object sender, EventArgs e)
                {
                    if (ExitHandler != null)
                        ExitHandler(sender, e);
                });
        }

        public event EventHandler ExitHandler;
    }
}
