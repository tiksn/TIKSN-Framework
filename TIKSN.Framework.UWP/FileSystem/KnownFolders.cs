using Microsoft.Extensions.FileProviders;
using Windows.Storage;

namespace TIKSN.FileSystem
{
    public class KnownFolders : IKnownFolders
    {
        public KnownFolders()
        {
            LocalAppData = new PhysicalFileProvider(ApplicationData.Current.LocalFolder.Path);
            RoamingAppData = new PhysicalFileProvider(ApplicationData.Current.RoamingFolder.Path);
        }

        public IFileProvider LocalAppData { get; }

        public IFileProvider RoamingAppData { get; }
    }
}
