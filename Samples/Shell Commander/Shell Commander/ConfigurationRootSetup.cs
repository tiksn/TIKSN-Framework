using TIKSN.Configuration;

namespace Shell_Commander
{
    public class ConfigurationRootSetup : ConfigurationRootSetupBase
    {
        private readonly string[] _args;

        public ConfigurationRootSetup(string[] args)
        {
            _args = args;
        }
    }
}
