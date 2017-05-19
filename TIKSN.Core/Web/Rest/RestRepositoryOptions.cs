using System;
using System.Collections.Generic;

namespace TIKSN.Web.Rest
{
	public class RestRepositoryOptions<T>
	{
		public RestRepositoryOptions()
		{
			MediaType = "application/json";
		}

		public Guid ApiKey { get; set; }

		public string MediaType { get; set; }

		public RestAuthenticationType Authentication { get; set; }

		public Dictionary<double, string> AcceptLanguages { get; set; }
	}
}
