﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace TIKSN.Configuration
{
    public static class RegistryConfigurationManager
    {
        public static T ReadObject<T>(string rootKey)
        {
            var configuration = Activator.CreateInstance<T>();
            var keys = new List<string>();
            keys.Add(rootKey);

            Initialize(configuration, keys);

            return configuration;
        }

        private static object GetValueFromRegistry(IEnumerable<string> keys, string valueName, Type type)
        {
            var globalValue = GetValueFromRegistry(RegistryHive.LocalMachine, keys, valueName);
            var localValue = GetValueFromRegistry(RegistryHive.CurrentUser, keys, valueName);

            object result = null;

            if (globalValue != null)
                result = globalValue;

            if (localValue != null)
                result = localValue;

            if (result == null)
                throw new Exception("Value not found");

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

            var result = regKey.GetValue(valueName);

            return result;
        }

        private static void Initialize(object obj, IEnumerable<string> keys)
        {
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(obj, GetValueFromRegistry(keys, propertyInfo.Name, propertyInfo.PropertyType));
                }
                else
                {
                    //TODO: in case of complex values, lists, dictionaries
                }
            }
        }
    }
}