using MongoDB.Bson;
using System.Threading.Tasks;

namespace Phoenix.MongoDB.EntityUpgrader
{
  /// <summary>
  /// Interface for Entity upgrader processes.
  /// </summary>
  public interface IEntityUpgrader
  {
    /// <summary>
    /// Upgrade BsonDocument to new schema version. 
    /// </summary>
    /// <param name="document">Document in old schema.</param>
    /// <returns>Document transformed to new schema.</returns>
    Task<BsonDocument> Upgrade(BsonDocument document);
  }
}
