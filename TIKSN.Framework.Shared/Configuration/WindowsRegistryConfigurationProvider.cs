using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System.Security;

namespace TIKSN.Configuration
{
    public class WindowsRegistryConfigurationProvider : ConfigurationProvider
    {
        private readonly RegistryView _registryView;
        private readonly string _rootKey;

        public WindowsRegistryConfigurationProvider(string rootKey, RegistryView registryView)
        {
            _registryView = registryView;
            _rootKey = rootKey;
        }

        public override void Load()
        {
            Data.Clear();

            try
            {
                using (var machineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, _registryView))
                {
                    PopulateKeys(machineKey, null);
                }
            }
            catch(SecurityException)
            {
            }

            try
            {
                using (var userKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, _registryView))
                {
                    PopulateKeys(userKey, null);
                }
            }
            catch(SecurityException)
            {
            }
        }

        private void PopulateKeys(RegistryKey currentRegistryKey, string parentConfigurationKey)
        {
            PopulateValues(currentRegistryKey, parentConfigurationKey);

            var subKeyNames = currentRegistryKey.GetSubKeyNames();

            foreach (var subKeyName in subKeyNames)
            {
                using (var registrySubKey = currentRegistryKey.OpenSubKey(subKeyName))
                {
                    PopulateKeys(currentRegistryKey, ConfigurationPath.Combine(parentConfigurationKey, subKeyName));
                }
            }
        }

        private void PopulateValues(RegistryKey currentRegistryKey, string parentConfigurationKey)
        {
            var valueNames = currentRegistryKey.GetValueNames();

            foreach (var valueName in valueNames)
            {
                var valueData = currentRegistryKey.GetValue(valueName);

                Set(ConfigurationPath.Combine(parentConfigurationKey, valueName), valueData.ToString());
            }
        }
    }
}