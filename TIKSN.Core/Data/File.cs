using System;

namespace TIKSN.Data
{
    public class File : FileInfo, IFile
    {
        public File(string path, byte[] content) : base(path)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public string Path { get; }

        public byte[] Content { get; }
    }

    public class File<TIdentity> : FileInfo<TIdentity>, IFile<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        public File(TIdentity id, string path, byte[] content) : base(id, path)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public byte[] Content { get; }
    }

    public class File<TIdentity, TMetadata> : FileInfo<TIdentity, TMetadata>, IFile<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
    {
        public File(TIdentity id, string path, TMetadata metadata, byte[] content) : base(id, path, metadata)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public byte[] Content { get; }
    }
}