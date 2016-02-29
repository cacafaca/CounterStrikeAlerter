using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class GmailSettings : BaseViewModel, INotifyPropertyChanged
    {
        protected Common.RegistrySettings RegistrySettinga;
        public GmailSettings()
        {
            RegistrySettinga = new Common.RegistrySettings();
        }

        public string GMailUser
        {
            get { return RegistrySettinga.GMailUser; }
            set
            {
                RegistrySettinga.GMailUser = value;
                RaisePropertyChanged(nameof(GMailUser));
            }
        }

        public string GMailPass
        {
            get { return RegistrySettinga.GMailPass; }
            set
            {
                RegistrySettinga.GMailPass = value;
                RaisePropertyChanged(nameof(GMailPass));
            }
        }

        /// <summary>
        /// Separate email addresses with semicolon.
        /// </summary>
        public string Addresses
        {
            get { return RegistrySettinga.Addresses; }
            set
            {
                RegistrySettinga.Addresses = value;
                RaisePropertyChanged(nameof(Addresses));
            }
        }

        public bool SendEmailActive
        {
            get { return RegistrySettinga.SendEmailActive; }
            set
            {
                RegistrySettinga.SendEmailActive = value;
                RaisePropertyChanged(nameof(SendEmailActive));
            }
        }

        public void Refresh()
        {
            RaisePropertyChanged(GMailUser);
        }
    }
}
