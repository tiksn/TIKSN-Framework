using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace TIKSN.FileSystem;

public class KnownFolders : IKnownFolders
{
    private readonly Assembly mainAssembly;
    private readonly KnownFolderVersionConsideration versionConsideration;

    public KnownFolders(KnownFoldersConfiguration knownFoldersConfiguration)
    {
        ArgumentNullException.ThrowIfNull(knownFoldersConfiguration);

        this.mainAssembly = knownFoldersConfiguration.MainAssembly;
        this.versionConsideration = knownFoldersConfiguration.VersionConsideration;

        this.LocalApplicationData = this.GetFromSpecialFolder(Environment.SpecialFolder.LocalApplicationData);
        this.RoamingApplicationData = this.GetFromSpecialFolder(Environment.SpecialFolder.ApplicationData);
    }

    public IFileProvider LocalApplicationData { get; }

    public IFileProvider RoamingApplicationData { get; }

    private static PhysicalFileProvider GetFromFolderPath(string folderPath)
    {
        _ = Directory.CreateDirectory(folderPath);

        return new PhysicalFileProvider(folderPath);
    }

    private PhysicalFileProvider GetFromSpecialFolder(Environment.SpecialFolder specialFolder)
    {
        var folderPath = Environment.GetFolderPath(specialFolder);

        var fileVersionInfo = FileVersionInfo.GetVersionInfo(this.mainAssembly.Location);

        folderPath = Path.Combine(folderPath, fileVersionInfo.CompanyName);
        folderPath = Path.Combine(folderPath, fileVersionInfo.ProductName);

        switch (this.versionConsideration)
        {
            case KnownFolderVersionConsideration.None:
                break;

            case KnownFolderVersionConsideration.Major:
                folderPath = Path.Combine(folderPath, this.mainAssembly.GetName().Version.ToString(1));
                break;

            case KnownFolderVersionConsideration.MajorMinor:
                folderPath = Path.Combine(folderPath, this.mainAssembly.GetName().Version.ToString(2));
                break;

            case KnownFolderVersionConsideration.MajorMinorBuild:
                folderPath = Path.Combine(folderPath, this.mainAssembly.GetName().Version.ToString(3));
                break;

            case KnownFolderVersionConsideration.MajorMinorBuildRevision:
                folderPath = Path.Combine(folderPath, this.mainAssembly.GetName().Version.ToString(4));
                break;

            default:
                throw new InvalidOperationException($"Unknown Version Consideration: {nameof(this.versionConsideration)}");
        }

        return GetFromFolderPath(folderPath);
    }
}
