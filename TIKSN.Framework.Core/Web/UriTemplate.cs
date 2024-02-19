namespace TIKSN.Web;

public class UriTemplate
{
    private readonly string template;
    private readonly Dictionary<string, string> values;

    public UriTemplate(string template)
    {
        this.template = template;
        this.values = new Dictionary<string, string>(StringComparer.Ordinal);
    }

    public Uri Compose()
    {
        var resourceLocation = this.template;

        foreach (var parameter in this.values)
        {
            var parameterName = $"{{{parameter.Key}}}";
            var escapedParameterValue = Uri.EscapeDataString(parameter.Value) ?? string.Empty;

            if (resourceLocation.Contains(parameterName, StringComparison.Ordinal))
            {
                resourceLocation = resourceLocation.Replace(parameterName, escapedParameterValue, StringComparison.Ordinal);
            }
            else
            {
                var queryToAppend = $"{parameterName}={escapedParameterValue}";
                if (resourceLocation.EndsWith('?'))
                {
                    resourceLocation += queryToAppend;
                }
                else if (resourceLocation.Contains('?', StringComparison.Ordinal))
                {
                    resourceLocation = $"{resourceLocation}&{queryToAppend}";
                }
                else
                {
                    resourceLocation = $"{resourceLocation}?{queryToAppend}";
                }
            }
        }

        return new Uri(resourceLocation, UriKind.Relative);
    }

    public void Fill(string name, string value) => this.values.Add(name, value);

    public void UnFill(string name) => this.values.Remove(name);
}
