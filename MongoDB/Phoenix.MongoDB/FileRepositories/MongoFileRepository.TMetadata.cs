using Microsoft.Extensions.Logging;
using System;         

namespace Phoenix.MongoDB.FileRepositories
{
  /// <summary>
  /// Manage MongoDb Bucket.
  /// </summary>
  /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
  /// <seealso cref="IMongoFileRepository{TMetadata, Guid}" />
  /// <seealso cref="MongoFileRepository{TMetadata, Guid}" />
  /// <seealso cref="IMongoFileRepository{TMetadata}" />
  public class MongoFileRepository<TMetadata>
      : MongoFileRepository<TMetadata, Guid>, IMongoFileRepository<TMetadata>, IMongoFileRepository<TMetadata, Guid>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata}" /> class.
    /// </summary>                                  
    public MongoFileRepository()
        : base()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>  
    public MongoFileRepository(string connectionName)
        : base(connectionName)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="bucketName">Name of the bucket.</param>  
    public MongoFileRepository(string connectionName, string bucketName)
        : base(connectionName, bucketName)
    { }
  }
}
