using System;
using System.Threading.Tasks;
using TIKSN.Web.Rest;

namespace Console_Client.Rest
{
	public class RestAuthenticationTokenProvider : IRestAuthenticationTokenProvider
	{
		public Task<string> GetAuthenticationToken(Guid apiKey)
		{
			throw new NotImplementedException();
		}
	}
}
