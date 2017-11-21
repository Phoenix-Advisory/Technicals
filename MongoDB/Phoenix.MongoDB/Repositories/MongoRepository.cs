using MongoDB.Bson;

namespace Phoenix.MongoDB.Repositories
{
  /// <summary>
  /// No type-safe Mongo Repository
  /// </summary>
  /// <seealso cref="MongoRepository&lt;BsonDocument, Object&gt;" />
  public class MongoRepository : MongoRepository<BsonDocument, object>, IMongoRepository
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="collectionName">Name of the collection.</param>
    public MongoRepository(string connectionName, string collectionName) :
        base(connectionName, collectionName)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository" /> class.
    /// </summary>                                  
    public MongoRepository() :
        base()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>  
    public MongoRepository(string connectionName) :
        base(connectionName)
    { }       
  }
}
