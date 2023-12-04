using LanguageExt;
using Microsoft.Extensions.Options;
using Microsoft.Win32;

#pragma warning disable CA1416 // Validate platform compatibility

namespace TIKSN.Settings;

public class WindowsRegistrySettingsService : ISettingsService
{
    private readonly IOptions<WindowsRegistrySettingsServiceOptions> options;

    public WindowsRegistrySettingsService(
        IOptions<WindowsRegistrySettingsServiceOptions> options)
    {
        this.options = options;
        this.ValidateOptions();
    }

    public T GetLocalSetting<T>(string name, T defaultValue) =>
        this.Process(
            RegistryHive.CurrentUser,
            name,
            defaultValue,
            GetSetting);

    public Option<T> GetLocalSetting<T>(string name) =>
        this.Process(
            RegistryHive.CurrentUser,
            name,
            Option<T>.None,
            FindSetting);

    public T GetRoamingSetting<T>(string name, T defaultValue) =>
        this.Process(
            RegistryHive.LocalMachine,
            name,
            defaultValue,
            GetSetting);

    public Option<T> GetRoamingSetting<T>(string name) =>
        this.Process(
            RegistryHive.LocalMachine,
            name,
            Option<T>.None,
            FindSetting);

    public IReadOnlyCollection<string> ListLocalSetting() =>
        this.Process<IReadOnlyCollection<string>>(
            RegistryHive.CurrentUser,
            string.Empty,
            value: null,
            ListNames);

    public IReadOnlyCollection<string> ListRoamingSetting() =>
        this.Process<IReadOnlyCollection<string>>(
            RegistryHive.LocalMachine,
            string.Empty,
            value: null,
            ListNames);

    public void RemoveLocalSetting(string name) =>
        this.Process(
            RegistryHive.CurrentUser,
            name,
            string.Empty,
            RemoveSetting);

    public void RemoveRoamingSetting(string name) =>
        this.Process(
            RegistryHive.LocalMachine,
            name,
            string.Empty,
            RemoveSetting);

    public void SetLocalSetting<T>(string name, T value) =>
        this.Process(
            RegistryHive.CurrentUser,
            name,
            value,
            SetSetting);

    public void SetRoamingSetting<T>(string name, T value) =>
        this.Process(
            RegistryHive.LocalMachine,
            name,
            value,
            SetSetting);

    private static Option<T> FindSetting<T>(
        RegistryKey subKey,
        string name,
        Option<T> defaultValue)
    {
        var value = subKey.GetValue(name, defaultValue: null);
        if (value == null)
        {
            return defaultValue;
        }
        return (T)value;
    }

    private static T GetSetting<T>(
        RegistryKey subKey,
        string name,
        T defaultValue) => (T)subKey.GetValue(name, defaultValue);

    private static T RemoveSetting<T>(
        RegistryKey subKey,
        string name,
        T value)
    {
        subKey.DeleteValue(name);

        return value;
    }

    private static T SetSetting<T>(
        RegistryKey subKey,
        string name,
        T value)
    {
        subKey.SetValue(name, value);

        return value;
    }

    private static IReadOnlyCollection<string> ListNames(
        RegistryKey subKey,
        string name,
        IReadOnlyCollection<string> value) => subKey.GetValueNames();

    private T Process<T>(
        RegistryHive hiveKey,
        string name,
        T value,
        Func<RegistryKey, string, T, T> processor)
    {
        ArgumentNullException.ThrowIfNull(processor);

        this.ValidateOptions();

        using var rootKey = RegistryKey.OpenBaseKey(hiveKey, this.options.Value.RegistryView);
        using var registrySubKey = rootKey.CreateSubKey(this.options.Value.SubKey, writable: true);
        return processor(registrySubKey, name, value);
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(this.options.Value.SubKey))
        {
            throw new InvalidOperationException($"Parameter '{nameof(this.options.Value.SubKey)}' is null or white space.");
        }
    }
}

#pragma warning restore CA1416 // Validate platform compatibility
