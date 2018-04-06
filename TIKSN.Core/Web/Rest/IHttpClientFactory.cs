using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
    public interface IHttpClientFactory
    {
        Task<HttpClient> Create(Guid apiKey);
    }
}