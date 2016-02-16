using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class SendMail
    {
        public SendMail(string user, string pass)
        {
            User = user;
            Pass = pass;
        }

        private string User;
        private string Pass;

        public void Send(string to, string subject, string body)
        {
            var credential = new System.Net.NetworkCredential(User, Pass);
            CSA.Common.SendMail sm = new Common.SendMail("smtp.gmail.com", 25, credential);
            sm.Send(to, subject, body);
        }
    }
}
