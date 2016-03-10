using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Reporter
{
    public class GmailSettings : ConfigurationSection
    {
        [ConfigurationProperty("GmailUser",
        DefaultValue = "Replace Gmail email address here.",
        IsRequired = true,
        IsKey = true)]
        public string GmailUser
        {
            get
            {
                return (string)this[nameof(GmailUser)];
            }
            set
            {
                this[nameof(GmailUser)] = value;
            }
        }

        [ConfigurationProperty("GmailEncryptedPassword",
        DefaultValue = "Replace Gmail encrypted password here.",
        IsRequired = true,
        IsKey = true)]
        public string GmailEncryptedPassword
        {
            get
            {
                return (string)this[nameof(GmailEncryptedPassword)];
            }
            set
            {
                this[nameof(GmailEncryptedPassword)] = value;
            }
        }

        [ConfigurationProperty("SendToAddresses",
        DefaultValue = "user1@domain.com;user2@domain.com",
        IsRequired = true,
        IsKey = true)]
        public string SendToAddresses
        {
            get
            {
                return (string)this[nameof(SendToAddresses)];
            }
            set
            {
                this[nameof(SendToAddresses)] = value;
            }
        }

        public GmailSettings()
        {
            ConfigMap = new ExeConfigurationFileMap();
            ConfigMap.ExeConfigFilename = string.Format("{0}\\{1}.config", CSA.Common.Util.GetAppPath(), GetType().Name);
        }

        ExeConfigurationFileMap ConfigMap;

        public void Load()
        {
            Common.Logger.TraceWriteLine(this.GetType().FullName + "> ExeConfigFilename: " + ConfigMap.ExeConfigFilename);

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ConfigMap, ConfigurationUserLevel.None);
            try
            {
                GmailSettings gmailSettings = (GmailSettings)config.GetSection("GmailSettings");
                Common.Logger.TraceWriteLine(this.GetType().FullName + "> GetSection: GmailUser=" + gmailSettings.GmailUser + "GmailPass=" + gmailSettings.GmailEncryptedPassword);
                GmailUser = gmailSettings.GmailUser;
                GmailEncryptedPassword = gmailSettings.GmailEncryptedPassword;
                SendToAddresses = gmailSettings.SendToAddresses;
            }
            catch (Exception ex)
            {
                Common.Logger.TraceWriteLine(ex.Message);
            }
        }

        private void Save()
        {
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ConfigMap, ConfigurationUserLevel.None);
            config.Sections.Remove(GetType().Name);
            config.Sections.Add(GetType().Name, this);
            config.Save(ConfigurationSaveMode.Full, true);
        }

        public List<string> SendToAddressesList()
        {
            return SendToAddresses.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

    }
}
