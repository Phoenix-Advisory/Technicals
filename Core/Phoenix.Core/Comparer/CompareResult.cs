using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Core.Comparer
{
  /// <summary>
  /// Contains differing values of property.
  /// </summary>
  public class CompareResult
  {
    /// <summary>
    /// Get full property name which differ.
    /// </summary>
    public string PropertyName { get; internal set; }
    /// <summary>
    /// Get the new value of property.
    /// </summary> 
    /// <remarks>
    /// New value is equal to <c>null</c> if destination object is <c>null</c>.
    /// </remarks>
    public object NewValue { get; internal set; }
    /// <summary>
    /// Get the old value of property.
    /// </summary>
    /// <remarks>
    /// Old value is equal to <c>null</c> if source object is <c>null</c>.
    /// </remarks>
    public object OldValue { get; internal set; }
  }
}
