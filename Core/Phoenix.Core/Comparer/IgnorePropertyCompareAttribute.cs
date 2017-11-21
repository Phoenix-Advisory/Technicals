using System;

namespace Phoenix.Core.Comparer
{
  /// <summary>
  /// Decorate property which not be consider in dif process.
  /// </summary>
  /// <seealso cref="Attribute" />  
  [AttributeUsage(AttributeTargets.Property)]
  public class IgnorePropertyCompareAttribute : Attribute
  {
  }
}
