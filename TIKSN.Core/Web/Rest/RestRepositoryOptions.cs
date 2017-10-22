using System;
using System.Collections.Generic;
using System.Text;

namespace TIKSN.Web.Rest
{
    public class RestRepositoryOptions<T>
    {
        public RestRepositoryOptions()
        {
            MediaType = "application/json";
            Encoding = Encoding.UTF8;
        }

        public Guid ApiKey { get; set; }

        public string MediaType { get; set; }

        public RestAuthenticationType Authentication { get; set; }

        public Dictionary<double, string> AcceptLanguages { get; set; }

        public string ResourceTemplate { get; set; }

        public Encoding Encoding { get; set; }
    }
}