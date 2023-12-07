using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace TIKSN.Data.Mongo;

public class MongoFileRepository<TIdentity, TMetadata> : IFileRepository, IFileRepository<TIdentity>,
    IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
{
    private readonly GridFSBucket<TIdentity> bucket;
    private readonly GridFSBucket bucketRaw;

    public MongoFileRepository(IMongoDatabaseProvider mongoDatabaseProvider, string bucketName)
    {
        ArgumentNullException.ThrowIfNull(mongoDatabaseProvider);

        if (string.IsNullOrEmpty(bucketName))
        {
            throw new ArgumentNullException(nameof(bucketName));
        }

        var database = mongoDatabaseProvider.GetDatabase();

        this.bucket = new GridFSBucket<TIdentity>(database, new GridFSBucketOptions { BucketName = bucketName });

        this.bucketRaw = new GridFSBucket(database, new GridFSBucketOptions { BucketName = bucketName });
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var fileInfo = await (await this.bucket
            .FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Filename, path), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleAsync(cancellationToken).ConfigureAwait(false);

        await this.bucket.DeleteAsync(fileInfo.Id, cancellationToken).ConfigureAwait(false);
    }

    public Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.bucket.DeleteAsync(id, cancellationToken);

    public async Task<IFile> DownloadAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var fileInfo = await (await this.bucket
            .FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Filename, path), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleAsync(cancellationToken).ConfigureAwait(false);

        var content = await this.bucket.DownloadAsBytesAsync(fileInfo.Id, options: null, cancellationToken).ConfigureAwait(false);

        return new File(fileInfo.Filename, content);
    }

    public async Task<IFile<TIdentity>> DownloadByIdAsync(
        TIdentity id,
        CancellationToken cancellationToken)
    {
        var content = await this.bucket.DownloadAsBytesAsync(id, options: null, cancellationToken).ConfigureAwait(false);

        var fileInfo = await (await this.bucket.FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleAsync(cancellationToken).ConfigureAwait(false);

        return new File<TIdentity>(id, fileInfo.Filename, content);
    }

    public async Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(
        TIdentity id,
        CancellationToken cancellationToken)
    {
        var fileInfo = await (await this.bucket.FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleAsync(cancellationToken).ConfigureAwait(false);

        return new FileInfo<TIdentity, TMetadata>(id, fileInfo.Filename,
            BsonSerializer.Deserialize<TMetadata>(fileInfo.Metadata));
    }

    public async Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(
        TIdentity id,
        CancellationToken cancellationToken)
    {
        var content = await this.bucket.DownloadAsBytesAsync(id, options: null, cancellationToken).ConfigureAwait(false);

        var fileInfo = await (await this.bucket.FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleAsync(cancellationToken).ConfigureAwait(false);

        return new File<TIdentity, TMetadata>(id, fileInfo.Filename,
            BsonSerializer.Deserialize<TMetadata>(fileInfo.Metadata), content);
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken) => await (await this.bucket
        .FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Filename, path), cancellationToken: cancellationToken).ConfigureAwait(false))
        .AnyAsync(cancellationToken).ConfigureAwait(false);

    public async Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken) =>
        await (await this.bucket.FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken).ConfigureAwait(false)).AnyAsync(cancellationToken).ConfigureAwait(false);

    public Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken) =>
        this.bucketRaw.UploadFromBytesAsync(path, content, options: null, cancellationToken);

    public Task UploadAsync(TIdentity id, string path, byte[] content, TMetadata metadata,
        CancellationToken cancellationToken) =>
        this.bucket.UploadFromBytesAsync(id, path, content,
            new GridFSUploadOptions { Metadata = metadata.ToBsonDocument() }, cancellationToken);

    public Task UploadByIdAsync(TIdentity id, string path, byte[] content, CancellationToken cancellationToken) =>
        this.bucket.UploadFromBytesAsync(id, path, content, options: null, cancellationToken);
}
