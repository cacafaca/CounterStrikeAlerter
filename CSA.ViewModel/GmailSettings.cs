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

        private string _GMailPass;

        public string GMailPass
        {
            get { return _GMailPass; }
            set
            {
                _GMailPass = value;
                RaisePropertyChanged(nameof(GMailPass));
            }
        }

        private string _Address;

        /// <summary>
        /// Separate email addresses with semicolon.
        /// </summary>
        public string Addresses
        {
            get { return _Address; }
            set
            {
                _Address = value;
                RaisePropertyChanged(nameof(Addresses));
            }
        }

        private bool _SendEmailActive;

        public bool SendEmailActive
        {
            get { return _SendEmailActive; }
            set
            {
                _SendEmailActive = value;
                RaisePropertyChanged(nameof(SendEmailActive));
            }
        }

        public void Refresh()
        {
            RaisePropertyChanged(GMailUser);
        }
    }
}
