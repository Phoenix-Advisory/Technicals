using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.MongoDB.Attributes
{
  /// <summary>
  /// Attribute used to annotate Entity upgrader implementation class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class EntityUpgraderAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the EntityUpgrader class attribute with the desired version and type.
    /// </summary>
    /// <param name="entityType">Target entity type of upgrader.</param>
    /// <param name="version">Target version of upgrader.</param>
    public EntityUpgraderAttribute(Type entityType, string version)
    {
      Version = Version.Parse(version);
      EntityType = entityType;
    }

    /// <summary>
    /// Get target version of upgrader.
    /// </summary>
    public Version Version
    {
      get;
      private set;
    }

    /// <summary>
    /// Get target entity type.
    /// </summary>
    public Type EntityType { get; private set; }
  }
}
