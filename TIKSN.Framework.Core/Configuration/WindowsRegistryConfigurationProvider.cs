using System.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace TIKSN.Configuration;

public class WindowsRegistryConfigurationProvider : ConfigurationProvider
{
    private readonly RegistryView registryView;
    private readonly string rootKey;

    public WindowsRegistryConfigurationProvider(string rootKey, RegistryView registryView)
    {
        if (string.IsNullOrWhiteSpace(rootKey))
        {
            throw new ArgumentException("Parameter is null or white space.", nameof(rootKey));
        }

        this.registryView = registryView;
        this.rootKey = rootKey;
    }

    public override void Load()
    {
        this.Data.Clear();

        try
        {
            using var machineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, this.registryView);
            if (machineKey != null)
            {
                this.PopulateRootKey(machineKey);
            }
        }
        catch (SecurityException)
        {
        }

        try
        {
            using var userKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, this.registryView);
            if (userKey != null)
            {
                this.PopulateRootKey(userKey);
            }
        }
        catch (SecurityException)
        {
        }
    }

    private static string CombineConfigurationPath(string parentConfigurationKey, string childConfigurationKey)
    {
        if (string.IsNullOrEmpty(parentConfigurationKey))
        {
            return childConfigurationKey;
        }

        return ConfigurationPath.Combine(parentConfigurationKey, childConfigurationKey);
    }

    private void PopulateKeys(RegistryKey currentRegistryKey, string parentConfigurationKey)
    {
        this.PopulateValues(currentRegistryKey, parentConfigurationKey);

        foreach (var subKeyName in currentRegistryKey.GetSubKeyNames())
        {
            using var registrySubKey = currentRegistryKey.OpenSubKey(subKeyName);
            if (registrySubKey != null)
            {
                this.PopulateKeys(registrySubKey, CombineConfigurationPath(parentConfigurationKey, subKeyName));
            }
        }
    }

    private void PopulateRootKey(RegistryKey hiveKey)
    {
        using var registryKey = hiveKey.OpenSubKey(this.rootKey);
        if (registryKey != null)
        {
            this.PopulateKeys(registryKey, parentConfigurationKey: null);
        }
    }

    private void PopulateValues(RegistryKey currentRegistryKey, string parentConfigurationKey)
    {
        foreach (var valueName in currentRegistryKey.GetValueNames())
        {
            var valueData = currentRegistryKey.GetValue(valueName);

            this.Set(CombineConfigurationPath(parentConfigurationKey, valueName), valueData.ToString());
        }
    }
}
