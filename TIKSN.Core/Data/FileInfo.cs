using System;

namespace TIKSN.Data
{
    public class FileInfo : IFileInfo
    {
        public FileInfo(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Argument is null, empty or whitespace.", nameof(path));
            }

            this.Path = path;
        }

        public string Path { get; }
    }

    public class FileInfo<TIdentity> : FileInfo, IFileInfo<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        public FileInfo(TIdentity id, string path) : base(path) => this.ID = id;

        public TIdentity ID { get; }
    }

    public class FileInfo<TIdentity, TMetadata> : FileInfo<TIdentity>, IFileInfo<TIdentity, TMetadata>
        where TIdentity : IEquatable<TIdentity>
    {
        public FileInfo(TIdentity id, string path, TMetadata metadata) : base(id, path) => this.Metadata = metadata;

        public TMetadata Metadata { get; }
    }
}
