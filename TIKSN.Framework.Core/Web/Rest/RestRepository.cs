using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TIKSN.Data;

namespace TIKSN.Web.Rest;

public class RestRepository<TEntity, TIdentity> :
    IRestRepository<TEntity, TIdentity>, IRestBulkRepository<TEntity, TIdentity>,
    IRepository<TEntity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IDeserializerRestFactory deserializerRestFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<RestRepository<TEntity, TIdentity>> logger;
    private readonly IOptions<RestRepositoryOptions<TEntity>> options;
    private readonly IRestAuthenticationTokenProvider restAuthenticationTokenProvider;
    private readonly ISerializerRestFactory serializerRestFactory;

    public RestRepository(
        IHttpClientFactory httpClientFactory,
        ISerializerRestFactory serializerRestFactory,
        IDeserializerRestFactory deserializerRestFactory,
        IRestAuthenticationTokenProvider restAuthenticationTokenProvider,
        IOptions<RestRepositoryOptions<TEntity>> options,
        ILogger<RestRepository<TEntity, TIdentity>> logger)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        this.serializerRestFactory = serializerRestFactory ?? throw new ArgumentNullException(nameof(serializerRestFactory));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.restAuthenticationTokenProvider = restAuthenticationTokenProvider ?? throw new ArgumentNullException(nameof(restAuthenticationTokenProvider));
        this.deserializerRestFactory = deserializerRestFactory ?? throw new ArgumentNullException(nameof(deserializerRestFactory));
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        this.AddObjectAsync(entity, cancellationToken);

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        this.AddObjectAsync(entities, cancellationToken);

    public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var httpClient = await this.GetHttpClientAsync().ConfigureAwait(false);
        var uriTemplate = new UriTemplate(this.options.Value.ResourceTemplate);

        uriTemplate.Fill("ID", id.ToString());

        var requestUrl = uriTemplate.Compose();

        var response = await httpClient.GetAsync(requestUrl, cancellationToken).ConfigureAwait(false);

        return await this.ObjectifyResponseAsync<TEntity>(response, defaultIfNotFound: true).ConfigureAwait(false);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var httpClient = await this.GetHttpClientAsync().ConfigureAwait(false);
        var uriTemplate = new UriTemplate(this.options.Value.ResourceTemplate);

        uriTemplate.Fill("ID", entity.ID.ToString());

        var requestUrl = uriTemplate.Compose();

        var response = await httpClient.DeleteAsync(requestUrl, cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        this.logger.LogWarning(1975130298, "TIKSN.Web.Rest.RestRepository.RemoveRangeAsync method is not advised to use");

        foreach (var entity in entities)
        {
            await this.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var httpClient = await this.GetHttpClientAsync().ConfigureAwait(false);
        var uriTemplate = new UriTemplate(this.options.Value.ResourceTemplate);

        uriTemplate.Fill("ID", entity.ID.ToString());

        var requestUrl = uriTemplate.Compose();

        using var content = this.GetContent(entity);
        var response = await httpClient.PutAsync(requestUrl, content, cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        var httpClient = await this.GetHttpClientAsync().ConfigureAwait(false);
        var uriTemplate = new UriTemplate(this.options.Value.ResourceTemplate);

        uriTemplate.Fill("ID", string.Empty);

        var requestUrl = uriTemplate.Compose();

        using var content = this.GetContent(entities);
        var response = await httpClient.PutAsync(requestUrl, content, cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    protected async Task<IEnumerable<TEntity>> SearchAsync(
        IEnumerable<KeyValuePair<string, string>> parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var httpClient = await this.GetHttpClientAsync().ConfigureAwait(false);
        var uriTemplate = new UriTemplate(this.options.Value.ResourceTemplate);

        foreach (var parameter in parameters)
        {
            uriTemplate.Fill(parameter.Key, parameter.Value);
        }

        var requestUrl = uriTemplate.Compose();

        var response = await httpClient.GetAsync(requestUrl, cancellationToken).ConfigureAwait(false);

        return await this.ObjectifyResponseAsync<IEnumerable<TEntity>>(response, defaultIfNotFound: false).ConfigureAwait(false);
    }

    protected async Task<TEntity> SingleOrDefaultAsync(
        IEnumerable<KeyValuePair<string, string>> parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var httpClient = await this.GetHttpClientAsync().ConfigureAwait(false);
        var uriTemplate = new UriTemplate(this.options.Value.ResourceTemplate);

        foreach (var parameter in parameters)
        {
            uriTemplate.Fill(parameter.Key, parameter.Value);
        }

        var requestUrl = uriTemplate.Compose();

        var response = await httpClient.GetAsync(requestUrl, cancellationToken).ConfigureAwait(false);

        return await this.ObjectifyResponseAsync<TEntity>(response, defaultIfNotFound: true).ConfigureAwait(false);
    }

    private async Task AddObjectAsync(object requestContent, CancellationToken cancellationToken)
    {
        var httpClient = await this.GetHttpClientAsync().ConfigureAwait(false);
        var uriTemplate = new UriTemplate(this.options.Value.ResourceTemplate);

        uriTemplate.Fill("ID", string.Empty);

        var requestUrl = uriTemplate.Compose();

        using var content = this.GetContent(requestContent);
        var response = await httpClient.PostAsync(requestUrl, content, cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    private StringContent GetContent(object requestContent) => new(
        this.serializerRestFactory.Create(this.options.Value.MediaType).Serialize(requestContent),
        this.options.Value.Encoding, this.options.Value.MediaType);

    private async Task<HttpClient> GetHttpClientAsync()
    {
        var httpClient = this.httpClientFactory.CreateClient(this.options.Value.ApiKey);

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(this.options.Value.MediaType));

        if (this.options.Value.AcceptLanguages != null)
        {
            foreach (var acceptLanguage in this.options.Value.AcceptLanguages)
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(
                    new StringWithQualityHeaderValue(acceptLanguage.Value, acceptLanguage.Key));
            }
        }

        await this.SetAuthenticationHeaderAsync(httpClient).ConfigureAwait(false);

        return httpClient;
    }

    private async Task<TResult> ObjectifyResponseAsync<TResult>(HttpResponseMessage response, bool defaultIfNotFound)
    {
        if (defaultIfNotFound && response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        _ = response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return this.deserializerRestFactory.Create(this.options.Value.MediaType).Deserialize<TResult>(content);
    }

    private async Task SetAuthenticationHeaderAsync(HttpClient httpClient)
    {
        string authenticationSchema;
        switch (this.options.Value.Authentication)
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
                throw new NotSupportedException(
                    $"Authentication type '{this.options.Value.Authentication}' is not supported.");
        }

        var authenticationToken =
            await this.restAuthenticationTokenProvider.GetAuthenticationTokenAsync(this.options.Value.ApiKey).ConfigureAwait(false);

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(authenticationSchema, authenticationToken);
    }
}
