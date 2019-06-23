using Microsoft.Extensions.FileProviders;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TIKSN.FileSystem
{
    public class KnownFolders : IKnownFolders
    {
        private readonly Assembly _mainAssembly;
        private readonly KnownFolderVersionConsideration _versionConsideration;

        public KnownFolders(KnownFoldersConfiguration knownFoldersConfiguration)
        {
            _mainAssembly = knownFoldersConfiguration.MainAssembly;
            _versionConsideration = knownFoldersConfiguration.VersionConsideration;

            LocalAppData = GetFromSpecialFolder(Environment.SpecialFolder.LocalApplicationData);
            RoamingAppData = GetFromSpecialFolder(Environment.SpecialFolder.ApplicationData);
        }

        public IFileProvider LocalAppData { get; }

        public IFileProvider RoamingAppData { get; }

        private IFileProvider GetFromFolderPath(string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            return new PhysicalFileProvider(folderPath);
        }

        private IFileProvider GetFromSpecialFolder(Environment.SpecialFolder specialFolder)
        {
            var folderPath = Environment.GetFolderPath(specialFolder);

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(_mainAssembly.Location);

            folderPath = Path.Combine(folderPath, fileVersionInfo.CompanyName);
            folderPath = Path.Combine(folderPath, fileVersionInfo.ProductName);

            switch (_versionConsideration)
            {
                case KnownFolderVersionConsideration.None:
                    break;

                case KnownFolderVersionConsideration.Major:
                    folderPath = Path.Combine(folderPath, _mainAssembly.GetName().Version.ToString(1));
                    break;

                case KnownFolderVersionConsideration.MajorMinor:
                    folderPath = Path.Combine(folderPath, _mainAssembly.GetName().Version.ToString(2));
                    break;

                case KnownFolderVersionConsideration.MajorMinorBuild:
                    folderPath = Path.Combine(folderPath, _mainAssembly.GetName().Version.ToString(3));
                    break;

                case KnownFolderVersionConsideration.MajorMinorBuildRevision:
                    folderPath = Path.Combine(folderPath, _mainAssembly.GetName().Version.ToString(4));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_versionConsideration));
            }

            return GetFromFolderPath(folderPath);
        }
    }
}