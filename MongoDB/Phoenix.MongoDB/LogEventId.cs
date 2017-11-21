using Microsoft.Extensions.Logging; 

namespace Phoenix.MongoDB
{
  /// <summary>
  /// Constants for Logging EventId
  /// </summary>
  internal static class LogEventId
  {
    /// <summary>
    /// The EventId for framework 
    /// </summary>
    public static readonly EventId Framework = new EventId(1001, "Phoenix.MongoDB");
  }
}
