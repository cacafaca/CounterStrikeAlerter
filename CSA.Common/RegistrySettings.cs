using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Common
{
    public class RegistrySettings
    {
        public RegistrySettings()
        {
            csaKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Nemanja\\CounterStrikeAlerter");
        }

        Microsoft.Win32.RegistryKey csaKey;

        public string GMailUser
        {
            get { return (string)csaKey.GetValue(nameof(GMailUser)); }
            set { csaKey.SetValue(nameof(GMailUser), value); }
        }

        public string GMailPass
        {
            get
            {
                string encryptedPass = (string)csaKey.GetValue(nameof(GMailPass));
                if (!string.IsNullOrEmpty(encryptedPass))
                    return (encryptedPass.Decrypt());
                else
                    return null;
            }
            set { csaKey.SetValue(nameof(GMailPass), value.Encrypt()); }
        }

        public string Addresses
        {
            get
            {
                string res = string.Empty;
                object val = csaKey.GetValue(nameof(Addresses));
                if (val != null)
                    res = (string)val;
                return res;
            }
            set { csaKey.SetValue(nameof(Addresses), value); }
        }

        public List<string> AddressesList()
        {
            return Addresses.Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public bool SendEmailActive
        {
            get
            {
                var value = csaKey.GetValue(nameof(SendEmailActive));
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
                csaKey.SetValue(nameof(SendEmailActive), value);
            }
        }

    }
}
