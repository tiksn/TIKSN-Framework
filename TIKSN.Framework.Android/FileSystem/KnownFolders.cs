using Android.App;
using Microsoft.Extensions.FileProviders;

namespace TIKSN.FileSystem
{
    public class KnownFolders : IKnownFolders
    {
        public IFileProvider LocalAppData => new PhysicalFileProvider(Application.Context.DataDir.AbsolutePath);

        public IFileProvider RoamingAppData => new PhysicalFileProvider(Application.Context.ApplicationContext.DataDir.AbsolutePath);
    }
}