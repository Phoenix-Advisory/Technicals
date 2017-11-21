using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Phoenix.MongoDB.Configuration
{
  /// <summary>
  /// MongoDb Database connection configuration.
  /// </summary>
  public class MongoDbSetting
  {
    /// <summary>
    /// Initializes the <see cref="MongoDbSetting" /> class.
    /// </summary>
    static MongoDbSetting()
    {
      BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

      ConventionPack pack = new ConventionPack()
                                {
                                    new EnumRepresentationConvention(BsonType.String)
                                };
      ConventionRegistry.Register("EnumStringConvention", pack, t => true);
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the database.
    /// </summary>
    /// <value>
    /// The database name.
    /// </value>
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the database name used for authentication.
    /// </summary>
    /// <value>
    /// The database name used for authentication.
    /// </value>
    public string AuthenticationDatabase { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>
    /// The username.
    /// </value>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the list of MongoDb servers.
    /// </summary>
    /// <value>
    /// The list servers of MongoDb servers in cluster.
    /// </value>
    public IList<MongoDbServer> Servers { get; set; } = new List<MongoDbServer>();

    /// <summary>
    /// Gets or sets the replicaset name.
    /// </summary>
    /// <value>
    /// The replicaset name.
    /// </value>
    public string ReplicaSet { get; set; }

    /// <summary>
    /// Gets or sets the connection timeout.
    /// </summary>
    /// <value>
    /// The connection timeout.
    /// </value>
    public string ConnectTimeout { get; set; } = "00:00:15";

    /// <summary>
    /// Gets or sets the read concern level.
    /// </summary>
    /// <value>
    /// The read concern level.
    /// </value>
    public ReadConcernLevel ReadConcernLevel { get; set; } = ReadConcernLevel.Majority;

    /// <summary>
    /// Gets or sets the read preference mode.
    /// </summary>
    /// <value>
    /// The read preference mode.
    /// </value>
    public ReadPreferenceMode ReadPreferenceMode { get; set; } = ReadPreferenceMode.SecondaryPreferred;

    /// <summary>
    /// Gets or sets the write concern mode.
    /// </summary>
    /// <value>
    /// The write concern mode.
    /// </value>
    public string WriteConcernMode { get; set; } = "majority";

    /// <summary>
    /// Gets the default size of the bucket chunk in bytes.
    /// </summary>
    /// <value>
    /// The default size of the bucket chunk in bytes.
    /// </value>
    public int DefaultChunkSize { get; set; }
  }
}
