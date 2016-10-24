using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
    public interface IRestRequesterConfiguration
    {
        Task<Uri> GetBaseUrl(string apiName);

        Task<IEnumerable<KeyValuePair<string, string>>> GetDefaultHeaders(string apiName);

        Task<IEnumerable<KeyValuePair<string, string>>> GetResourceParameters(string apiName);
    }
}