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
using System.Windows.Shapes;
using CSA.Common;

namespace CounterStrikeAlerter
{
    /// <summary>
    /// Interaction logic for GmailSettings.xaml
    /// </summary>
    public partial class GmailSettings : Window
    {
        public GmailSettings()
        {
            InitializeComponent();
        }

        private void sendMailActive_Checked(object sender, RoutedEventArgs e)
        {
            if (sendMailActive.IsChecked == null)
                sendMailActive.IsChecked = false;
            ToggleGmailSettings((bool)sendMailActive.IsChecked);
        }

        private void ToggleGmailSettings(bool enable)
        {
            if (fromEmailAddress != null)
            {
                fromEmailAddress.IsEnabled = enable;
                toEmailAddresses.IsEnabled = enable;
                password.IsEnabled = enable;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleGmailSettings((bool)sendMailActive.IsChecked);
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            MainViewModel.GmailSettings.GMailPass = ((PasswordBox)sender).Password.Encrypt();
        }

        private MainViewModel MainViewModel
        {
            get
            {
                return (MainViewModel)DataContext;
            }
        }
    }
}