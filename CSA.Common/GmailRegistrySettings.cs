using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Common
{
    public class GmailRegistrySettings
    {
        public GmailRegistrySettings()
        {
            CsaKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Nemanja\\CounterStrikeAlerter");
        }

        Microsoft.Win32.RegistryKey CsaKey;

        public string GMailUser
        {
            get
            {
                return (string)CsaKey.GetValue(nameof(GMailUser));
            }
            set
            {
                CsaKey.SetValue(nameof(GMailUser), value);
            }
        }

        /// <summary>
        /// Encrypted password.
        /// </summary>
        public string GMailPass
        {
            get
            {
                return (string)CsaKey.GetValue(nameof(GMailPass)); ;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    CsaKey.SetValue(nameof(GMailPass), value);
                }
            }
        }

        public string Addresses
        {
            get
            {
                string res = string.Empty;
                object val = CsaKey.GetValue(nameof(Addresses));
                if (val != null)
                    res = (string)val;
                return res;
            }
            set
            {
                CsaKey.SetValue(nameof(Addresses), value);
            }
        }

        public List<string> AddressesList()
        {
            return Addresses.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public bool SendEmailActive
        {
            get
            {
                var value = CsaKey.GetValue(nameof(SendEmailActive));
                bool res = false;
                if (value != null)
                {
                    bool.TryParse((string)value, out res);
                    return res;
                }
                else
                    return false;
            }
            set
            {
                CsaKey.SetValue(nameof(SendEmailActive), value);
            }
        }

        public string GMailSmtpAddress
        {
            get
            {
                string smtpAddress = (string)CsaKey.GetValue(nameof(GMailSmtpAddress));
                if (string.IsNullOrWhiteSpace(smtpAddress))
                    smtpAddress = "smtp.gmail.com";
                return smtpAddress;
            }
            set
            {
                CsaKey.SetValue(nameof(GMailSmtpAddress), value);
            }
        }

        public int GMailSmtpPort
        {
            get
            {
                string portStr = (string)CsaKey.GetValue(nameof(GMailSmtpPort));
                int port;
                if (!int.TryParse(portStr, out port))
                    port = 587;
                return port;
            }
            set
            {
                CsaKey.SetValue(nameof(GMailSmtpPort), value);
            }
        }

    }
}
