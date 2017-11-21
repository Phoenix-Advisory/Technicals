using Phoenix.Core.ParameterGuard;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Core.Collections
{
  /// <summary>
  /// Extend <see cref="IList{T}"/>
  /// </summary>
  public static class IListExtensions
  {
    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">TypeOf element</typeparam>
    /// <param name="destination">The destination.</param>
    /// <param name="sources">
    /// The collection whose elements should be added to the end of the <see cref="IList{T}"/>. 
    /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
    /// </param>
    /// <param name="onlyNotNull"><c>true</c> if null value must be skipped; otherwise <c>false</c>.</param>
    public static void AddRange<T>(this IList<T> destination, IEnumerable<T> sources, bool onlyNotNull = true)
    {
      Guard.IsNotNull(destination, nameof(destination));

      foreach (T itm in sources.Where(x => !onlyNotNull || x != null))
        destination.Add(itm);
    }
  }
}
