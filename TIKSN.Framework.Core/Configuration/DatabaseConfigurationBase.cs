using Microsoft.Extensions.Configuration;

namespace TIKSN.Configuration;

public abstract class DatabaseConfigurationBase
{
    private readonly IConfigurationRoot configurationRoot;

    protected DatabaseConfigurationBase(IConfigurationRoot configurationRoot) =>
        this.configurationRoot = configurationRoot;

    protected string? GetConnectionString(string name) => this.configurationRoot.GetConnectionString(name);
}
