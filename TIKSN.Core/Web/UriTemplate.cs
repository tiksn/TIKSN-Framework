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
			_template = template;
			_values = new Dictionary<string, string>();
		}

		public Uri Compose()
		{
			var queriesToAppend = new List<string>();
			var resourceLocation = _template;

			foreach (var parameter in _values)
			{
				var parameterName = $"{{{parameter.Key}}}";
				var escapedParameterValue = Uri.EscapeUriString(parameter.Value) ?? string.Empty;

				if (resourceLocation.Contains(parameterName))
					resourceLocation = resourceLocation.Replace(parameterName, escapedParameterValue);
				else
					queriesToAppend.Add($"{parameterName}={escapedParameterValue}");
			}

			var builder = new UriBuilder(new Uri(resourceLocation, UriKind.Relative));

			var queryToAppend = string.Join("&", queriesToAppend);

			if (builder.Query != null && builder.Query.Length > 1)
				builder.Query = builder.Query.Substring(1) + "&" + queryToAppend;
			else
				builder.Query = queryToAppend;

			return builder.Uri;
		}

		public void Fill(string name, string value)
		{
			_values.Add(name, value);
		}

		public void Unfill(string name)
		{
			_values.Remove(name);
		}
	}
}
