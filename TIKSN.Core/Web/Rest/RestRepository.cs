using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Web.Rest
{
	public class RestRepository<T> : IRepository<T>, IRestRepository<T>
	{
		private readonly IRestAuthenticationTokenProvider _restAuthenticationTokenProvider;
		private readonly IOptions<RestRepositoryOptions<T>> _options;
		private readonly ISerializerRestFactory _serializerRestFactory;
		private readonly IHttpClientFactory _httpClientFactory;

		public RestRepository(
			IHttpClientFactory httpClientFactory,
			ISerializerRestFactory serializerRestFactory,
			IRestAuthenticationTokenProvider restAuthenticationTokenProvider,
			IOptions<RestRepositoryOptions<T>> options)
		{
			_httpClientFactory = httpClientFactory;
			_serializerRestFactory = serializerRestFactory;
			_options = options;
			_restAuthenticationTokenProvider = restAuthenticationTokenProvider;
		}

		public Task AddAsync(T entity, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<T> GetAsync(string id, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			throw new NotImplementedException();
		}

		public Task RemoveAsync(T entity, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(T entity, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		private async Task<HttpClient> GetHttpClientAsync()
		{
			var httpClient = await _httpClientFactory.Create(_options.Value.ApiKey);

			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_options.Value.MediaType));

			if (_options.Value.AcceptLanguages != null)
				foreach (var acceptLanguage in _options.Value.AcceptLanguages)
					httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(acceptLanguage.Value, acceptLanguage.Key));

			await SetAuthenticationHeader(httpClient);

			return httpClient;
		}

		private async Task SetAuthenticationHeader(HttpClient httpClient)
		{
			var authenticationSchema = string.Empty;

			switch (_options.Value.Authentication)
			{
				case RestAuthenticationType.None:
					return;

				case RestAuthenticationType.Basic:
					authenticationSchema = "Basic";
					break;

				case RestAuthenticationType.Bearer:
					authenticationSchema = "Bearer";
					break;

				default:
					throw new NotSupportedException($"Authentication type '{_options.Value.Authentication.ToString()}' is not supported.");
			}

			var authenticationToken = await _restAuthenticationTokenProvider.GetAuthenticationToken(_options.Value.ApiKey);

			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationSchema, authenticationToken);
		}
	}
}