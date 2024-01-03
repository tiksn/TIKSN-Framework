namespace TIKSN.Web.Rest;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RestContentAttribute : Attribute
{
    public RestContentAttribute(string mediaType = "application/json") => this.MediaType = mediaType;

    public string MediaType { get; }
}
