using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;

namespace TIKSN.Settings
{
	public class WindowsRegistrySettingsService : ISettingsService
	{
		private readonly IOptions<WindowsRegistrySettingsServiceOptions> _options;

		public WindowsRegistrySettingsService(IOptions<WindowsRegistrySettingsServiceOptions> options)
		{
			_options = options;
		}

		public T GetLocalSetting<T>(string name, T defaultValue)
		{
			return Process(RegistryHive.CurrentUser, name, defaultValue, GetSetting);
		}

		public T GetRoamingSetting<T>(string name, T defaultValue)
		{
			return Process(RegistryHive.LocalMachine, name, defaultValue, GetSetting);
		}

		public void SetLocalSetting<T>(string name, T value)
		{
			Process(RegistryHive.CurrentUser, name, value, SetSetting);
		}

		public void SetRoamingSetting<T>(string name, T value)
		{
			Process(RegistryHive.LocalMachine, name, value, SetSetting);
		}

		private T GetSetting<T>(RegistryKey subKey, string name, T defaultValue)
		{
			return (T)subKey.GetValue(name, defaultValue);
		}

		private T Process<T>(RegistryHive hiveKey, string name, T value, Func<RegistryKey, string, T, T> processor)
		{
			using (var machineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, _options.Value.RegistryView))
			{
				using (var registrySubKey = machineKey.OpenSubKey(_options.Value.SubKey))
				{
					return processor(registrySubKey, name, value);
				}
			}
		}

		private T SetSetting<T>(RegistryKey subKey, string name, T value)
		{
			subKey.SetValue(name, value);

			return value;
		}
	}
}