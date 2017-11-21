using MongoDB.Bson;

namespace Phoenix.MongoDB.FileRepositories
{
  /// <summary>
  /// Interface for MongoDb file management.
  /// </summary>
  public interface IMongoFileRepository : IMongoFileRepository<BsonDocument, object>
  {
  }
}
