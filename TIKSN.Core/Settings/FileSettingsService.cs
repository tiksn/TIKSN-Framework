using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.Extensions.FileProviders;
using TIKSN.Configuration;
using TIKSN.FileSystem;

namespace TIKSN.Settings
{
    public class FileSettingsService : ISettingsService
    {
        private readonly IPartialConfiguration<FileSettingsServiceOptions> _configuration;
        private readonly IKnownFolders _knownFolders;

        public FileSettingsService(IKnownFolders knownFolders,
            IPartialConfiguration<FileSettingsServiceOptions> configuration)
        {
            this._knownFolders = knownFolders ?? throw new ArgumentNullException(nameof(knownFolders));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public T GetLocalSetting<T>(string name, T defaultValue) => this.Apply(this._knownFolders.LocalAppData, name,
            defaultValue, GetterProcessor);

        public T GetRoamingSetting<T>(string name, T defaultValue) => this.Apply(this._knownFolders.RoamingAppData,
            name, defaultValue, GetterProcessor);

        public IReadOnlyCollection<string> ListLocalSetting() => this.ListNames(this._knownFolders.LocalAppData);

        public IReadOnlyCollection<string> ListRoamingSetting() => this.ListNames(this._knownFolders.RoamingAppData);

        public void RemoveLocalSetting(string name) =>
            this.Apply<object>(this._knownFolders.LocalAppData, name, null, RemovalProcessor);

        public void RemoveRoamingSetting(string name) =>
            this.Apply<object>(this._knownFolders.RoamingAppData, name, null, RemovalProcessor);

        public void SetLocalSetting<T>(string name, T value) =>
            this.Apply(this._knownFolders.LocalAppData, name, value, SetterProcessor);

        public void SetRoamingSetting<T>(string name, T value) =>
            this.Apply(this._knownFolders.RoamingAppData, name, value, SetterProcessor);

        private T Apply<T>(IFileProvider fileProvider, string name, T value, Func<BsonDocument, string, T, T> processor)
        {
            using var db = this.GetDatabase(fileProvider, out var settingsCollection, out var bsonDocument);
            var result = processor(bsonDocument, name, value);

            _ = settingsCollection.Update(bsonDocument);

            return result;
        }

        private LiteDatabase GetDatabase(IFileProvider fileProvider,
            out ILiteCollection<BsonDocument> settingsCollection, out BsonDocument bsonDocument)
        {
            var fileInfo = fileProvider.GetFileInfo(this._configuration.GetConfiguration().RelativePath);

            var connectionString = new ConnectionString { Filename = fileInfo.PhysicalPath };

            var db = new LiteDatabase(connectionString);

            settingsCollection = db.GetCollection("Settings");
            bsonDocument = settingsCollection.FindById(Guid.Empty);
            if (bsonDocument == null)
            {
                bsonDocument = new BsonDocument
                {
                    { "_id", Guid.Empty }
                };
                _ = settingsCollection.Insert(bsonDocument);
            }

            return db;
        }

        private static T GetterProcessor<T>(BsonDocument document, string name, T defaultValue)
        {
            if (document.TryGetValue(name, out var bsonValue))
            {
                if (bsonValue.IsNull)
                {
                    return defaultValue;
                }

                var type = typeof(T);
                var typeCode = Type.GetTypeCode(type);

                void CheckType(BsonType bsonType)
                {
                    if (bsonValue.Type != bsonType)
                    {
                        throw new ArgumentException(
                            $"Expected type was '{bsonType}' whitch is not matching with actual type '{bsonValue.Type}'.");
                    }
                }

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        CheckType(BsonType.Boolean);
                        return (T)(object)bsonValue.AsBoolean;

                    case TypeCode.Byte:
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                        CheckType(BsonType.Int32);
                        return (T)(object)bsonValue.AsInt32;

                    case TypeCode.DateTime:
                        CheckType(BsonType.DateTime);
                        return (T)(object)bsonValue.AsDateTime;

                    case TypeCode.Decimal:
                    case TypeCode.UInt64:
                        CheckType(BsonType.Decimal);
                        return (T)(object)bsonValue.AsDecimal;

                    case TypeCode.Double:
                    case TypeCode.Single:
                        CheckType(BsonType.Double);
                        return (T)(object)bsonValue.AsDouble;

                    case TypeCode.Int64:
                    case TypeCode.UInt32:
                        CheckType(BsonType.Int64);
                        return (T)(object)bsonValue.AsInt64;

                    case TypeCode.String:
                        CheckType(BsonType.String);
                        return (T)(object)bsonValue.AsString;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Empty:
                    case TypeCode.Object:
                    default:
                        if (type == typeof(Guid))
                        {
                            CheckType(BsonType.Guid);
                            return (T)(object)bsonValue.AsGuid;
                            break;
                        }
                        else
                        {
                            throw new NotSupportedException("Type is not supported.");
                        }
                }
            }

            return defaultValue;
        }

        private IReadOnlyCollection<string> ListNames(IFileProvider fileProvider)
        {
            using var db = this.GetDatabase(fileProvider, out var settingsCollection, out var bsonDocument);
            return bsonDocument.Keys.Where(n => !string.Equals(n, "_id", StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        private static T RemovalProcessor<T>(BsonDocument document, string name, T value)
        {
            _ = document.Remove(name);

            return value;
        }

        private static T SetterProcessor<T>(BsonDocument document, string name, T value)
        {
            object valueObject = value;

            var type = typeof(T);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    document[name] = new BsonValue((bool)valueObject);
                    break;

                case TypeCode.Byte:
                    document[name] = new BsonValue((byte)valueObject);
                    break;

                case TypeCode.Char:
                    document[name] = new BsonValue((char)valueObject);
                    break;

                case TypeCode.DateTime:
                    document[name] = new BsonValue((DateTime)valueObject);
                    break;

                case TypeCode.Decimal:
                    document[name] = new BsonValue((decimal)valueObject);
                    break;

                case TypeCode.Double:
                    document[name] = new BsonValue((double)valueObject);
                    break;

                case TypeCode.Empty:
                    document[name] = BsonValue.Null;
                    break;

                case TypeCode.Int16:
                    document[name] = new BsonValue((short)valueObject);
                    break;

                case TypeCode.Int32:
                    document[name] = new BsonValue((int)valueObject);
                    break;

                case TypeCode.Int64:
                    document[name] = new BsonValue((long)valueObject);
                    break;

                case TypeCode.SByte:
                    document[name] = new BsonValue((sbyte)valueObject);
                    break;

                case TypeCode.Single:
                    document[name] = new BsonValue((float)valueObject);
                    break;

                case TypeCode.String:
                    document[name] = new BsonValue((string)valueObject);
                    break;

                case TypeCode.UInt16:
                    document[name] = new BsonValue((ushort)valueObject);
                    break;

                case TypeCode.UInt32:
                    document[name] = new BsonValue((uint)valueObject);
                    break;

                case TypeCode.UInt64:
                    document[name] = new BsonValue((decimal)valueObject);
                    break;
                case TypeCode.DBNull:
                case TypeCode.Object:
                default:
                    if (type == typeof(Guid))
                    {
                        document[name] = new BsonValue((Guid)valueObject);
                        break;
                    }
                    else
                    {
                        throw new NotSupportedException("Type is not supported.");
                    }
            }

            return value;
        }
    }
}
