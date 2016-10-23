using System;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
    public interface IRestRequesterConfiguration
    {
        Task<Uri> GetBaseUrl(string apiName);
    }
}