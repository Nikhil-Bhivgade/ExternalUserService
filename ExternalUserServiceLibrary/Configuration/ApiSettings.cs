using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalUserServiceLibrary.Configuration
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int CacheDurationSeconds { get; set; }
    }
}
