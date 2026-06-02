using System.Text;

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
        var resourceLocation = new StringBuilder(this.template);

        foreach (var parameter in this.values)
        {
            var parameterName = $"{{{parameter.Key}}}";
            var escapedParameterValue = Uri.EscapeDataString(parameter.Value) ?? string.Empty;

            if (resourceLocation.ToString().Contains(parameterName, StringComparison.Ordinal))
            {
                _ = resourceLocation.Replace(parameterName, escapedParameterValue);
            }
            else
            {
                var queryToAppend = $"{parameterName}={escapedParameterValue}";
                if (resourceLocation.Length > 0 && resourceLocation[^1] == '?')
                {
                    _ = resourceLocation.Append(queryToAppend);
                }
                else if (resourceLocation.ToString().Contains(value: '?', StringComparison.Ordinal))
                {
                    _ = resourceLocation.Append('&').Append(queryToAppend);
                }
                else
                {
                    _ = resourceLocation.Append('?').Append(queryToAppend);
                }
            }
        }

        return new Uri(resourceLocation.ToString(), UriKind.Relative);
    }

    public void Fill(string name, string value) => this.values.Add(name, value);

    public void UnFill(string name) => this.values.Remove(name);
}
