﻿using System;
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
			var resourceLocation = _template;

			foreach (var parameter in _values)
			{
				var parameterName = $"{{{parameter.Key}}}";
				var escapedParameterValue = Uri.EscapeUriString(parameter.Value) ?? string.Empty;

				if (resourceLocation.Contains(parameterName))
					resourceLocation = resourceLocation.Replace(parameterName, escapedParameterValue);
				else
				{
					var queryToAppend = $"{parameterName}={escapedParameterValue}";
					if (resourceLocation.EndsWith("?"))
						resourceLocation = $"{resourceLocation}{queryToAppend}";
					else if (resourceLocation.Contains("?"))
						resourceLocation = $"{resourceLocation}&{queryToAppend}";
					else
						resourceLocation = $"{resourceLocation}?{queryToAppend}";
				}
			}

			return new Uri(resourceLocation, UriKind.Relative);
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
