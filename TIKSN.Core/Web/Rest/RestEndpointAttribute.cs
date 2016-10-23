using System;

namespace TIKSN.Web.Rest
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RestEndpointAttribute : Attribute
    {
        public RestEndpointAttribute(string apiName, RestVerb verb, string resourceTemplate, string mediaType = "application/json")
        {
            ApiName = apiName;
            Verb = verb;
            ResourceTemplate = resourceTemplate;
            MediaType = mediaType;
        }

        public string ApiName { get; private set; }

        public string MediaType { get; private set; }

        public string ResourceTemplate { get; private set; }

        public RestVerb Verb { get; private set; }
    }
}