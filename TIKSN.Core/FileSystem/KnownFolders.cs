using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace TIKSN.FileSystem
{
    public class KnownFolders : IKnownFolders
    {
        private readonly Assembly _mainAssembly;
        private readonly KnownFolderVersionConsideration _versionConsideration;

        public KnownFolders(KnownFoldersConfiguration knownFoldersConfiguration)
        {
            this._mainAssembly = knownFoldersConfiguration.MainAssembly;
            this._versionConsideration = knownFoldersConfiguration.VersionConsideration;

            this.LocalAppData = this.GetFromSpecialFolder(Environment.SpecialFolder.LocalApplicationData);
            this.RoamingAppData = this.GetFromSpecialFolder(Environment.SpecialFolder.ApplicationData);
        }

        public IFileProvider LocalAppData { get; }

        public IFileProvider RoamingAppData { get; }

        private static IFileProvider GetFromFolderPath(string folderPath)
        {
            _ = Directory.CreateDirectory(folderPath);

            return new PhysicalFileProvider(folderPath);
        }

        private IFileProvider GetFromSpecialFolder(Environment.SpecialFolder specialFolder)
        {
            var folderPath = Environment.GetFolderPath(specialFolder);

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(this._mainAssembly.Location);

            folderPath = Path.Combine(folderPath, fileVersionInfo.CompanyName);
            folderPath = Path.Combine(folderPath, fileVersionInfo.ProductName);

            switch (this._versionConsideration)
            {
                case KnownFolderVersionConsideration.None:
                    break;

                case KnownFolderVersionConsideration.Major:
                    folderPath = Path.Combine(folderPath, this._mainAssembly.GetName().Version.ToString(1));
                    break;

                case KnownFolderVersionConsideration.MajorMinor:
                    folderPath = Path.Combine(folderPath, this._mainAssembly.GetName().Version.ToString(2));
                    break;

                case KnownFolderVersionConsideration.MajorMinorBuild:
                    folderPath = Path.Combine(folderPath, this._mainAssembly.GetName().Version.ToString(3));
                    break;

                case KnownFolderVersionConsideration.MajorMinorBuildRevision:
                    folderPath = Path.Combine(folderPath, this._mainAssembly.GetName().Version.ToString(4));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(this._versionConsideration));
            }

            return GetFromFolderPath(folderPath);
        }
    }
}
