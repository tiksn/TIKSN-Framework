using Microsoft.Extensions.FileProviders;

namespace TIKSN.FileSystem;

public interface IKnownFolders
{
    IFileProvider LocalAppData { get; }
    IFileProvider RoamingAppData { get; }
}
