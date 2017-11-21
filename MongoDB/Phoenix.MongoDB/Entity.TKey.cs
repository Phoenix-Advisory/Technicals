using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.MongoDB
{
  /// <summary>
  /// Base class for Entities stored in MongoDb.
  /// </summary>
  /// <typeparam name="Tkey">The type of the key.</typeparam>
  public class Entity<Tkey>
  {
    /// <summary>
    /// Gets or sets the identifier of the MongoDb row.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [BsonId]
    public Tkey Id { get; set; }

    /// <summary>
    /// Gets or sets the extra elements which are not define in entity but present in MongoDb row.
    /// </summary>
    [BsonExtraElements]
    public IDictionary<string, object> ExtraElements { get; set; }
  }
}
