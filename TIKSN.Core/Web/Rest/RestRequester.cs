using System;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
    public class RestRequester : IRestRequester
    {
        private readonly IRestRequesterConfiguration _configuration;
        private readonly IDeserializerRestFactory _deserializerRestFactory;
        private readonly ISerializerRestFactory _serializerRestFactory;

        public RestRequester(IRestRequesterConfiguration configuration, ISerializerRestFactory serializerRestFactory, IDeserializerRestFactory deserializerRestFactory)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            Contract.Requires<ArgumentNullException>(serializerRestFactory != null);
            Contract.Requires<ArgumentNullException>(deserializerRestFactory != null);

            _configuration = configuration;
            _serializerRestFactory = serializerRestFactory;
            _deserializerRestFactory = deserializerRestFactory;
        }

        public async Task<TResult> Request<TResult, TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType().GetTypeInfo();

            var restEndpointAttribute = requestType.GetCustomAttribute<RestEndpointAttribute>();

            if (restEndpointAttribute == null)
                throw new NotSupportedException("Requested Type has to have RestEndpointAttribute.");

            var resourceLocation = restEndpointAttribute.ResourceTemplate;

            bool hasContent = false;
            object requestContent = null;
            string requestContentMediaType = null;

            foreach (var property in requestType.DeclaredProperties)
            {
                var restContentAttribute = property.GetCustomAttribute<RestContentAttribute>();

                if (restContentAttribute == null)
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
                else
                {
                    if (hasContent)
                        throw new NotSupportedException("Request model can't have more than one content property.");
                    hasContent = true;

                    requestContent = property.GetValue(request);
                    requestContentMediaType = restContentAttribute.MediaType;
                }
            }

            var resourceParameters = await _configuration.GetResourceParameters(restEndpointAttribute.ApiName);
            if (resourceParameters != null)
            {
                foreach (var resourceParameter in resourceParameters)
                {
                    resourceLocation = ReplaceParameterValueInLocation(resourceLocation, resourceParameter.Key, resourceParameter.Value);
                }
            }

            var baseUrl = await _configuration.GetBaseUrl(restEndpointAttribute.ApiName);
            var requestUrl = new Uri(baseUrl, resourceLocation);

            var httpClient = new HttpClient();

            var defaultHeaders = await _configuration.GetDefaultHeaders(restEndpointAttribute.ApiName);
            if (defaultHeaders != null)
            {
                foreach (var defaultHeader in defaultHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(defaultHeader.Key, defaultHeader.Value);
                }
            }

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
    }
}