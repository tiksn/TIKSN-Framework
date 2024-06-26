using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace TIKSN.Web.Rest;

public class RestRequester : IRestRequester
{
    private readonly IDeserializerRestFactory deserializerRestFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IRestAuthenticationTokenProvider restAuthenticationTokenProvider;
    private readonly ISerializerRestFactory serializerRestFactory;

    public RestRequester(
        IHttpClientFactory httpClientFactory,
        ISerializerRestFactory serializerRestFactory,
        IDeserializerRestFactory deserializerRestFactory,
        IRestAuthenticationTokenProvider restAuthenticationTokenProvider)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        this.serializerRestFactory =
            serializerRestFactory ?? throw new ArgumentNullException(nameof(serializerRestFactory));
        this.deserializerRestFactory = deserializerRestFactory ??
                                        throw new ArgumentNullException(nameof(deserializerRestFactory));
        this.restAuthenticationTokenProvider = restAuthenticationTokenProvider ??
                                                throw new ArgumentNullException(
                                                    nameof(restAuthenticationTokenProvider));
    }

#pragma warning disable MA0051 // Method is too long

    public async Task<TResult?> RequestAsync<TResult, TRequest>(
        TRequest request,
        CancellationToken cancellationToken)
#pragma warning restore MA0051 // Method is too long
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType().GetTypeInfo();

        var restEndpointAttribute = requestType.GetCustomAttribute<RestEndpointAttribute>() ?? throw new NotSupportedException("Requested Type has to have RestEndpointAttribute.");

        var resourceLocation = restEndpointAttribute.ResourceTemplate;
        var requestUrl = new Uri(resourceLocation, UriKind.Relative);

        var httpClient = this.httpClientFactory.CreateClient(restEndpointAttribute.ApiKey);

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(restEndpointAttribute.MediaType));

        var hasContent = false;
        object? requestContent = null;
        string? requestContentMediaType = null;

        foreach (var property in requestType.DeclaredProperties)
        {
            var restContentAttribute = property.GetCustomAttribute<RestContentAttribute>();
            var acceptLanguageAttribute = property.GetCustomAttribute<AcceptLanguageAttribute>();

            if (acceptLanguageAttribute != null)
            {
                var propertyValue = (CultureInfo?)property.GetValue(request);
                if (propertyValue is not null)
                {
                    httpClient.DefaultRequestHeaders.AcceptLanguage.Add(acceptLanguageAttribute.Quality.HasValue
                        ? new StringWithQualityHeaderValue(propertyValue.Name, acceptLanguageAttribute.Quality.Value)
                        : new StringWithQualityHeaderValue(propertyValue.Name));
                }
            }

            if (restContentAttribute != null)
            {
                if (hasContent)
                {
                    throw new NotSupportedException("Request model can't have more than one content property.");
                }

                hasContent = true;

                requestContent = property.GetValue(request);
                requestContentMediaType = restContentAttribute.MediaType;
            }
            else
            {
                var propertyValue = property.GetValue(request);
                var parameter = string.Empty;

                if (propertyValue != null)
                {
                    string? propertyStringValue;

                    if (property.PropertyType == typeof(DateTimeOffset))
                    {
                        propertyStringValue = ((DateTimeOffset)propertyValue).ToString("s");
                    }
                    else
                    {
                        propertyStringValue = propertyValue.ToString();
                    }

                    parameter = propertyStringValue ?? string.Empty;
                }

                resourceLocation = ReplaceParameterValueInLocation(resourceLocation, property.Name, parameter);
            }
        }

        await this.SetAuthenticationHeaderAsync(restEndpointAttribute, httpClient).ConfigureAwait(false);

        return await this.MakeRequestAsync<TResult>(httpClient, restEndpointAttribute.Verb, requestUrl, requestContent,
            requestContentMediaType, restEndpointAttribute.MediaType, cancellationToken).ConfigureAwait(false);
    }

    private static string ReplaceParameterValueInLocation(
        string resourceLocation,
        string parameterName,
        string parameterValue)
    {
        parameterValue ??= string.Empty;
        var escapedParameterValue = Uri.EscapeDataString(parameterValue);

        return resourceLocation.Replace($"{{{parameterName}}}", escapedParameterValue, StringComparison.Ordinal);
    }

    private StringContent? GetContent(object? requestContent, string? requestContentMediaType)
    {
        if (requestContent is not null && requestContentMediaType is not null)
        {
            return new(
            this.serializerRestFactory.Create(requestContentMediaType).Serialize(requestContent), Encoding.UTF8,
            requestContentMediaType);
        }

        return null;
    }

    private async Task<HttpResponseMessage> MakePostRequestAsync(
        HttpClient httpClient,
        Uri requestUrl,
        object? requestContent,
        string? requestContentMediaType,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(requestUrl);

        using var content = this.GetContent(requestContent, requestContentMediaType);
        return await httpClient.PostAsync(requestUrl, content, cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> MakePutRequestAsync(
        HttpClient httpClient,
        Uri requestUrl,
        object? requestContent,
        string? requestContentMediaType,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(requestUrl);

        using var content = this.GetContent(requestContent, requestContentMediaType);
        return await httpClient.PutAsync(requestUrl, content, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TResult?> MakeRequestAsync<TResult>(
        HttpClient httpClient,
        RestVerb verb,
        Uri requestUrl,
        object? requestContent,
        string? requestContentMediaType,
        string responseContentMediaType,
        CancellationToken cancellationToken)
    {
        using var response = verb switch
        {
            RestVerb.Get => await httpClient.GetAsync(requestUrl, cancellationToken).ConfigureAwait(false),
            RestVerb.Delete => await httpClient.DeleteAsync(requestUrl, cancellationToken).ConfigureAwait(false),
            RestVerb.Put => await this.MakePutRequestAsync(httpClient, requestUrl, requestContent, requestContentMediaType, cancellationToken).ConfigureAwait(false),
            RestVerb.Post => await this.MakePostRequestAsync(httpClient, requestUrl, requestContent, requestContentMediaType, cancellationToken).ConfigureAwait(false),
            _ => throw new NotSupportedException($"Request method '{verb}' is not supported."),
        };
        return await this.ParseResponseAsync<TResult>(response, responseContentMediaType).ConfigureAwait(false);
    }

    private async Task<TResult?> ParseResponseAsync<TResult>(
        HttpResponseMessage response,
        string responseContentMediaType)
    {
        var result = default(TResult);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (content is not null)
        {
            result = this.deserializerRestFactory.Create(responseContentMediaType).Deserialize<TResult>(content);
        }

        return result;
    }

    private async Task SetAuthenticationHeaderAsync(RestEndpointAttribute restEndpointAttribute, HttpClient httpClient)
    {
        string authenticationSchema;
        switch (restEndpointAttribute.Authentication)
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
                    $"Authentication type '{restEndpointAttribute.Authentication}' is not supported.");
        }

        var authenticationToken =
            await this.restAuthenticationTokenProvider.GetAuthenticationTokenAsync(restEndpointAttribute.ApiKey).ConfigureAwait(false);

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(authenticationSchema, authenticationToken);
    }
}
