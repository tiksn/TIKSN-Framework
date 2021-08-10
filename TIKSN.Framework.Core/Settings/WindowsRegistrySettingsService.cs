using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Win32;

namespace TIKSN.Settings
{
    public class WindowsRegistrySettingsService : ISettingsService
    {
        private readonly IOptions<WindowsRegistrySettingsServiceOptions> _options;

        public WindowsRegistrySettingsService(IOptions<WindowsRegistrySettingsServiceOptions> options)
        {
            this._options = options;
            this.ValidateOptions();
        }

        public T GetLocalSetting<T>(string name, T defaultValue) =>
            this.Process(RegistryHive.CurrentUser, name, defaultValue, GetSetting);

        public T GetRoamingSetting<T>(string name, T defaultValue) =>
            this.Process(RegistryHive.LocalMachine, name, defaultValue, GetSetting);

        public IReadOnlyCollection<string> ListLocalSetting() =>
            this.Process<IReadOnlyCollection<string>>(RegistryHive.CurrentUser, null, null, this.ListNames);

        public IReadOnlyCollection<string> ListRoamingSetting() =>
            this.Process<IReadOnlyCollection<string>>(RegistryHive.LocalMachine, null, null, this.ListNames);

        public void RemoveLocalSetting(string name) =>
            this.Process<object>(RegistryHive.CurrentUser, name, null, RemoveSetting);

        public void RemoveRoamingSetting(string name) =>
            this.Process<object>(RegistryHive.LocalMachine, name, null, RemoveSetting);

        public void SetLocalSetting<T>(string name, T value) =>
            this.Process(RegistryHive.CurrentUser, name, value, SetSetting);

        public void SetRoamingSetting<T>(string name, T value) =>
            this.Process(RegistryHive.LocalMachine, name, value, SetSetting);

        private static T GetSetting<T>(RegistryKey subKey, string name, T defaultValue) =>
            (T)subKey.GetValue(name, defaultValue);

        private IReadOnlyCollection<string> ListNames(RegistryKey subKey, string name,
            IReadOnlyCollection<string> value) => subKey.GetValueNames();

        private T Process<T>(RegistryHive hiveKey, string name, T value, Func<RegistryKey, string, T, T> processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            this.ValidateOptions();

            using var rootKey = RegistryKey.OpenBaseKey(hiveKey, this._options.Value.RegistryView);
            using var registrySubKey = rootKey.CreateSubKey(this._options.Value.SubKey, true);
            return processor(registrySubKey, name, value);
        }

        private static T RemoveSetting<T>(RegistryKey subKey, string name, T value)
        {
            subKey.DeleteValue(name);

            return value;
        }

        private static T SetSetting<T>(RegistryKey subKey, string name, T value)
        {
            subKey.SetValue(name, value);

            return value;
        }

        private void ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(this._options.Value.SubKey))
            {
                throw new ArgumentException("Parameter is null or white space.", nameof(this._options.Value.SubKey));
            }
        }
    }
}
