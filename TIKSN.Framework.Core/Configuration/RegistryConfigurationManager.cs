using Microsoft.Win32;

namespace TIKSN.Configuration;

public static class RegistryConfigurationManager
{
    public static T ReadObject<T>(string rootKey)
    {
        var configuration = Activator.CreateInstance<T>();
        var keys = new List<string>
        {
            rootKey,
        };

        Initialize(configuration, keys);

        return configuration;
    }

    private static object GetValueFromRegistry(IEnumerable<string> keys, string valueName)
    {
        var globalValue = GetValueFromRegistry(RegistryHive.LocalMachine, keys, valueName);
        var localValue = GetValueFromRegistry(RegistryHive.CurrentUser, keys, valueName);

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
            throw new Exception("Value not found");
        }

        return result;
    }

    private static object GetValueFromRegistry(RegistryHive hKey, IEnumerable<string> keys, string valueName)
    {
        var regKey = RegistryKey.OpenBaseKey(hKey, RegistryView.Default);

        foreach (var key in keys)
        {
            regKey = regKey.OpenSubKey(key);

            if (regKey == null)
            {
                return null;
            }
        }

        return regKey.GetValue(valueName);
    }

    private static void Initialize(object obj, IEnumerable<string> keys)
    {
        foreach (var propertyInfo in obj.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(obj,
                    GetValueFromRegistry(keys, propertyInfo.Name));
            }
        }
    }
}
