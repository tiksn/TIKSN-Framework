using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using TIKSN.Analytics.Telemetry;
using TIKSN.Data;
using TIKSN.Localization;

namespace TIKSN.Web.Rest
{
	public class RestRepository<TEntity, TIdentity> :
		IRestRepository<TEntity, TIdentity>, IRestBulkRepository<TEntity, TIdentity>,
		IRepository<TEntity>
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>
	{
		private readonly IDeserializerRestFactory _deserializerRestFactory;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IOptions<RestRepositoryOptions<TEntity>> _options;
		private readonly IStringLocalizer _stringLocalizer;
		private readonly ITraceTelemeter _traceTelemeter;
		private readonly IRestAuthenticationTokenProvider _restAuthenticationTokenProvider;
		private readonly ISerializerRestFactory _serializerRestFactory;

		public RestRepository(
			IHttpClientFactory httpClientFactory,
			ISerializerRestFactory serializerRestFactory,
			IDeserializerRestFactory deserializerRestFactory,
			IRestAuthenticationTokenProvider restAuthenticationTokenProvider,
			IOptions<RestRepositoryOptions<TEntity>> options,
			IStringLocalizer stringLocalizer,
			ITraceTelemeter traceTelemeter)
		{
			_httpClientFactory = httpClientFactory;
			_serializerRestFactory = serializerRestFactory;
			_options = options;
			_stringLocalizer = stringLocalizer;
			_traceTelemeter = traceTelemeter;
			_restAuthenticationTokenProvider = restAuthenticationTokenProvider;
			_deserializerRestFactory = deserializerRestFactory;
		}

		public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
		{
			return AddObjectAsync(entity, cancellationToken);
		}

		public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			return AddObjectAsync(entities, cancellationToken);
		}

		public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			var uriTemplate = new UriTemplate(_options.Value.ResourceTemplate);

			uriTemplate.Fill("ID", id.ToString());

			var requestUrl = uriTemplate.Compose();

			var response = await httpClient.GetAsync(requestUrl, cancellationToken);

			return await ObjectifyResponse<TEntity>(response, true);
		}

		public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			var uriTemplate = new UriTemplate(_options.Value.ResourceTemplate);

			uriTemplate.Fill("ID", entity.ID.ToString());

			var requestUrl = uriTemplate.Compose();

			var response = await httpClient.DeleteAsync(requestUrl, cancellationToken);

			response.EnsureSuccessStatusCode();
		}

		public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			await _traceTelemeter.TrackTrace(_stringLocalizer.GetRequiredString(LocalizationKeys.Key638306944));

			foreach (var entity in entities)
				await RemoveAsync(entity, cancellationToken);
		}

		public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			var uriTemplate = new UriTemplate(_options.Value.ResourceTemplate);

			uriTemplate.Fill("ID", entity.ID.ToString());

			var requestUrl = uriTemplate.Compose();

			var response = await httpClient.PutAsync(requestUrl, GetContent(entity), cancellationToken);

			response.EnsureSuccessStatusCode();
		}

		public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			var uriTemplate = new UriTemplate(_options.Value.ResourceTemplate);

			uriTemplate.Fill("ID", string.Empty);

			var requestUrl = uriTemplate.Compose();

			var response = await httpClient.PutAsync(requestUrl, GetContent(entities), cancellationToken);

			response.EnsureSuccessStatusCode();
		}

		protected async Task<IEnumerable<TEntity>> SearchAsync(IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			var uriTemplate = new UriTemplate(_options.Value.ResourceTemplate);

			foreach (var parameter in parameters)
				uriTemplate.Fill(parameter.Key, parameter.Value);

			var requestUrl = uriTemplate.Compose();

			var response = await httpClient.GetAsync(requestUrl, cancellationToken);

			return await ObjectifyResponse<IEnumerable<TEntity>>(response, false);
		}

		protected async Task<TEntity> SingleOrDefaultAsync(IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			var uriTemplate = new UriTemplate(_options.Value.ResourceTemplate);

			foreach (var parameter in parameters)
				uriTemplate.Fill(parameter.Key, parameter.Value);

			var requestUrl = uriTemplate.Compose();

			var response = await httpClient.GetAsync(requestUrl, cancellationToken);

			return await ObjectifyResponse<TEntity>(response, true);
		}

		private async Task AddObjectAsync(object requestContent, CancellationToken cancellationToken)
		{
			var httpClient = await GetHttpClientAsync();
			var uriTemplate = new UriTemplate(_options.Value.ResourceTemplate);

			uriTemplate.Fill("ID", string.Empty);

			var requestUrl = uriTemplate.Compose();

			var response = await httpClient.PostAsync(requestUrl, GetContent(requestContent), cancellationToken);

			response.EnsureSuccessStatusCode();
		}

		private HttpContent GetContent(object requestContent)
		{
			return new StringContent(_serializerRestFactory.Create(_options.Value.MediaType).Serialize(requestContent), _options.Value.Encoding, _options.Value.MediaType);
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

		private async Task<TResult> ObjectifyResponse<TResult>(HttpResponseMessage response, bool defaultIfNotFound)
		{
			if (defaultIfNotFound && response.StatusCode == System.Net.HttpStatusCode.NotFound)
				return default(TResult);

			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();

			return _deserializerRestFactory.Create(_options.Value.MediaType).Deserialize<TResult>(content);
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
					throw new NotSupportedException($"Authentication type '{_options.Value.Authentication}' is not supported.");
			}

			var authenticationToken = await _restAuthenticationTokenProvider.GetAuthenticationToken(_options.Value.ApiKey);

			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationSchema, authenticationToken);
		}
	}
}