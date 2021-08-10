using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace TIKSN.Settings
{
    public class SettingsService : ISettingsService
    {
        public T GetLocalSetting<T>(string name, T defaultValue)
        {
            return GetSetting<T>(ApplicationData.Current.LocalSettings, name, defaultValue);
        }

        public T GetRoamingSetting<T>(string name, T defaultValue)
        {
            return GetSetting<T>(ApplicationData.Current.RoamingSettings, name, defaultValue);
        }

        public IReadOnlyCollection<string> ListLocalSetting()
        {
            return ApplicationData.Current.LocalSettings.Values.Keys.ToArray();
        }

        public IReadOnlyCollection<string> ListRoamingSetting()
        {
            return ApplicationData.Current.RoamingSettings.Values.Keys.ToArray();
        }

        public void RemoveLocalSetting(string name)
        {
            RemoveSetting(ApplicationData.Current.LocalSettings, name);
        }

        public void RemoveRoamingSetting(string name)
        {
            RemoveSetting(ApplicationData.Current.RoamingSettings, name);
        }

        public void SetLocalSetting<T>(string name, T value)
        {
            SetSetting(ApplicationData.Current.LocalSettings, name, value);
        }

        public void SetRoamingSetting<T>(string name, T value)
        {
            SetSetting(ApplicationData.Current.RoamingSettings, name, value);
        }

        private T GetSetting<T>(ApplicationDataContainer container, string name, T defaultValue)
        {
            if (container.Values.ContainsKey(name))
            {
                return (T)container.Values[name];
            }
            else
            {
                return defaultValue;
            }
        }

        private void RemoveSetting(ApplicationDataContainer container, string name)
        {
            container.Values.Remove(name);
        }

        private void SetSetting<T>(ApplicationDataContainer container, string name, T value)
        {
            container.Values[name] = value;
        }
    }
}
