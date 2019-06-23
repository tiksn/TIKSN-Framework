using System;
using System.Reflection;

namespace TIKSN.FileSystem
{
    public class KnownFoldersConfiguration
    {
        public KnownFoldersConfiguration(Assembly mainAssembly, KnownFolderVersionConsideration versionConsideration)
        {
            MainAssembly = mainAssembly ?? throw new ArgumentNullException(nameof(mainAssembly));
            VersionConsideration = versionConsideration;
        }

        public Assembly MainAssembly { get; }
        public KnownFolderVersionConsideration VersionConsideration { get; }
    }
}
