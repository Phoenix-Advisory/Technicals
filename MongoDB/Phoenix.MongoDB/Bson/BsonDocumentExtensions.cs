using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Phoenix.Core.ParameterGuard;

namespace Phoenix.MongoDB.Bson
{
  /// <summary>
  /// Extension methods for BsonDocument.
  /// </summary>
  public static class BsonDocumentExtensions
  {
    /// <summary>
    /// Transform <paramref name="doc"/> to dynamic.
    /// </summary>
    /// <param name="doc">The document.</param>
    /// <returns></returns>
    public static dynamic ToDynamic(this BsonDocument doc)
    {
      Guard.IsNotNull(doc, nameof(doc));

      string json = doc.ToJson();
      dynamic obj = JToken.Parse(json);
      return obj;
    }
  }
}
