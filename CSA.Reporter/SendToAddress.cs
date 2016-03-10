using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Reporter
{
    public class SendToAddress : ConfigurationElement
    {
        private static readonly ConfigurationPropertyCollection PropertyCollection = new ConfigurationPropertyCollection();
        private const string SendToAddressName = "SendToAddress";
        private const string DefaultSendToAddress = "email1@domain.com";

        public SendToAddress()
        {
            PropertyCollection.Add(
                new ConfigurationProperty(SendToAddressName, SendToAddressName.GetType(), DefaultSendToAddress, 
                    ConfigurationPropertyOptions.IsRequired));
        }
    }
}
