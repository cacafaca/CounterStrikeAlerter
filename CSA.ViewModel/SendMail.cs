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

        public SendMail(string user, string encriptedPassword)
        {
            User = user;
            EncryptedPassword = encriptedPassword;
        }

        private string User;
        private string Password;
        private string EncryptedPassword;


        private void ReadUserAndPasswordFromRegistry()
        {
            System.Diagnostics.Debug.WriteLine("Read registry.");
            RegistrySettings regSet = new RegistrySettings();
            User = regSet.GMailUser;
            Password = regSet.GMailPass;
        }

        public void Send(string to, string subject, string body)
        {
            System.Net.NetworkCredential credential;
            if (string.IsNullOrEmpty(EncryptedPassword))
            {
                ReadUserAndPasswordFromRegistry();
                credential = new System.Net.NetworkCredential(User, Password);
            }
            else
                credential = new System.Net.NetworkCredential(User, EncryptedPassword.Decrypt());

            CSA.Common.SendMail sm = new Common.SendMail("smtp.gmail.com", 587, credential);
            sm.Send(to, subject, body);
        }
    }
}
