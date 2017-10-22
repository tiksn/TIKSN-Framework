using Microsoft.Extensions.Configuration;

namespace TIKSN.Configuration
{
    public abstract class DatabaseConfigurationBase
    {
        private readonly IConfigurationRoot _configurationRoot;

        protected DatabaseConfigurationBase(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        protected string GetConnectionString(string name)
        {
            return _configurationRoot.GetConnectionString(name);
        }
    }
}