namespace TIKSN.Settings
{
	public interface ISettingsService
	{
		T GetLocalSetting<T>(string name, T defaultValue);

		T GetRoamingSetting<T>(string name, T defaultValue);

		void SetLocalSetting<T>(string name, T value);

		void SetRoamingSetting<T>(string name, T value);
	}
}