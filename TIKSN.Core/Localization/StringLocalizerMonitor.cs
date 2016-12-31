using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;

namespace TIKSN.Localization
{
	/// <summary>
	/// IStringLocalizer decorator for monitoring not found resources.
	/// </summary>
	public class StringLocalizerMonitor : IStringLocalizer
	{
		private readonly ILogger<StringLocalizerMonitor> _logger;
		private readonly IOptions<StringLocalizerMonitorOptions> _options;
		private readonly IStringLocalizer _stringLocalizer;

		public StringLocalizerMonitor(IStringLocalizer stringLocalizer, ILoggerFactory loggerFactory, IOptions<StringLocalizerMonitorOptions> options)
		{
			_stringLocalizer = stringLocalizer;
			_logger = loggerFactory.CreateLogger<StringLocalizerMonitor>();
			_options = options;
		}

		public LocalizedString this[string name]
		{
			get
			{
				return Log(_stringLocalizer[name]);
			}
		}

		public LocalizedString this[string name, params object[] arguments]
		{
			get
			{
				return Log(_stringLocalizer[name, arguments]);
			}
		}

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			return _stringLocalizer.GetAllStrings(includeParentCultures);
		}

		public IStringLocalizer WithCulture(CultureInfo culture)
		{
			return _stringLocalizer.WithCulture(culture);
		}

		private LocalizedString Log(LocalizedString localizedString)
		{
			if (localizedString.ResourceNotFound)
				_logger.Log(_options.Value.LogLevel, 414761847, $"Resource with name '{localizedString.Name}' is not found.", null, (s, e) => s);

			return localizedString;
		}
	}
}