using Microsoft.Win32;

namespace TIKSN.Platforms.Windows.Configuration;

public static class RegistryConfigurationManager
{
    public static T ReadObject<T>(string rootKey)
    {
        var configuration = Activator.CreateInstance<T>();

        Initialize(configuration, rootKey);

        return configuration;
    }

    private static object GetValueFromRegistry(string key, string valueName)
    {
        var globalValue = GetValueFromRegistry(RegistryHive.LocalMachine, key, valueName);
        var localValue = GetValueFromRegistry(RegistryHive.CurrentUser, key, valueName);

        object result = null;

        if (globalValue != null)
        {
            result = globalValue;
        }

        if (localValue != null)
        {
            result = localValue;
        }

        if (result == null)
        {
            throw new InvalidOperationException("Value not found");
        }

        return result;
    }

    private static object GetValueFromRegistry(RegistryHive hive, string key, string valueName)
    {
        using var regKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
        using var regSubKey = regKey.OpenSubKey(key);

        return regSubKey.GetValue(valueName);
    }

    private static void Initialize(object obj, string key)
    {
        foreach (var propertyInfo in obj.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(obj, GetValueFromRegistry(key, propertyInfo.Name));
            }
        }
    }
}
