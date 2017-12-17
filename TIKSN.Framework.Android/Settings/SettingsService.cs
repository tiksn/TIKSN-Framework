using Android.App;
using Android.Content;
using Android.Preferences;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TIKSN.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly IOptions<AndroidSettingsOptions> options;

        public SettingsService(IOptions<AndroidSettingsOptions> options)
        {
            this.options = options;
        }

        public T GetLocalSetting<T>(string name, T defaultValue)
        {
            var preferences = GetLocalPreferences();

            return GetSetting<T>(preferences, name, defaultValue);
        }

        public T GetRoamingSetting<T>(string name, T defaultValue)
        {
            var preferences = GetRoamingPreferences();

            return GetSetting<T>(preferences, name, defaultValue);
        }

        public IReadOnlyCollection<string> ListLocalSetting()
        {
            var preferences = GetLocalPreferences();

            return preferences.All.Keys.ToArray();
        }

        public IReadOnlyCollection<string> ListRoamingSetting()
        {
            var preferences = GetRoamingPreferences();

            return preferences.All.Keys.ToArray();
        }

        public void RemoveLocalSetting(string name)
        {
            var preferences = GetLocalPreferences();

            RemoveSetting(preferences, name);
        }

        public void RemoveRoamingSetting(string name)
        {
            var preferences = GetRoamingPreferences();

            RemoveSetting(preferences, name);
        }

        public void SetLocalSetting<T>(string name, T value)
        {
            var preferences = GetLocalPreferences();

            SetSetting(preferences, name, value);
        }

        public void SetRoamingSetting<T>(string name, T value)
        {
            var preferences = GetRoamingPreferences();
            SetSetting(preferences, name, value);
        }

        private ISharedPreferences GetLocalPreferences()
        {
            var ctx = Application.Context.ApplicationContext;
            var preferences = PreferenceManager.GetDefaultSharedPreferences(ctx);
            return preferences;
        }

        private ISharedPreferences GetRoamingPreferences()
        {
            var ctx = Application.Context.ApplicationContext;
            var preferences = ctx.GetSharedPreferences(options.Value.RoamingNamespace, FileCreationMode.WorldWriteable);
            return preferences;
        }

        private T GetSetting<T>(ISharedPreferences preferences, string name, T defaultValue)
        {
            var type = typeof(T);

            //Is Not Universal (T)(object)preferences.GetStringSet(name, default(ICollection<string>));

            var typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return (T)(object)preferences.GetBoolean(name, (bool)(object)defaultValue);

                case TypeCode.Int32:
                    return (T)(object)preferences.GetInt(name, (int)(object)defaultValue); ;

                case TypeCode.Int64:
                    return (T)(object)preferences.GetLong(name, (long)(object)defaultValue);

                case TypeCode.Single:
                    return (T)(object)preferences.GetFloat(name, (float)(object)defaultValue);

                case TypeCode.String:
                    return (T)(object)preferences.GetString(name, (string)(object)defaultValue);

                default:
                    if (defaultValue is ICollection<string>)
                        return (T)preferences.GetStringSet(name, (ICollection<string>)(object)defaultValue);
                    else
                    {
                        var jsonValue = preferences.GetString(name, null);
                        if (!string.IsNullOrEmpty(jsonValue))
                            return JsonConvert.DeserializeObject<T>(jsonValue);

                        return defaultValue;
                    }
                    break;
            }
        }

        private void SetSetting<T>(ISharedPreferences preferences, string name, T value)
        {
            using (var prefs = preferences)
            {
                using (var editor = prefs.Edit())
                {
                    SetSetting(editor, name, value);

                    editor.Commit();
                }
            }
        }

        private void RemoveSetting(ISharedPreferences preferences, string name)
        {
            using (var prefs = preferences)
            {
                using (var editor = prefs.Edit())
                {
                    editor.Remove(name);

                    editor.Commit();
                }
            }
        }

        private void SetSetting<T>(ISharedPreferencesEditor preferencesEditor, string name, T value)
        {
            var type = typeof(T);

            //Is Not Universal preferencesEditor.PutStringSet(name, (ICollection<string>)value);

            var typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    preferencesEditor.PutBoolean(name, (bool)(object)value);
                    break;

                case TypeCode.Int32:
                    preferencesEditor.PutInt(name, (int)(object)value);
                    break;

                case TypeCode.Int64:
                    preferencesEditor.PutLong(name, (long)(object)value);
                    break;

                case TypeCode.Single:
                    preferencesEditor.PutFloat(name, (float)(object)value);
                    break;

                case TypeCode.String:
                    preferencesEditor.PutString(name, (string)(object)value);
                    break;

                default:
                    if (value is ICollection<string>)
                        preferencesEditor.PutStringSet(name, (ICollection<string>)value);
                    else
                    {
                        var jsonValue = JsonConvert.SerializeObject(value);
                        preferencesEditor.PutString(name, jsonValue);
                    }
                    break;
            }
        }
    }
}