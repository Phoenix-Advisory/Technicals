using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.MongoDB
{
  /// <summary>
  /// Base class for Entities with <see cref="Guid"/> as Key stored in MongoDb.
  /// </summary>
  /// <seealso cref="Entity{T}" />
  public class Entity : Entity<Guid>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    public Entity()
    {
      Id = Guid.NewGuid();
    }
  }
}
