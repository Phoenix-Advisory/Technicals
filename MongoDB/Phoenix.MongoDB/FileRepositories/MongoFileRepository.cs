using MongoDB.Bson;

namespace Phoenix.MongoDB.FileRepositories
{
  /// <summary>
  /// Manage MongoDB File Bucket
  /// </summary>
  /// <seealso cref="IMongoFileRepository&lt;BsonDocument, Object&gt;" />
  /// <seealso cref="MongoFileRepository&lt;BsonDocument, Object&gt;" />
  /// <seealso cref="IMongoFileRepository" />
  public class MongoFileRepository : MongoFileRepository<BsonDocument, object>, IMongoFileRepository, IMongoFileRepository<BsonDocument, object>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository" /> class.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(ConnectionManager connectionManager)
        : base(connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(string connectionName, ConnectionManager connectionManager)
        : base(connectionName, connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="bucketName">Name of the bucket.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(string connectionName, string bucketName, ConnectionManager connectionManager)
        : base(connectionName, bucketName, connectionManager)
    { }
  }
}
