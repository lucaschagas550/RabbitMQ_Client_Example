using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Extensions
{
    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public RabbitMQConfiguration() { }

        public RabbitMQConfiguration(string hostName, string userName, string password)
         {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }
    }
}