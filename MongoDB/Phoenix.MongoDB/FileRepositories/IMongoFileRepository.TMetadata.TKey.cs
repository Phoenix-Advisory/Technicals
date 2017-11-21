using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.MongoDB.FileRepositories
{
  /// <summary>
  /// Interface for MongoDb file management, <typeparamref name="TMetadata"/> will be stored as file metadata.
  /// </summary>
  /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <seealso cref="IMongoFileRepository" />
  public interface IMongoFileRepository<TMetadata, TKey>
  {
    /// <summary>
    /// Gets the database connection used.
    /// </summary>
    IMongoDatabase Database { get; }
    /// <summary>
    /// Gets the name of the bucket.
    /// </summary>
    string BucketName { get; }

    /// <summary>
    /// Gets the native MongoDb bucket.
    /// </summary>
    IGridFSBucket Bucket { get; }

    #region Create
    /// <summary>
    /// Adds file to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> Create(byte[] content, string filename, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Update
    /// <summary>
    /// Updates file to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="content">The content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<FileInfo<TMetadata>> Update(ObjectId id, byte[] content, string filename, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Updates file to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="content">The content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> Update(TKey id, byte[] content, string filename, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Delete
    /// <summary>
    /// Deletes file in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<bool> Delete(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Deletes file in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<bool> Delete(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Deletes all files corresponding to <paramref name="predicate"/> asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<long> Delete(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Deletes all files corresponding to <paramref name="filter"/> asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<long> Delete(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Count
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="predicate"/> asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The count of element corresponding to the <paramref name="predicate"/>.</returns>
    Task<int> Count(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="filter" /> asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The count of element corresponding to the <paramref name="filter" />.
    /// </returns>
    Task<int> Count(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Count Long
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="predicate"/> asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The count of element corresponding to the <paramref name="predicate"/>.</returns>
    Task<long> CountLong(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="filter" /> asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The count of element corresponding to the <paramref name="filter" />.
    /// </returns>
    Task<long> CountLong(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Exists
    /// <summary>
    /// Check asynchronously if file with <paramref name="id"/> exists in MongoDb Bucket.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><b>true</b> if bucket contains the <paramref name="id"/>, otherwise <b>false</b>.</returns>
    Task<bool> Exists(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Check asynchronously if file with <paramref name="id"/> exists in MongoDb Bucket.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><b>true</b> if bucket contains the <paramref name="id"/>, otherwise <b>false</b>.</returns>
    Task<bool> Exists(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region GetSingle
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetSingle(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetSingle(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetSingle(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<FileInfo<TMetadata>> GetSingle(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region GetSingle
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetSingleOrDefault(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetSingleOrDefault(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetSingleOrDefault(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<FileInfo<TMetadata>> GetSingleOrDefault(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region GetFirst
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetFirst(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetFirst(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetFirst(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<FileInfo<TMetadata>> GetFirst(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region GetFirstOrDefault
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetFirstOrDefault(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetFirstOrDefault(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> GetFirstOrDefault(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<FileInfo<TMetadata>> GetFirstOrDefault(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region GetMany
    /// <summary>
    /// Finds all the records corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<IEnumerable<FileInfo<TMetadata>>> GetMany(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Finds all the records corresponding to <paramref name="filter" /> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The informations stored in bucket.
    /// </returns>
    Task<IEnumerable<FileInfo<TMetadata>>> GetMany(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region GetContent
    /// <summary>
    /// Gets the file content.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The byte array  with content of file.</returns>
    Task<byte[]> GetContent(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets the file content.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The byte array with content of file.</returns>
    Task<byte[]> GetContent(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region GetStream
    /// <summary>
    /// Gets the content stream reader from MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<StreamReader> GetStream(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets the content stream reader from MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<StreamReader> GetStream(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Update metadata
    /// <summary>
    /// Updates metadata to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<FileInfo<TMetadata>> UpdateMetadata(ObjectId id, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Updates metadata to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    Task<FileInfo<TMetadata>> UpdateMetadata(TKey id, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken));
    #endregion
  }
}
