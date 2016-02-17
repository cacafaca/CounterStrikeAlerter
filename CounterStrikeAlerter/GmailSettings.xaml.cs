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

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToRegistry();
            Close();
        }

        private void SaveToRegistry()
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Nemanja\\CounterStrikeAlerter");
            key.SetValue("GMailUser", userTextBox.Text);
            key.SetValue("GMailPass", passwordTextBox.Text);
            key.Close();
        }
    }
}
