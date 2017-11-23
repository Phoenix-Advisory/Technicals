using Autofac;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.Operations;
using Phoenix.Core.Comparer;
using Phoenix.Core.DependencyInjection;
using Phoenix.Core.ParameterGuard;
using Phoenix.Core.Reflection;
using Phoenix.Core.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.MongoDB.Repositories
{
  /// <summary>
  /// Type safe Mongo Repository with generic primary key type.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <seealso cref="IMongoRepository{TEntity, TKey}" />
  public class MongoRepository<TEntity, TKey> : IMongoRepository<TEntity, TKey>
      where TEntity : class
  {
    private readonly string _ConnectionName;
    private readonly string _CollectionName;
    private IMongoDatabase _Database;
    private IMongoCollection<TEntity> _Collection;
    private readonly object _LockObj = new object();
    private readonly ConnectionManager _ConnectionManager;

    /// <summary>
    /// The default options for Update operations
    /// </summary>
    protected static readonly UpdateOptions _DefaultUpdateOption = new UpdateOptions { IsUpsert = true };

    /// <summary>
    /// Gets the database used.
    /// </summary>
    public virtual IMongoDatabase Database
    {
      get
      {
        if (_Database == null)
        {
          lock (_LockObj)
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
    /// Gets the name of the collection.
    /// </summary>
    public virtual string CollectionName { get { return Collection.CollectionNamespace.CollectionName; } }

    /// <summary>
    /// Give access to MongoDb driver Collection.
    /// </summary>
    public virtual IMongoCollection<TEntity> Collection
    {
      get
      {
        if (_Collection == null)
        {
          lock (_LockObj)
          {
            if (_Collection == null)
            {
              _Collection = Database.GetCollection<TEntity>(_CollectionName);
            }
          }
        }
        return _Collection;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository{TEntity, TKey}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="collectionName">Name of the collection.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(string connectionName, string collectionName, ConnectionManager connectionManager)
    {
      Guard.IsNotNull(connectionManager, nameof(connectionManager));
      Guard.IsNotNullOrWhiteSpace(connectionName, nameof(connectionName));
      Guard.IsNotNullOrWhiteSpace(collectionName, nameof(collectionName));

      _ConnectionManager = connectionManager;
      _ConnectionName = connectionName;
      _CollectionName = collectionName;           
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository{TEntity, TKey}" /> class.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(ConnectionManager connectionManager) :
        this(ConnectionManager.GetDatabaseSettingsName(typeof(TEntity)), ConnectionManager.GetCollectionName(typeof(TEntity)), connectionManager)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository{TEntity, TKey}" /> class.
    /// </summary>
    /// <param name="connectionName">Name of the connection.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public MongoRepository(string connectionName, ConnectionManager connectionManager) :
        this(connectionName, ConnectionManager.GetCollectionName(typeof(TEntity)), connectionManager)
    { }          

    #region Count
    /// <summary>
    /// Counts the records in MongoDb collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection.
    /// </returns>
    public virtual Task<int> Count(CancellationToken cancellationToken = default(CancellationToken))
    {
      return Count(x => true, cancellationToken);
    }
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="predicate" />.
    /// </returns>
    public virtual async Task<int> Count(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      long result = await CountLong(predicate, cancellationToken).ConfigureAwait(false);
      return (int)result;
    }
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="filter" />.
    /// </returns>
    public virtual async Task<int> Count(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      long result = await CountLong(filter, cancellationToken).ConfigureAwait(false);
      return (int)result;
    }
    #endregion

    #region Count Long
    /// <summary>
    /// Counts the records in MongoDb collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection.
    /// </returns>
    public virtual Task<long> CountLong(CancellationToken cancellationToken = default(CancellationToken))
    {
      return CountLong(x => true, cancellationToken);
    }
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="predicate" />.
    /// </returns>
    public virtual Task<long> CountLong(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.CountAsync(predicate, cancellationToken: cancellationToken);
    }
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="filter" />.
    /// </returns>
    public virtual Task<long> CountLong(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.CountAsync(filter, cancellationToken: cancellationToken);
    }
    #endregion

    #region Exists
    /// <summary>
    /// Check if row with <paramref name="id"/> exists.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><b>true</b> if row exists in database, otherwise <b>false</b>.</returns>
    public virtual Task<bool> Exists(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.Find(GetFilter(id)).AnyAsync(cancellationToken);
    }
    /// <summary>
    /// Check if row corresponding to <paramref name="predicate" /> exists.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <b>true</b> if row exists in database, otherwise <b>false</b>.
    /// </returns>
    public virtual Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.Find(predicate).AnyAsync(cancellationToken);
    }
    /// <summary>
    /// Check if row corresponding to <paramref name="filter" /> exists.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <b>true</b> if row exists in database, otherwise <b>false</b>.
    /// </returns>
    public virtual Task<bool> Exists(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.Find(filter).AnyAsync(cancellationToken);
    }
    #endregion

    #region Delete
    /// <summary>
    /// Deletes all row corresponding to the specified <paramref name="id" />.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <b>true</b> if row corresponding to <paramref name="id"/> is deleted, otherwise <b>false</b>.
    /// </returns>
    public virtual async Task<bool> Delete(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      DeleteResult result = await Collection.DeleteOneAsync(GetFilter(id), cancellationToken).ConfigureAwait(false);
      return result.DeletedCount == 1;
    }
    /// <summary>
    /// Deletes all row corresponding to the specified <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of rows are deleted.
    /// </returns>
    public virtual async Task<long> Delete(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      DeleteResult result = await Collection.DeleteManyAsync(predicate, cancellationToken).ConfigureAwait(false);
      return result.DeletedCount;
    }
    /// <summary>
    /// Deletes all row corresponding to the specified <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Number of rows are deleted.</returns>
    public virtual async Task<long> Delete(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      DeleteResult result = await Collection.DeleteManyAsync(filter, cancellationToken).ConfigureAwait(false);
      return result.DeletedCount;
    }
    #endregion

    #region GetSingle
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="id" />.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    public virtual Task<TEntity> GetSingle(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingle(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    public virtual Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.Find(predicate).SingleAsync(cancellationToken);
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    public virtual Task<TEntity> GetSingle(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.Find(filter).SingleAsync(cancellationToken);
    }
    #endregion

    #region GetSingleOrDefault
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="id" /> or default value.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    public virtual Task<TEntity> GetSingleOrDefault(TKey id, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetSingleOrDefault(GetFilter(id), defaultValue, cancellationToken);
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" /> or default value.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    public virtual async Task<TEntity> GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      TEntity result = await Collection.Find(predicate).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
      return result ?? defaultValue;
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" /> or default value.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<TEntity> GetSingleOrDefault(FilterDefinition<TEntity> filter, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      TEntity result = await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
      return result ?? defaultValue;
    }
    #endregion

    #region GetFirst
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="id" />.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Task<TEntity> GetFirst(TKey id, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirst(GetFilter(id), cancellationToken);
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    public virtual Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.Find(predicate).FirstAsync(cancellationToken);
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Task<TEntity> GetFirst(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Collection.Find(filter).FirstAsync(cancellationToken);
    }
    #endregion

    #region GetFirstOrDefault
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="id" /> or default value.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Task<TEntity> GetFirstOrDefault(TKey id, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetFirstOrDefault(GetFilter(id), defaultValue, cancellationToken);
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" /> or default value.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<TEntity> GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken))
    {
      TEntity result = await Collection.Find(predicate).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
      return result ?? defaultValue;
    }
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" /> or default value.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<TEntity> GetFirstOrDefault(FilterDefinition<TEntity> filter, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      TEntity result = await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
      return result ?? defaultValue;
    }
    #endregion

    #region GetMany
    /// <summary>
    /// Gets all MongoDb documents.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The list of entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Task<IEnumerable<TEntity>> GetMany(FindOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      return GetMany(x => true, options, cancellationToken);
    }
    /// <summary>
    /// Gets all MongoDb documents corresponding to <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The list of corresponding entities.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<IEnumerable<TEntity>> GetMany(Expression<Func<TEntity, bool>> predicate, FindOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      IAsyncCursor<TEntity> cursor = await Collection.FindAsync(predicate, options, cancellationToken).ConfigureAwait(false);
      return await cursor.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    /// <summary>
    /// Gets all MongoDb documents corresponding to <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The list of corresponding entities.
    /// </returns>
    public virtual async Task<IEnumerable<TEntity>> GetMany(FilterDefinition<TEntity> filter, FindOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      IAsyncCursor<TEntity> cursor = await Collection.FindAsync(filter, options, cancellationToken).ConfigureAwait(false);
      return await cursor.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Create
    /// <summary>
    /// Creates the specified entites.
    /// </summary>
    /// <param name="entites">The entites.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The list of new entities stored in collection.
    /// </returns>
    public virtual async Task<IEnumerable<TEntity>> Create(IEnumerable<TEntity> entites, InsertManyOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      await Collection.InsertManyAsync(entites, options).ConfigureAwait(false);
      return entites;
    }
    /// <summary>
    /// Creates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The new entity stored in collection.
    /// </returns>
    public virtual async Task<TEntity> Create(TEntity entity, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      await Collection.InsertOneAsync(entity, options).ConfigureAwait(false);
      return entity;
    }
    #endregion

    #region Update
    /// <summary>
    /// Updates the MongoDb documents.
    /// </summary>
    /// <param name="entites">The list of entites.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The list of new entities stored in collection.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<IEnumerable<TEntity>> UpdateMany<TSpecificEntity>(IEnumerable<TSpecificEntity> entites, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        where TSpecificEntity : Entity<TKey>, TEntity
    {
      foreach (TSpecificEntity entity in entites)
      {
        await Update(entity, options, cancellationToken).ConfigureAwait(false);
      }

      return entites;
    }
    /// <summary>
    /// Updates the MongoDb document corresponding to the specified <paramref name="id" />.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="entity">The new entity.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The new entity stored in collection.
    /// </returns>
    public virtual async Task<TEntity> Update(TKey id, TEntity entity, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (id == null)
      {
        await Create(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      else
      {
        await Collection.ReplaceOneAsync(GetFilter(id), entity, options ?? _DefaultUpdateOption, cancellationToken).ConfigureAwait(false);
      }

      return entity;
    }

    /// <summary>
    /// Updates the MongoDb document corresponding to the specified <paramref name="entity" />.
    /// </summary>
    /// <typeparam name="TSpecificEntity">The type of the specific entity.</typeparam>
    /// <param name="entity">The new entity.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The new entity stored in collection.
    /// </returns>
    public virtual Task<TEntity> Update<TSpecificEntity>(TSpecificEntity entity, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        where TSpecificEntity : Entity<TKey>, TEntity
    {
      return Update(entity.Id, entity, options, cancellationToken);
    }
    #endregion

    #region ExecuteAggregate
    /// <summary>
    /// Executes the aggregation pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="IEnumerable{BsonDocument}"/> return by aggregation operation.</returns>
    public virtual Task<IEnumerable<BsonDocument>> ExecuteAggregate(IEnumerable<string> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      return ExecuteAggregate<BsonDocument>(pipeline, options, cancellationToken);
    }

    /// <summary>
    /// Executes the aggregation pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="IEnumerable{TResult}"/> return by aggregation operation.</returns>
    public virtual async Task<IEnumerable<TResult>> ExecuteAggregate<TResult>(IEnumerable<string> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      IEnumerable<BsonDocument> parsedPipeline = pipeline.Select(x => BsonDocument.Parse(x));
      IAsyncCursor<TResult> cursor = await Collection.AggregateAsync<TResult>(parsedPipeline.ToArray(), options ?? new AggregateOptions { AllowDiskUse = true, UseCursor = true }, cancellationToken).ConfigureAwait(false);
      return await cursor.ToListAsync().ConfigureAwait(false);
    }
    #endregion

    #region ExecuteMapReduce
    /// <summary>
    /// Maps the reduce.
    /// </summary>
    /// <param name="mapFunction">The map function.</param>
    /// <param name="reduceFunction">The reduce function.</param>
    /// <param name="options">The otpions.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="IAsyncCursor{BsonDocument}"/> with result rows.</returns>
    public virtual Task<IEnumerable<BsonDocument>> ExecuteMapReduce(string mapFunction, string reduceFunction, MapReduceOptions<TEntity, BsonDocument> options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      return ExecuteMapReduce<BsonDocument>(mapFunction, reduceFunction, options, cancellationToken);
    }

    /// <summary>
    /// Executes maps-reduce on collection.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="mapFunction">The map function.</param>
    /// <param name="reduceFunction">The reduce function.</param>
    /// <param name="options">The otpions of execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="IAsyncCursor{TResult}"/> with result rows.</returns>
    public virtual async Task<IEnumerable<TResult>> ExecuteMapReduce<TResult>(string mapFunction, string reduceFunction, MapReduceOptions<TEntity, TResult> options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      BsonJavaScript bsonMap = new BsonJavaScript(mapFunction);
      BsonJavaScript bsonReduce = new BsonJavaScript(reduceFunction);
      IAsyncCursor<TResult> cursor = await Collection.MapReduceAsync(bsonMap, bsonReduce, options, cancellationToken).ConfigureAwait(false);
      return await cursor.ToListAsync().ConfigureAwait(false);
    }
    #endregion

    #region ExecuteScript
    /// <summary>
    /// Executes javascript script to collection.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rusult of script execution.</returns>
    public virtual async Task<BsonValue> Execute(string script, CancellationToken cancellationToken = default(CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      BsonJavaScript function = new BsonJavaScript(script);
      EvalOperation op = new EvalOperation(Database.DatabaseNamespace, function, null);
      using (var writeBinding = new WritableServerBinding(Database.Client.Cluster))
      {
        return await op.ExecuteAsync(writeBinding, cancellationToken).ConfigureAwait(false);
      }
    }
    #endregion

    private static FilterDefinition<TEntity> GetFilter(TKey id)
    {
      return Builders<TEntity>.Filter.Eq("_id", id);
    }

    #region CollectionManagement
    /// <summary>
    /// Creates the collection.
    /// </summary>
    /// <param name="maxDocuments">The maximum documents.</param>
    /// <param name="maxSize">The maximum size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task CreateCollection(long? maxDocuments, long? maxSize, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (!await CheckCollectionExists().ConfigureAwait(false))
      {
        await Database.CreateCollectionAsync(
           CollectionName,
           new CreateCollectionOptions
           {
             ValidationLevel = DocumentValidationLevel.Off,
             AutoIndexId = true,
             Capped = (maxDocuments.HasValue || maxSize.HasValue),
             MaxDocuments = maxDocuments,
             MaxSize = maxSize
           },
           cancellationToken
       ).ConfigureAwait(false);
      }
    }

    /// <summary> 
    /// Checks if the collection exists. 
    /// </summary> 
    /// <returns></returns> 
    public async Task<bool> CheckCollectionExists()
    {
      var colCursor = await Database.ListCollectionsAsync(
                        new ListCollectionsOptions
                        {
                          Filter = Builders<BsonDocument>.Filter.Eq("name", CollectionName)
                        }
                      ).ConfigureAwait(false);
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
      return Database.DropCollectionAsync(CollectionName, cancellationToken);
    }
    #endregion
  }
}
