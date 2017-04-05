using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
	public class MongoRepository<T> : IMongoRepository<T>
	{
		private readonly IMongoCollection<T> collection;

		public MongoRepository(IMongoDatabaseProvider mongoDatabaseProvider, string collectionName)
		{
			var database = mongoDatabaseProvider.GetDatabase();
			collection = database.GetCollection<T>(collectionName);
		}
	}
}