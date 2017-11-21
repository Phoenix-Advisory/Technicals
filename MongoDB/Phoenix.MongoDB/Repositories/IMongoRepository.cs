using MongoDB.Bson;     

namespace Phoenix.MongoDB.Repositories
{
  /// <summary>
  /// Interface for manage data of MongoDb collection.
  /// </summary>
  public interface IMongoRepository : IMongoRepository<BsonDocument, object>
  {

  }
}
