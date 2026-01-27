using Microsoft.Extensions.FileProviders;

namespace TIKSN.FileSystem;

public interface IKnownFolders
{
    public IFileProvider LocalApplicationData { get; }
    public IFileProvider RoamingApplicationData { get; }
}
