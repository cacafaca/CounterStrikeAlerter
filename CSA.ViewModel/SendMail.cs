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
        public SendMail()
        {
        }

        private string User;
        private string Password;

        private void ReadUserAndPasswordFromRegistry()
        {
            RegistrySettings regSet = new RegistrySettings();
            User = regSet.GMailUser;
            Password = regSet.GMailPass;
        }

        public void Send(string to, string subject, string body)
        {
            ReadUserAndPasswordFromRegistry();
            var credential = new System.Net.NetworkCredential(User, Password);
            CSA.Common.SendMail sm = new Common.SendMail("smtp.gmail.com", 587, credential);
            sm.Send(to, subject, body);
        }
    }
}
