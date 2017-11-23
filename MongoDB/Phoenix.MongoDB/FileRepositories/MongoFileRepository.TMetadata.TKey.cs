using Autofac;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Phoenix.Core.DependencyInjection;
using Phoenix.Core.ParameterGuard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.MongoDB.FileRepositories
{
  /// <summary>
  /// Manage MongoDb Bucket.
  /// </summary>
  /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <seealso cref="IMongoFileRepository{TMetadata, TKey}" />
  public class MongoFileRepository<TMetadata, TKey> : IMongoFileRepository<TMetadata, TKey>
  {
    private readonly string _ConnectionName;
    private readonly string _BucketName;           
    private readonly object _LockObj = new object();
    private IMongoDatabase _Database;
    private IGridFSBucket _Bucket;                                             
    private IMongoCollection<FileInfo<TMetadata>> _FileCollection;
    private readonly ConnectionManager _ConnectionManager;

    /// <summary>
    /// Gets the database connection used.
    /// </summary>
    public virtual IMongoDatabase Database
    {
      get
      {
        if (_Database == null)
        {
          lock (this)
          {
            if (_Database == null)
            {
              _Database = _ConnectionManager.GetDatabase(_ConnectionName);
            }
          }
        }
        return _Database;
      }
    } 

    /// <summary>
    /// Gets the file metadata.
    /// </summary>
    public virtual IMongoCollection<FileInfo<TMetadata>> FileMetadata
    {
      get
      {
        if (_FileCollection == null)
        {
          lock (_LockObj)
          {
            if (_FileCollection == null)
            {
              _FileCollection = Database.GetCollection<FileInfo<TMetadata>>($"{_BucketName}.files");
            }
          }
        }
        return _FileCollection;
      }
    }

    /// <summary>
    /// Gets the name of the bucket.
    /// </summary>
    public virtual string BucketName { get { return Bucket.Options.BucketName; } }

    /// <summary>
    /// Gets the native MongoDb bucket.
    /// </summary>
    public virtual IGridFSBucket Bucket
    {
      get
      {
        if (_Bucket == null)
        {
          lock (this)
          {
            if (_Bucket == null)
            {
              _Bucket = new GridFSBucket(Database, new GridFSBucketOptions { BucketName = _BucketName, ReadPreference = ReadPreference.Primary, WriteConcern = Database.Settings.WriteConcern, ReadConcern = Database.Settings.ReadConcern });
            }
          }
        }
        return _Bucket;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata, TKey}" /> class.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(ConnectionManager connectionManager) :
        this(ConnectionManager.GetDatabaseSettingsName(typeof(TMetadata)), ConnectionManager.GetBucketName(typeof(TMetadata)), connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata, TKey}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(string connectionName, ConnectionManager connectionManager) :
        this(connectionName, ConnectionManager.GetBucketName(typeof(TMetadata)), connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFileRepository{TMetadata, TKey}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="bucketName">Name of the bucket.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoFileRepository(string connectionName, string bucketName, ConnectionManager connectionManager)
    {
      Guard.IsNotNull(connectionManager, nameof(connectionManager));
      Guard.IsNotNullOrWhiteSpace(connectionName, nameof(connectionName));
      Guard.IsNotNullOrWhiteSpace(bucketName, nameof(bucketName));

      _ConnectionManager = connectionManager;
      _ConnectionName = connectionName;
      _BucketName = bucketName;
    } 

    #region Create
    /// <summary>
    /// Adds file to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual async Task<FileInfo<TMetadata>> Create(byte[] content, string filename, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (content == null || content.Count() == 0)
        throw new ArgumentNullException("Content cannot be null");
      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("File name cannot be null or empty");
      if (metadata == null)
        throw new ArgumentNullException("Metadata cannot be null");

      cancellationToken.ThrowIfCancellationRequested();

      ObjectId id = await Bucket.UploadFromBytesAsync(
              filename,
              content,
              new GridFSUploadOptions
              {
                Metadata = metadata.ToBsonDocument()
              },
              cancellationToken)
          .ConfigureAwait(false);

      FileInfo<TMetadata> res = null;

      Stopwatch timeoutChecker = Stopwatch.StartNew();
      do
      {
        await Task.Delay(250).ConfigureAwait(false);
        res = await GetSingleOrDefault(id).ConfigureAwait(false);
      }
      while (res == null && timeoutChecker.ElapsedMilliseconds < 300000);

      return res;
    }
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
    public virtual async Task<FileInfo<TMetadata>> Update(ObjectId id, byte[] content, string filename, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (content == null || content.Count() == 0)
        throw new ArgumentNullException("Content cannot be null");
      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("File name cannot be null or empty");
      if (metadata == null)
        throw new ArgumentNullException("Metadata cannot be null");

      cancellationToken.ThrowIfCancellationRequested();

      bool exist = await Exists(id, cancellationToken).ConfigureAwait(false);
      if (exist)
      {
        await Delete(id, cancellationToken).ConfigureAwait(false);
      }
      
      await Bucket.UploadFromBytesAsync(
          id,
          filename,
          content,
          new GridFSUploadOptions
          {
            Metadata = metadata.ToBsonDocument()
          },
          cancellationToken
      ).ConfigureAwait(false);

      return await GetSingle(id).ConfigureAwait(false);
    }
    /// <summary>
    /// Updates file to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="content">The content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual async Task<FileInfo<TMetadata>> Update(TKey id, byte[] content, string filename, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (content == null || content.Count() == 0)
      {
        throw new ArgumentNullException("Content cannot be null");
      }
      if (string.IsNullOrEmpty(filename))
      {
        throw new ArgumentNullException("File name cannot be null or empty");
      }
      if (metadata == null)
      {
        throw new ArgumentNullException("Metadata cannot be null");
      }

      cancellationToken.ThrowIfCancellationRequested();

      GridFSFileInfo found = await Bucket.Find(GetFilter(id)).FirstAsync(cancellationToken).ConfigureAwait(false);
      return await Update(found.Id, content, filename, metadata, cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Delete
    /// <summary>
    /// Deletes file in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<bool> Delete(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();

      GridFSFileInfo found = await Bucket.Find(GetFilter(id)).FirstAsync(cancellationToken).ConfigureAwait(false);
      if (found != null)
      {
        await Bucket.DeleteAsync(found.Id, cancellationToken).ConfigureAwait(false);
        return true;
      }

      return false;
    }
    /// <summary>
    /// Deletes file in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<bool> Delete(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();

      GridFSFileInfo found = await Bucket.Find(GetFilter(id)).FirstAsync(cancellationToken).ConfigureAwait(false);
      if (found != null)
      {
        await Bucket.DeleteAsync(found.Id, cancellationToken).ConfigureAwait(false);
        return true;
      }

      return false;
    }
    /// <summary>
    /// Deletes all files corresponding to <paramref name="predicate"/> asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<long> Delete(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      long count = 0;
      await Bucket.Find(GetFilter(predicate)).ForEachAsync(async x =>
      {
        await Bucket.DeleteAsync(x.Id, cancellationToken).ConfigureAwait(false);
        count++;
      },
          cancellationToken
      ).ConfigureAwait(false);
      return count;
    }

    /// <summary>
    /// Deletes all files corresponding to <paramref name="filter"/> asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<long> Delete(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      long count = 0;
      await Bucket.Find(filter).ForEachAsync(async x =>
      {
        await Bucket.DeleteAsync(x.Id, cancellationToken).ConfigureAwait(false);
        count++;
      },
          cancellationToken
      ).ConfigureAwait(false);
      return count;
    }
    #endregion

    #region Count
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="predicate"/> asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The count of element corresponding to the <paramref name="predicate"/>.</returns>
    public virtual Task<int> Count(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Count(GetFilter(predicate), cancellationToken);
    }
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="filter" /> asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The count of element corresponding to the <paramref name="filter" />.
    /// </returns>
    public virtual async Task<int> Count(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      IList<GridFSFileInfo> matches = await Bucket.Find(filter).ToListAsync(cancellationToken).ConfigureAwait(false);
      return matches.Count;
    }
    #endregion

    #region Count Long
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="predicate"/> asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The count of element corresponding to the <paramref name="predicate"/>.</returns>
    public virtual Task<long> CountLong(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return CountLong(GetFilter(predicate), cancellationToken);
    }
    /// <summary>
    /// Get the count of element corresponding to the <paramref name="filter" /> asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The count of element corresponding to the <paramref name="filter" />.
    /// </returns>
    public virtual async Task<long> CountLong(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      IList<GridFSFileInfo> matches = await Bucket.Find(filter).ToListAsync(cancellationToken).ConfigureAwait(false);
      return matches.LongCount();
    }
    #endregion

    #region Exists
    /// <summary>
    /// Check asynchronously if file with <paramref name="id"/> exists in MongoDb Bucket.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><b>true</b> if bucket contains the <paramref name="id"/>, otherwise <b>false</b>.</returns>
    public virtual Task<bool> Exists(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Bucket.Find(GetFilter(id)).AnyAsync(cancellationToken);
    }
    /// <summary>
    /// Check asynchronously if file with <paramref name="id"/> exists in MongoDb Bucket.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><b>true</b> if bucket contains the <paramref name="id"/>, otherwise <b>false</b>.</returns>
    public virtual Task<bool> Exists(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Bucket.Find(GetFilter(id)).AnyAsync(cancellationToken);
    }
    #endregion

    #region GetSingle
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetSingle(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingle(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetSingle(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingle(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetSingle(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingle(GetFilter(predicate), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<FileInfo<TMetadata>> GetSingle(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      //GridFSFileInfo result = await Bucket.Find(filter).SingleAsync(cancellationToken).ConfigureAwait(false);

      return await Convert(await Bucket.Find(filter).SingleAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
    }
    #endregion

    #region GetSingleOrDefault
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetSingleOrDefault(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingleOrDefault(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetSingleOrDefault(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingleOrDefault(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetSingleOrDefault(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingleOrDefault(GetFilter(predicate), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<FileInfo<TMetadata>> GetSingleOrDefault(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      //GridFSFileInfo result = await Bucket.Find(filter).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

      return await Convert(await Bucket.Find(filter).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
    }
    #endregion

    #region GetFirst
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetFirst(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirst(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetFirst(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirst(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetFirst(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirst(GetFilter(predicate), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<FileInfo<TMetadata>> GetFirst(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      
      return await Convert(await Bucket.Find(filter).FirstAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
    }
    #endregion

    #region GetFirstOrDefault
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetFirstOrDefault(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirstOrDefault(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="id"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetFirstOrDefault(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirstOrDefault(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<FileInfo<TMetadata>> GetFirstOrDefault(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirstOrDefault(GetFilter(predicate), cancellationToken);
    }
    /// <summary>
    /// Finds a record corresponding to <paramref name="filter"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<FileInfo<TMetadata>> GetFirstOrDefault(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      return await Convert(await Bucket.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
    }
    #endregion

    #region GetMany
    /// <summary>
    /// Finds all the records corresponding to <paramref name="predicate"/> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual Task<IEnumerable<FileInfo<TMetadata>>> GetMany(Expression<Func<FileInfo<TMetadata>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetMany(GetFilter(predicate), cancellationToken);
    }
    /// <summary>
    /// Finds all the records corresponding to <paramref name="filter" /> in MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The informations stored in bucket.
    /// </returns>
    public virtual async Task<IEnumerable<FileInfo<TMetadata>>> GetMany(FilterDefinition<GridFSFileInfo> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      IList<GridFSFileInfo> result = await Bucket.Find(filter).ToListAsync(cancellationToken).ConfigureAwait(false);

      IList<FileInfo<TMetadata>> res = new List<FileInfo<TMetadata>>();
      foreach (GridFSFileInfo fi in result)
      {
        FileInfo<TMetadata> tmp = await Convert(
                     await Bucket.Find(GetFilter(fi.Id)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
                 ).ConfigureAwait(false);

        res.Add(tmp);
      }
      return res;
    }
    #endregion

    #region GetContent
    /// <summary>
    /// Gets the file content.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The byte array  with content of file.</returns>
    public virtual async Task<byte[]> GetContent(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();

      FileInfo<TMetadata> found = await GetFirst(id, cancellationToken).ConfigureAwait(false);
      return await GetContent(found.Id, cancellationToken).ConfigureAwait(false);
    }
    /// <summary>
    /// Gets the file content.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The byte array with content of file.</returns>
    public virtual Task<byte[]> GetContent(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Bucket.DownloadAsBytesAsync(id, new GridFSDownloadOptions { CheckMD5 = true }, cancellationToken);
    }
    #endregion

    #region GetStream
    /// <summary>
    /// Gets the content stream reader from MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<StreamReader> GetStream(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();

      FileInfo<TMetadata> found = await GetFirst(id, cancellationToken).ConfigureAwait(false);
      return await GetStream(found.Id, cancellationToken).ConfigureAwait(false);
    }
    /// <summary>
    /// Gets the content stream reader from MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<StreamReader> GetStream(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();

      MemoryStream result = new MemoryStream();
      await Bucket.DownloadToStreamAsync(id, result, new GridFSDownloadOptions { CheckMD5 = true }, cancellationToken).ConfigureAwait(false);
      result.Seek(0, SeekOrigin.Begin);
      return new StreamReader(result);
    }
    #endregion

    private static FilterDefinition<GridFSFileInfo> GetFilter(TKey id)
    {
      return Builders<GridFSFileInfo>.Filter.Eq("Metadata._id", id);
    }
    private static FilterDefinition<GridFSFileInfo> GetFilter(ObjectId id)
    {
      return Builders<GridFSFileInfo>.Filter.Eq("_id", id);
    }

    private static BsonDocument GetFilter(Expression<Func<FileInfo<TMetadata>, bool>> predicate)
    {
      var serializerRegistry = BsonSerializer.SerializerRegistry;
      var documentSerializer = serializerRegistry.GetSerializer<FileInfo<TMetadata>>();
      return BsonDocument.Parse(Builders<FileInfo<TMetadata>>.Filter.Where(predicate).Render(documentSerializer, serializerRegistry).ToJson());
    }

    private static Task<FileInfo<TMetadata>> Convert(GridFSFileInfo gridInfo)
    {
      FileInfo<TMetadata> res = null;

      if (gridInfo == null)
      {
        return Task.FromResult(res);
      }

      res = new FileInfo<TMetadata>
      {
        Id = gridInfo.Id,
        Filename = gridInfo.Filename,
        Length = gridInfo.Length,
        ChunkSizeBytes = gridInfo.ChunkSizeBytes,
        MD5 = gridInfo.MD5,
        UploadDateTime = gridInfo.UploadDateTime,
        Metadata = BsonSerializer.Deserialize<TMetadata>(gridInfo.Metadata)
      };

      return Task.FromResult(res);
    }

    #region ICollectionManager
    /// <summary>
    /// Creates the collection.
    /// </summary>
    /// <param name="maxDocuments">The maximum documents.</param>
    /// <param name="maxSize">The maximum size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task CreateCollection(long? maxDocuments, long? maxSize, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (!await CheckCollectionExists(BucketName + ".files").ConfigureAwait(false))
      {
        await Database.GetCollection<BsonDocument>(BucketName + ".files").Indexes.CreateOneAsync("{ filename : 1, uploadDate : 1 }").ConfigureAwait(false);
      }
      if (!await CheckCollectionExists(BucketName + ".chunks").ConfigureAwait(false))
      {
        await Database.GetCollection<BsonDocument>(BucketName + ".chunks").Indexes.CreateOneAsync("{ files_id : 1, n : 1 }", new CreateIndexOptions { Unique = true }).ConfigureAwait(false);
      }
    }

    private async Task<bool> CheckCollectionExists(string name)
    {
      var colCursor = await Database.ListCollectionsAsync(new ListCollectionsOptions { Filter = Builders<BsonDocument>.Filter.Eq("name", name) }).ConfigureAwait(false);
      var col = await colCursor.SingleOrDefaultAsync().ConfigureAwait(false);
      return col != null;
    }

    /// <summary>
    /// Drops the collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task DropCollection(CancellationToken cancellationToken = default(CancellationToken))
    {
      return Bucket.DropAsync(cancellationToken);
    }
    #endregion

    #region Update metadata
    /// <summary>
    /// Updates metadata to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<FileInfo<TMetadata>> UpdateMetadata(ObjectId id, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (metadata == null)
        throw new ArgumentNullException("Metadata cannot be null");

      cancellationToken.ThrowIfCancellationRequested();

      bool exist = await Exists(id, cancellationToken).ConfigureAwait(false);
      if (exist)
      {
        await FileMetadata.UpdateOneAsync(x => x.Id == id, Builders<FileInfo<TMetadata>>.Update.Set(x => x.Metadata, metadata)).ConfigureAwait(false);
        
        return await GetSingle(id).ConfigureAwait(false);
      }

      return null;
    }
    /// <summary>
    /// Updates metadata to MongoDb Bucket asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="metadata">The metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The informations stored in bucket.</returns>
    public virtual async Task<FileInfo<TMetadata>> UpdateMetadata(TKey id, TMetadata metadata, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (metadata == null)
        throw new ArgumentNullException("Metadata cannot be null");

      cancellationToken.ThrowIfCancellationRequested();

      GridFSFileInfo found = await Bucket.Find(GetFilter(id)).FirstAsync(cancellationToken).ConfigureAwait(false);
      return await UpdateMetadata(found.Id, metadata, cancellationToken).ConfigureAwait(false);
    }
    #endregion
  }
}
