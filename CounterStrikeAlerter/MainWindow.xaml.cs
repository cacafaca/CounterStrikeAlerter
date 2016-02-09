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
            //DataContext = new ServerMonitor("193.104.68.49", 27040);
            DataContext = new ServerMonitor(Properties.Settings.Default.ServerAddress, Properties.Settings.Default.ServerPort);
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

            PutToSleep += MainWindow_PutToSleep;


            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(ContentControl.ContentProperty, typeof(Label));
            if (dpd != null)
            {
                dpd.AddValueChanged(notificationLabel, delegate
                {
                    if (Visibility == Visibility.Hidden)
                        Show();
                    Dispatcher.Invoke(delegate () { }, DispatcherPriority.Render);
                    Topmost = true;
                    var timer = new System.Timers.Timer(15000);
                    timer.Enabled = true;
                    timer.Elapsed += Timer_Elapsed;
                });
            }

            TrayIcon.ExitHandler += TrayIcon_ExitHandler;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Hide();
            }));

        }

        private void MainWindow_PutToSleep(object sender, EventArgs e)
        {
            Hide();
        }

        private event EventHandler PutToSleep;

        private void TrayIcon_ExitHandler(object sender, EventArgs e)
        {
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
            ((ServerMonitor)DataContext).StartMonitoring();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TrayIcon.Dispose();
        }

    }
}
