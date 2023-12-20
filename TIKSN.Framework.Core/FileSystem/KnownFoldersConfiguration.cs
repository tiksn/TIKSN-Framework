using System.Reflection;

namespace TIKSN.FileSystem;

public class KnownFoldersConfiguration
{
    public KnownFoldersConfiguration(Assembly mainAssembly, KnownFolderVersionConsideration versionConsideration)
    {
        this.MainAssembly = mainAssembly ?? throw new ArgumentNullException(nameof(mainAssembly));
        this.VersionConsideration = versionConsideration;
    }

    public Assembly MainAssembly { get; }
    public KnownFolderVersionConsideration VersionConsideration { get; }
}
