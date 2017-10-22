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
            ValidateOptions();
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
            ValidateOptions();

            using (var rootKey = RegistryKey.OpenBaseKey(hiveKey, _options.Value.RegistryView))
            {
                using (var registrySubKey = rootKey.OpenSubKey(_options.Value.SubKey))
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

        private void ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(_options.Value.SubKey))
                throw new ArgumentException("Parameter is null or white space.", nameof(_options.Value.SubKey));
        }
    }
}