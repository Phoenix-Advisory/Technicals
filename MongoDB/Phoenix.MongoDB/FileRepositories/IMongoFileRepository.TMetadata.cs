using System;

namespace Phoenix.MongoDB.FileRepositories
{
  /// <summary>
  /// Interface for MongoDb file management, <typeparamref name="TMetadata"/> will be stored as file metadata.
  /// </summary>
  /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
  /// <seealso cref="IMongoFileRepository{TMetadata, TKey}" />
  public interface IMongoFileRepository<TMetadata> : IMongoFileRepository<TMetadata, Guid>
  {
  }
}
