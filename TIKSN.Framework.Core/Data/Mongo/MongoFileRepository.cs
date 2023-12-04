using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace TIKSN.Data.Mongo;

public class MongoFileRepository<TIdentity, TMetadata> : IFileRepository, IFileRepository<TIdentity>,
    IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
{
    private readonly GridFSBucket<TIdentity> _bucket;
    private readonly GridFSBucket _bucketRaw;

    public MongoFileRepository(IMongoDatabaseProvider mongoDatabaseProvider, string bucketName)
    {
        if (mongoDatabaseProvider == null)
        {
            throw new ArgumentNullException(nameof(mongoDatabaseProvider));
        }

        if (string.IsNullOrEmpty(bucketName))
        {
            throw new ArgumentNullException(nameof(bucketName));
        }

        var database = mongoDatabaseProvider.GetDatabase();

        this._bucket = new GridFSBucket<TIdentity>(database, new GridFSBucketOptions { BucketName = bucketName });

        this._bucketRaw = new GridFSBucket(database, new GridFSBucketOptions { BucketName = bucketName });
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var fileInfo = await (await this._bucket
            .FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Filename, path), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleAsync(cancellationToken).ConfigureAwait(true);

        await this._bucket.DeleteAsync(fileInfo.Id, cancellationToken).ConfigureAwait(true);
    }

    public async Task<IFile> DownloadAsync(string path, CancellationToken cancellationToken)
    {
        var fileInfo = await (await this._bucket
            .FindAsync(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Filename, path), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleAsync(cancellationToken).ConfigureAwait(true);

        var content = await this._bucket.DownloadAsBytesAsync(fileInfo.Id, null, cancellationToken).ConfigureAwait(true);

        return new File(fileInfo.Filename, content);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken) => this._bucket
        .Find(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Filename, path), cancellationToken: cancellationToken)
        .AnyAsync(cancellationToken);

    public Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken) =>
        this._bucketRaw.UploadFromBytesAsync(path, content, null, cancellationToken);

    public async Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(TIdentity id,
        CancellationToken cancellationToken)
    {
        var fileInfo = await this._bucket.Find(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken)
            .SingleAsync(cancellationToken).ConfigureAwait(true);

        return new FileInfo<TIdentity, TMetadata>(id, fileInfo.Filename,
            BsonSerializer.Deserialize<TMetadata>(fileInfo.Metadata));
    }

    public async Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(TIdentity id,
        CancellationToken cancellationToken)
    {
        var content = await this._bucket.DownloadAsBytesAsync(id, null, cancellationToken).ConfigureAwait(true);

        var fileInfo = await this._bucket.Find(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken)
            .SingleAsync(cancellationToken).ConfigureAwait(true);

        return new File<TIdentity, TMetadata>(id, fileInfo.Filename,
            BsonSerializer.Deserialize<TMetadata>(fileInfo.Metadata), content);
    }

    public Task UploadAsync(TIdentity id, string path, byte[] content, TMetadata metadata,
        CancellationToken cancellationToken) =>
        this._bucket.UploadFromBytesAsync(id, path, content,
            new GridFSUploadOptions { Metadata = metadata.ToBsonDocument() }, cancellationToken);

    public Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken) =>
        this._bucket.DeleteAsync(id, cancellationToken);

    public async Task<IFile<TIdentity>> DownloadByIdAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var content = await this._bucket.DownloadAsBytesAsync(id, null, cancellationToken).ConfigureAwait(true);

        var fileInfo = await this._bucket.Find(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken)
            .SingleAsync(cancellationToken).ConfigureAwait(true);

        return new File<TIdentity>(id, fileInfo.Filename, content);
    }

    public Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken) =>
        this._bucket.Find(Builders<GridFSFileInfo<TIdentity>>.Filter.Eq(item => item.Id, id), cancellationToken: cancellationToken).AnyAsync(cancellationToken);

    public Task UploadByIdAsync(TIdentity id, string path, byte[] content, CancellationToken cancellationToken) =>
        this._bucket.UploadFromBytesAsync(id, path, content, null, cancellationToken);
}
