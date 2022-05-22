using Microsoft.Extensions.Configuration;

namespace TIKSN.Configuration
{
    public abstract class DatabaseConfigurationBase
    {
        private readonly IConfigurationRoot _configurationRoot;

        protected DatabaseConfigurationBase(IConfigurationRoot configurationRoot) =>
            this._configurationRoot = configurationRoot;

        protected string GetConnectionString(string name) => this._configurationRoot.GetConnectionString(name);
    }
}
