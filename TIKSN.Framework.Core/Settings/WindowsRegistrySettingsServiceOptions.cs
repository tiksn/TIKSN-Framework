using Microsoft.Win32;

namespace TIKSN.Settings
{
    public class WindowsRegistrySettingsServiceOptions
    {
        public WindowsRegistrySettingsServiceOptions() => this.RegistryView = RegistryView.Default;

        public RegistryView RegistryView { get; set; }

        public string SubKey { get; set; }
    }
}
