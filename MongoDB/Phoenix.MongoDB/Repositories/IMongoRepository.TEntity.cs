using System;

namespace Phoenix.MongoDB.Repositories
{
  /// <summary>
  /// Interface for manage data of MongoDb collection.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public interface IMongoRepository<TEntity> : IMongoRepository<TEntity, Guid>
      where TEntity : class
  {
  }
}
