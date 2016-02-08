﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSA.ViewModel;
using System.Windows.Interop;
using System.Windows.Forms;

namespace CounterStrikeAlerter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new ServerMonitor(new CSA.ViewModel.Server("193.104.68.49", 27040));
            InitializeComponent();
            CustomInitialization();

            StartMonitoring();
        }

        private void CustomInitialization()
        {
            SetWindowLocationAndSize();
            //WindowState = WindowState.Minimized;
            //ShowInTaskbar = false;
            TrayIcon = new TrayIcon();
            Icon = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.cstrike.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void SetWindowLocationAndSize()
        {
            Width = 400;
            Height = 200;
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 10;
            Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
        }

        TrayIcon TrayIcon;

        private void StartMonitoring()
        {
            ((ServerMonitor)DataContext).StartMonitoring();
        }

        private void notificationLabel_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TrayIcon.Dispose();
        }
    }
}