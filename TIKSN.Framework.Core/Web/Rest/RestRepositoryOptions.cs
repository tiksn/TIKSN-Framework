using System.Text;

namespace TIKSN.Web.Rest;

public class RestRepositoryOptions<T>
{
    public RestRepositoryOptions()
    {
        this.MediaType = "application/json";
        this.Encoding = Encoding.UTF8;
    }

    public string ApiKey { get; set; }

    public string MediaType { get; set; }

    public RestAuthenticationType Authentication { get; set; }

    public Dictionary<double, string> AcceptLanguages { get; set; }

    public string ResourceTemplate { get; set; }

    public Encoding Encoding { get; set; }
}
