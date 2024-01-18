using System.Text;

namespace TIKSN.Web.Rest;

#pragma warning disable S2326 // Unused type parameters should be removed
public class RestRepositoryOptions<T>
#pragma warning restore S2326 // Unused type parameters should be removed
{
    public RestRepositoryOptions()
    {
        this.MediaType = "application/json";
        this.Encoding = Encoding.UTF8;
        this.AcceptLanguages = new Dictionary<double, string>();
    }

    public string EndpointKey { get; set; }

    public string MediaType { get; set; }

    public RestAuthenticationType Authentication { get; set; }

    public IDictionary<double, string> AcceptLanguages { get; }

    public string ResourceTemplate { get; set; }

    public Encoding Encoding { get; set; }
}
