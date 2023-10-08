using System;
using System.Collections.Generic;

namespace TIKSN.Web
{
    public class UriTemplate
    {
        private readonly string _template;
        private readonly Dictionary<string, string> _values;

        public UriTemplate(string template)
        {
            this._template = template;
            this._values = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public Uri Compose()
        {
            var resourceLocation = this._template;

            foreach (var parameter in this._values)
            {
                var parameterName = $"{{{parameter.Key}}}";
                var escapedParameterValue = Uri.EscapeUriString(parameter.Value) ?? string.Empty;

                if (resourceLocation.Contains(parameterName, StringComparison.Ordinal))
                {
                    resourceLocation = resourceLocation.Replace(parameterName, escapedParameterValue, StringComparison.Ordinal);
                }
                else
                {
                    var queryToAppend = $"{parameterName}={escapedParameterValue}";
                    if (resourceLocation.EndsWith("?", StringComparison.Ordinal))
                    {
                        resourceLocation = resourceLocation + queryToAppend;
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

        public void Fill(string name, string value) => this._values.Add(name, value);

        public void Unfill(string name) => this._values.Remove(name);
    }
}
