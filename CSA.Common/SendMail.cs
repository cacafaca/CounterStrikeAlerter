﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Common
{
    public class SendMail
    {
        public SendMail(string smtpServer, int smtpPort, string from, ICredentialsByHost credentials)
        {
            Client = new SmtpClient();
            Client.Host = smtpServer;
            Client.Port = smtpPort; // Default for GMail is 587.
            Client.DeliveryMethod = SmtpDeliveryMethod.Network;
            Client.UseDefaultCredentials = false;
            Client.EnableSsl = true;
            Client.Timeout = 10000;
            Client.Credentials = credentials;
            From = from;
        }
        string From;
        SmtpClient Client;

        public void Send(string to, string subject, string body)
        {
            MailMessage message = new MailMessage(From, to, subject, body);
            message.BodyEncoding = UTF8Encoding.UTF8;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            Client.Send(message);
        }
    }
}
