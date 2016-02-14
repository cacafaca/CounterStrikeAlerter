using System;
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
using System.ComponentModel;
using System.Windows.Threading;

namespace CounterStrikeAlerter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CustomInitialization();

            StartMonitoring();
        }

        private void CustomInitialization()
        {
            ((MainViewModel)DataContext).ServerMonitor = new ServerMonitor(Properties.Settings.Default.ServerAddressAndPort, Properties.Settings.Default.ServerReadInterval);

            Hide();
            SetWindowLocationAndSize();
            ShowInTaskbar = false;
            TrayIcon = new TrayIcon();
            Icon = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.cstrike.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            HideTimer = new System.Timers.Timer(Properties.Settings.Default.BaloonHideInterval);
            HideTimer.Enabled = false;
            HideTimer.AutoReset = false;

            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(ContentControl.ContentProperty, typeof(Label));
            if (dpd != null)
            {
                dpd.AddValueChanged(notificationLabel, delegate
                {
                    ShowFormForAPeriodOfTime();
                });
            }

            HideTimer.Elapsed += Timer_Elapsed;
            TrayIcon.ExitHandler += TrayIcon_ExitHandler;
            TrayIcon.Click += TrayIcon_Click;
        }

        private void ShowFormForAPeriodOfTime()
        {
            if (Visibility == Visibility.Hidden)
                Show();
            Dispatcher.Invoke(delegate () { }, DispatcherPriority.Render);
            Topmost = true;
            HideTimer.Enabled = true;
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            ShowFormForAPeriodOfTime();
        }

        System.Timers.Timer HideTimer;

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Hide();
            }));
        }

        private void TrayIcon_ExitHandler(object sender, EventArgs e)
        {
            TrayIcon.Dispose();
            TrayIcon = null;    // Have purpose of detecting a wish to exit application.
            Close();
        }

        private void SetWindowLocationAndSize()
        {
            Width = 400;
            Height = 200;
            Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - Height - 10;
            Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
        }

        TrayIcon TrayIcon;

        private void StartMonitoring()
        {
            ((MainViewModel)DataContext).ServerMonitor.StartMonitoring();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = TrayIcon != null;
        }

    }
}
