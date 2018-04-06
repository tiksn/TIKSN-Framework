using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
    public class RestRequester : IRestRequester
    {
        private readonly IDeserializerRestFactory _deserializerRestFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRestAuthenticationTokenProvider _restAuthenticationTokenProvider;
        private readonly ISerializerRestFactory _serializerRestFactory;

        public RestRequester(IHttpClientFactory httpClientFactory,
            ISerializerRestFactory serializerRestFactory,
            IDeserializerRestFactory deserializerRestFactory,
            IRestAuthenticationTokenProvider restAuthenticationTokenProvider)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _serializerRestFactory = serializerRestFactory ?? throw new ArgumentNullException(nameof(serializerRestFactory));
            _deserializerRestFactory = deserializerRestFactory ?? throw new ArgumentNullException(nameof(deserializerRestFactory));
            _restAuthenticationTokenProvider = restAuthenticationTokenProvider ?? throw new ArgumentNullException(nameof(restAuthenticationTokenProvider));
        }

        public async Task<TResult> Request<TResult, TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType().GetTypeInfo();

            var restEndpointAttribute = requestType.GetCustomAttribute<RestEndpointAttribute>();

            if (restEndpointAttribute == null)
                throw new NotSupportedException("Requested Type has to have RestEndpointAttribute.");

            var resourceLocation = restEndpointAttribute.ResourceTemplate;
            var requestUrl = new Uri(resourceLocation, UriKind.Relative);

            var httpClient = await _httpClientFactory.Create(restEndpointAttribute.ApiKey);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(restEndpointAttribute.MediaType));

            var hasContent = false;
            object requestContent = null;
            string requestContentMediaType = null;

            foreach (var property in requestType.DeclaredProperties)
            {
                var restContentAttribute = property.GetCustomAttribute<RestContentAttribute>();
                var acceptLanguageAttribute = property.GetCustomAttribute<AcceptLanguageAttribute>();

                if (acceptLanguageAttribute != null)
                {
                    var propertyValue = (CultureInfo)property.GetValue(request);
                    httpClient.DefaultRequestHeaders.AcceptLanguage.Add(acceptLanguageAttribute.Quality.HasValue ?
                        new StringWithQualityHeaderValue(propertyValue.Name, acceptLanguageAttribute.Quality.Value) :
                        new StringWithQualityHeaderValue(propertyValue.Name));
                }

                if (restContentAttribute != null)
                {
                    if (hasContent)
                        throw new NotSupportedException("Request model can't have more than one content property.");
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
                        string propertyStringValue;

                        if (property.PropertyType == typeof(DateTimeOffset))
                        {
                            propertyStringValue = ((DateTimeOffset)propertyValue).ToString("s");
                        }
                        else
                        {
                            propertyStringValue = propertyValue.ToString();
                        }

                        parameter = propertyStringValue;
                    }

                    resourceLocation = ReplaceParameterValueInLocation(resourceLocation, property.Name, parameter);
                }
            }

            await SetAuthenticationHeader(restEndpointAttribute, httpClient);

            return await MakeRequest<TResult>(httpClient, restEndpointAttribute.Verb, requestUrl, requestContent, requestContentMediaType, restEndpointAttribute.MediaType, cancellationToken);
        }

        private static string ReplaceParameterValueInLocation(string resourceLocation, string parameterName, string parameterValue)
        {
            var escapedParameterValue = parameterValue ?? string.Empty;
            escapedParameterValue = Uri.EscapeUriString(parameterValue);

            resourceLocation = resourceLocation.Replace($"{{{parameterName}}}", escapedParameterValue);
            return resourceLocation;
        }

        private HttpContent GetContent(object requestContent, string requestContentMediaType)
        {
            return new StringContent(_serializerRestFactory.Create(requestContentMediaType).Serialize(requestContent), Encoding.UTF8, requestContentMediaType);
        }

        private async Task<TResult> MakeRequest<TResult>(HttpClient httpClient, RestVerb verb, Uri requestUrl, object requestContent, string requestContentMediaType, string responseContentMediaType, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            switch (verb)
            {
                case RestVerb.Get:
                    response = await httpClient.GetAsync(requestUrl, cancellationToken);
                    break;

                case RestVerb.Delete:
                    response = await httpClient.DeleteAsync(requestUrl, cancellationToken);
                    break;

                case RestVerb.Put:
                    response = await httpClient.PutAsync(requestUrl, GetContent(requestContent, requestContentMediaType), cancellationToken);
                    break;

                case RestVerb.Post:
                    response = await httpClient.PostAsync(requestUrl, GetContent(requestContent, requestContentMediaType), cancellationToken);
                    break;

                default:
                    throw new NotSupportedException($"Request method '{verb.ToString()}' is not supported.");
            }

            return await ParseResponse<TResult>(response, responseContentMediaType);
        }

        private async Task<TResult> ParseResponse<TResult>(HttpResponseMessage response, string responseContentMediaType)
        {
            var result = default(TResult);

            var content = await response.Content.ReadAsStringAsync();

            if (content != null)
            {
                result = _deserializerRestFactory.Create(responseContentMediaType).Deserialize<TResult>(content);
            }

            return result;
        }

        private async Task SetAuthenticationHeader(RestEndpointAttribute restEndpointAttribute, HttpClient httpClient)
        {
            var authenticationSchema = string.Empty;

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
                    throw new NotSupportedException($"Authentication type '{restEndpointAttribute.Authentication.ToString()}' is not supported.");
            }

            var authenticationToken = await _restAuthenticationTokenProvider.GetAuthenticationToken(restEndpointAttribute.ApiKey);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationSchema, authenticationToken);
        }
    }
}