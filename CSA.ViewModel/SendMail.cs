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
        public SendMail(string gmailUser, string gmailEncriptedPassword, string gmailSmtpAddress, int gmailSmtpPort)
        {
            GmailUser = gmailUser;
            GmailEncryptedPassword = gmailEncriptedPassword;
            GmailSmtpAddress = gmailSmtpAddress;
            GmailSmtpPort = gmailSmtpPort;
        }

        private string GmailUser;
        private string GmailEncryptedPassword;
        private string GmailSmtpAddress;
        private int GmailSmtpPort;

        public void Send(string to, string subject, string body)
        {
            System.Net.NetworkCredential credential;
            System.Security.SecureString secStr = new System.Security.SecureString();
            foreach (var c in GmailEncryptedPassword.Decrypt())
                secStr.AppendChar(c);
            credential = new System.Net.NetworkCredential(GmailUser, secStr);

            CSA.Common.SendMail sm = new Common.SendMail(GmailSmtpAddress, GmailSmtpPort, credential);
            sm.Send(to, subject, body);
        }
    }
}
