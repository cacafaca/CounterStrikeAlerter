using CSA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class SendMail
    {
        public SendMail() :
            this(null, null)
        {
        }

        public SendMail(string gmailUser, string gmailEncriptedPassword)
        {
            GmailUser = gmailUser;
            GmailEncryptedPassword = gmailEncriptedPassword;
        }

        private string GmailUser;
        private string GmailEncryptedPassword;


        private void ReadUserAndPasswordFromRegistry()
        {
            Common.Logger.TraceWriteLine("Read registry.");
            GmailRegistrySettings regSet = new GmailRegistrySettings();
            GmailUser = regSet.GMailUser;
            GmailEncryptedPassword = regSet.GMailPass;
        }

        public void Send(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(GmailUser) || string.IsNullOrEmpty(GmailEncryptedPassword))
                ReadUserAndPasswordFromRegistry();
            System.Net.NetworkCredential credential;
            credential = new System.Net.NetworkCredential(GmailUser, GmailEncryptedPassword.Decrypt());

            CSA.Common.SendMail sm = new Common.SendMail("smtp.gmail.com", 587, credential);
            sm.Send(to, subject, body);
        }
    }
}
