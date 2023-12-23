using Microsoft.Extensions.FileProviders;

namespace TIKSN.FileSystem;

public interface IKnownFolders
{
    IFileProvider LocalApplicationData { get; }
    IFileProvider RoamingApplicationData { get; }
}
