using LanguageExt;

namespace TIKSN.Settings;

public interface ISettingsService
{
    public T GetLocalSetting<T>(string name, T defaultValue);

    public Option<T> GetLocalSetting<T>(string name);

    public T GetRoamingSetting<T>(string name, T defaultValue);

    public Option<T> GetRoamingSetting<T>(string name);

    public IReadOnlyCollection<string> ListLocalSetting();

    public IReadOnlyCollection<string> ListRoamingSetting();

    public void RemoveLocalSetting(string name);

    public void RemoveRoamingSetting(string name);

    public void SetLocalSetting<T>(string name, T value);

    public void SetRoamingSetting<T>(string name, T value);
}
