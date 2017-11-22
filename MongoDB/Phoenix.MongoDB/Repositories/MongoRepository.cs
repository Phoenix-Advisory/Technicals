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
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(string connectionName, string collectionName, ConnectionManager connectionManager) :
        base(connectionName, collectionName, connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository" /> class.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(ConnectionManager connectionManager) :
        base(connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(string connectionName, ConnectionManager connectionManager) :
        base(connectionName, connectionManager)
    { }       
  }
}
