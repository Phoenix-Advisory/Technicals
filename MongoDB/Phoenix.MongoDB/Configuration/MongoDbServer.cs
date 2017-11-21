namespace Phoenix.MongoDB.Configuration
{
  /// <summary>
  /// MongoDb Server Configuration.
  /// </summary>
  public class MongoDbServer
  {
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    /// <value>
    /// The IP address or DNS Name.
    /// </value>
    public string Address { get; set; } = "localhost";
    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    /// <value>
    /// The port used for connection. Default value is 27017. 
    /// </value>
    public int Port { get; set; } = 27017;
  }
}
