using System;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
	public interface IRestAuthenticationTokenProvider
	{
		Task<string> GetAuthenticationToken(Guid apiKey);
	}
}