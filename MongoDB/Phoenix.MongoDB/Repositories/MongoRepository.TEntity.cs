using System;

namespace Phoenix.MongoDB.Repositories
{
  /// <summary>
  /// Typed Mongo Repository with Guid as primary key.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <seealso cref="MongoRepository{TEntity, Guid}" />
  /// <seealso cref="IMongoRepository{TEntity}" />
  /// <seealso cref="IMongoRepository{TEntity, Guid}" />
  /// <seealso cref="MongoRepository{TEntity, Guid}" />
  public class MongoRepository<TEntity> : MongoRepository<TEntity, Guid>, IMongoRepository<TEntity>, IMongoRepository<TEntity, Guid>
        where TEntity : class
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
    /// Initializes a new instance of the <see cref="MongoRepository{TEntity}" /> class.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(ConnectionManager connectionManager) :
        base(connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository{TEntity}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(string connectionName, ConnectionManager connectionManager) :
        base(connectionName, connectionManager)
    { }        
  }
}
