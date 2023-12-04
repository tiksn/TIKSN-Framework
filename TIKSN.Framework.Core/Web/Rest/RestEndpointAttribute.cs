namespace TIKSN.Web.Rest;

[AttributeUsage(AttributeTargets.Class)]
public class RestEndpointAttribute : Attribute
{
    public RestEndpointAttribute(string apiKey, RestVerb verb, string resourceTemplate,
        RestAuthenticationType authentication = RestAuthenticationType.None, string mediaType = "application/json")
    {
        this.ApiKey = apiKey;
        this.Verb = verb;
        this.ResourceTemplate = resourceTemplate;
        this.MediaType = mediaType;
        this.Authentication = authentication;
    }

    public string ApiKey { get; }

    public RestAuthenticationType Authentication { get; }

    public string MediaType { get; }

    public string ResourceTemplate { get; }

    public RestVerb Verb { get; }
}
