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
      : MongoFileRepository<TMetadata, Guid>, IMongoFileRepository<TMetadata>
    where TMetadata : class
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata}" /> class.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(ConnectionManager connectionManager)
        : base(connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(string connectionName, ConnectionManager connectionManager)
        : base(connectionName, connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="bucketName">Name of the bucket.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(string connectionName, string bucketName, ConnectionManager connectionManager)
        : base(connectionName, bucketName, connectionManager)
    { }
  }
}
