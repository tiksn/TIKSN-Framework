using LiteDB;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using TIKSN.Configuration;
using TIKSN.FileSystem;

namespace TIKSN.Settings
{
    public class FileSettingsService : ISettingsService
    {
        private readonly IPartialConfiguration<FileSettingsServiceOptions> _configuration;
        private readonly IKnownFolders _knownFolders;

        public FileSettingsService(IKnownFolders knownFolders, IPartialConfiguration<FileSettingsServiceOptions> configuration)
        {
            _knownFolders = knownFolders ?? throw new ArgumentNullException(nameof(knownFolders));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public T GetLocalSetting<T>(string name, T defaultValue)
        {
            return Apply(_knownFolders.LocalAppData, name, defaultValue, GetterProcessor);
        }

        public T GetRoamingSetting<T>(string name, T defaultValue)
        {
            return Apply(_knownFolders.RoamingAppData, name, defaultValue, GetterProcessor);
        }

        public IReadOnlyCollection<string> ListLocalSetting()
        {
            return ListNames(_knownFolders.LocalAppData);
        }

        public IReadOnlyCollection<string> ListRoamingSetting()
        {
            return ListNames(_knownFolders.RoamingAppData);
        }

        public void RemoveLocalSetting(string name)
        {
            Apply<object>(_knownFolders.LocalAppData, name, null, RemovalProcessor);
        }

        public void RemoveRoamingSetting(string name)
        {
            Apply<object>(_knownFolders.RoamingAppData, name, null, RemovalProcessor);
        }

        public void SetLocalSetting<T>(string name, T value)
        {
            Apply(_knownFolders.LocalAppData, name, value, SetterProcessor);
        }

        public void SetRoamingSetting<T>(string name, T value)
        {
            Apply(_knownFolders.RoamingAppData, name, value, SetterProcessor);
        }

        private T Apply<T>(IFileProvider fileProvider, string name, T value, Func<BsonDocument, string, T, T> processor)
        {
            using (var db = GetDatabase(fileProvider, out LiteCollection<BsonDocument> settingsCollection, out BsonDocument bsonDocument))
            {
                var result = processor(bsonDocument, name, value);

                settingsCollection.Update(bsonDocument);

                return result;
            }
        }

        private LiteDatabase GetDatabase(IFileProvider fileProvider, out LiteCollection<BsonDocument> settingsCollection, out BsonDocument bsonDocument)
        {
            var fileInfo = fileProvider.GetFileInfo(_configuration.GetConfiguration().RelativePath);

            var connectionString = new ConnectionString
            {
                Filename = fileInfo.PhysicalPath
            };

            var db = new LiteDatabase(connectionString);

            settingsCollection = db.GetCollection("Settings");
            bsonDocument = settingsCollection.FindOne(x => true);
            if (bsonDocument == null)
            {
                bsonDocument = new BsonDocument();
                settingsCollection.Insert(bsonDocument);
            }

            return db;
        }

        private T GetterProcessor<T>(BsonDocument document, string name, T defaultValue)
        {
            if (document.TryGetValue(name, out BsonValue bsonValue))
            {
                if (bsonValue.IsNull)
                    return defaultValue;

                return (T)bsonValue.RawValue;
            }

            return defaultValue;
        }

        private IReadOnlyCollection<string> ListNames(IFileProvider fileProvider)
        {
            using (var db = GetDatabase(fileProvider, out LiteCollection<BsonDocument> settingsCollection, out BsonDocument bsonDocument))
            {
                return bsonDocument.Keys.Where(n => !string.Equals(n, "_id", StringComparison.OrdinalIgnoreCase)).ToArray();
            }
        }

        private T RemovalProcessor<T>(BsonDocument document, string name, T value)
        {
            document.Remove(name);

            return value;
        }

        private T SetterProcessor<T>(BsonDocument document, string name, T value)
        {
            document.Set(name, new BsonValue(value));
            return value;
        }
    }
}