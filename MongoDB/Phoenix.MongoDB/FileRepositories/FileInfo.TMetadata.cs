using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Phoenix.MongoDB.FileRepositories
{
  /// <summary>
  /// Represents information about a stored GridFS file (backed by a files collection document).
  /// </summary>
  /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
  public class FileInfo<TMetadata>
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [BsonElement(elementName: "_id")]
    public ObjectId Id { get; set; }
    /// <summary>
    /// Gets the size of a chunk.
    /// </summary>
    /// <value>
    /// The chunk size.
    /// </value>
    [BsonElement(elementName: "chunkSize")]
    public int ChunkSizeBytes { get; set; }
    /// <summary>
    /// Gets or sets the length of content.
    /// </summary>
    /// <value>
    /// The length of content.
    /// </value>
    [BsonElement(elementName: "length")]
    public long Length { get; set; }
    /// <summary>
    /// Gets or sets the type of the content.
    /// </summary>
    /// <value>
    /// The type of the content.
    /// </value>
    [BsonElement(elementName: "contentType")]
    public string ContentType { get; set; }
    /// <summary>
    /// Gets or sets the filename.
    /// </summary>
    /// <value>
    /// The filename.
    /// </value>
    [BsonElement(elementName: "filename")]
    public string Filename { get; set; }
    /// <summary>
    /// Gets or sets the MD5 hash.
    /// </summary>
    /// <value>
    /// The MD5 hash.
    /// </value>
    [BsonElement(elementName: "md5")]
    public string MD5 { get; set; }
    /// <summary>
    /// Gets or sets the upload date time.
    /// </summary>
    /// <value>
    /// The upload date time.
    /// </value>
    [BsonElement(elementName: "uploadDate")]
    public DateTime UploadDateTime { get; set; }
    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    /// <value>
    /// The metadata.
    /// </value>
    [BsonElement(elementName: "metadata")]
    public TMetadata Metadata { get; set; }
  }
}
