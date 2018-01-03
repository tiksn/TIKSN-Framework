﻿using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

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

        public IReadOnlyCollection<string> ListLocalSetting()
        {
            return Process<IReadOnlyCollection<string>>(RegistryHive.CurrentUser, null, null, ListNames);
        }

        public IReadOnlyCollection<string> ListRoamingSetting()
        {
            return Process<IReadOnlyCollection<string>>(RegistryHive.LocalMachine, null, null, ListNames);
        }

        public void RemoveLocalSetting(string name)
        {
            Process<object>(RegistryHive.CurrentUser, name, null, RemoveSetting);
        }

        public void RemoveRoamingSetting(string name)
        {
            Process<object>(RegistryHive.LocalMachine, name, null, RemoveSetting);
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

        private IReadOnlyCollection<string> ListNames(RegistryKey subKey, string name, IReadOnlyCollection<string> value)
        {
            return subKey.GetValueNames();
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

        private T RemoveSetting<T>(RegistryKey subKey, string name, T value)
        {
            subKey.DeleteValue(name);

            return value;
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