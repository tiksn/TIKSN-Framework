using System.Collections.Generic;

namespace TIKSN.Settings
{
    public interface ISettingsService
    {
        T GetLocalSetting<T>(string name, T defaultValue);

        T GetRoamingSetting<T>(string name, T defaultValue);

        IReadOnlyCollection<string> ListLocalSetting();

        IReadOnlyCollection<string> ListRoamingSetting();

        void RemoveLocalSetting(string name);

        void RemoveRoamingSetting(string name);

        void SetLocalSetting<T>(string name, T value);

        void SetRoamingSetting<T>(string name, T value);
    }
}
