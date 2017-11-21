using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.MongoDB.Repositories
{
  /// <summary>
  /// Interface for manage data of MongoDb collection.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TKey">The type of the primary key.</typeparam>
  /// <seealso cref="IMongoRepository" />
  public interface IMongoRepository<TEntity, TKey>
      where TEntity : class
  {
    /// <summary>
    /// Gets the database used.
    /// </summary>
    IMongoDatabase Database { get; }

    /// <summary>
    /// Gets the name of the collection.
    /// </summary>
    string CollectionName { get; }

    /// <summary>
    /// Give access to MongoDb driver Collection.
    /// </summary>
    IMongoCollection<TEntity> Collection { get; }

    #region Count
    /// <summary>
    /// Counts the records in MongoDb collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection.
    /// </returns>
    Task<int> Count(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="predicate" />.
    /// </returns>
    Task<int> Count(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="filter" />.
    /// </returns>
    Task<int> Count(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Count Long
    /// <summary>
    /// Counts the records in MongoDb collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection.
    /// </returns>
    Task<long> CountLong(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="predicate" />.
    /// </returns>
    Task<long> CountLong(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Counts the records in MongoDb collection that match the specified <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of record in collection which match the <paramref name="filter" />.
    /// </returns>
    Task<long> CountLong(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region Exists
    /// <summary>
    /// Check if row with <paramref name="id"/> exists.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><b>true</b> if row exists in database, otherwise <b>false</b>.</returns>
    Task<bool> Exists(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Check if row corresponding to <paramref name="predicate" /> exists.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <b>true</b> if row exists in database, otherwise <b>false</b>.
    /// </returns>
    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Check if row corresponding to <paramref name="filter" /> exists.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <b>true</b> if row exists in database, otherwise <b>false</b>.
    /// </returns>
    Task<bool> Exists(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken));
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
    Task<bool> Delete(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Deletes all row corresponding to the specified <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Number of rows are deleted.
    /// </returns>
    Task<long> Delete(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Deletes all row corresponding to the specified <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Number of rows are deleted.</returns>
    Task<long> Delete(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken));
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
    Task<TEntity> GetSingle(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetSingle(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken));
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
    Task<TEntity> GetSingleOrDefault(TKey id, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" /> or default value.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" /> or default value.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetSingleOrDefault(FilterDefinition<TEntity> filter, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken));
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
    Task<TEntity> GetFirst(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetFirst(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default(CancellationToken));
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
    Task<TEntity> GetFirstOrDefault(TKey id, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="predicate" /> or default value.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets one MongoDb document corresponding to <paramref name="filter" /> or default value.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The corresponding entities.
    /// </returns>
    Task<TEntity> GetFirstOrDefault(FilterDefinition<TEntity> filter, TEntity defaultValue = default(TEntity), CancellationToken cancellationToken = default(CancellationToken));
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
    Task<IEnumerable<TEntity>> GetMany(FindOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets all MongoDb documents corresponding to <paramref name="predicate" />.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The list of corresponding entities.
    /// </returns>
    Task<IEnumerable<TEntity>> GetMany(Expression<Func<TEntity, bool>> predicate, FindOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Gets all MongoDb documents corresponding to <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The list of corresponding entities.
    /// </returns>
    Task<IEnumerable<TEntity>> GetMany(FilterDefinition<TEntity> filter, FindOptions<TEntity> options = null, CancellationToken cancellationToken = default(CancellationToken));
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
    Task<IEnumerable<TEntity>> Create(IEnumerable<TEntity> entites, InsertManyOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Creates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The new entity stored in collection.
    /// </returns>
    Task<TEntity> Create(TEntity entity, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
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
    Task<IEnumerable<TEntity>> UpdateMany<TSpecificEntity>(IEnumerable<TSpecificEntity> entites, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        where TSpecificEntity : Entity<TKey>, TEntity;
    /// <summary>
    /// Updates the MongoDb document corresponding to the specified <paramref name="entity" /> key.
    /// </summary>
    /// <typeparam name="TSpecificEntity">The type of the specific entity.</typeparam>
    /// <param name="entity">The new entity.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The new entity stored in collection.
    /// </returns>
    Task<TEntity> Update<TSpecificEntity>(TSpecificEntity entity, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        where TSpecificEntity : Entity<TKey>, TEntity;
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
    Task<TEntity> Update(TKey id, TEntity entity, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region ExecuteAggregate
    /// <summary>
    /// Executes the aggregation pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="IEnumerable{BsonDocument}"/> return by aggregation operation.</returns>
    Task<IEnumerable<BsonDocument>> ExecuteAggregate(IEnumerable<string> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Executes the aggregation pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="IEnumerable{TResult}"/> return by aggregation operation.</returns>
    Task<IEnumerable<TResult>> ExecuteAggregate<TResult>(IEnumerable<string> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region ExecuteMapReduce
    /// <summary>
    /// Maps the reduce.
    /// </summary>
    /// <param name="mapFunction">The map function.</param>
    /// <param name="reduceFunction">The reduce function.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The <see cref="IAsyncCursor{BsonDocument}" /> with result rows.
    /// </returns>
    Task<IEnumerable<BsonDocument>> ExecuteMapReduce(string mapFunction, string reduceFunction, MapReduceOptions<TEntity, BsonDocument> options = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// Executes maps-reduce on collection.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="mapFunction">The map function.</param>
    /// <param name="reduceFunction">The reduce function.</param>
    /// <param name="otpions">The otpions of execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The <see cref="IAsyncCursor{TResult}" /> with result rows.
    /// </returns>
    Task<IEnumerable<TResult>> ExecuteMapReduce<TResult>(string mapFunction, string reduceFunction, MapReduceOptions<TEntity, TResult> otpions = null, CancellationToken cancellationToken = default(CancellationToken));
    #endregion

    #region ExecuteScript
    /// <summary>
    /// Executes javascript script to collection.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rusult of script execution.</returns>
    Task<BsonValue> Execute(string script, CancellationToken cancellationToken = default(CancellationToken));
    #endregion
  }
}
