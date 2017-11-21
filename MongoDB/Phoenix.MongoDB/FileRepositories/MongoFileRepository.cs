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
    public MongoFileRepository()
        : base()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>  
    public MongoFileRepository(string connectionName)
        : base(connectionName)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="bucketName">Name of the bucket.</param>    
    public MongoFileRepository(string connectionName, string bucketName)
        : base(connectionName, bucketName)
    { }
  }
}
