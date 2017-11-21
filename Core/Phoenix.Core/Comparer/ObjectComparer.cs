using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Phoenix.Core.Comparer
{
  /// <summary>
  /// Compute difference between 2 instance of class.
  /// </summary>
  public static class ObjectComparer
  {
    /// <summary>
    /// Compares the specified old and new object.
    /// </summary>
    /// <typeparam name="T">Type of object to compare.</typeparam>
    /// <param name="oldObject">The old object.</param>
    /// <param name="newObject">The new object.</param>
    /// <returns><see cref="IEnumerable{DifResult}"/> containing all differente properties.</returns>
    public static IEnumerable<CompareResult> Compare<T>(T oldObject, T newObject)
    {
      return Compare(oldObject?.GetType() ?? newObject?.GetType() ?? typeof(T), oldObject, newObject, string.Empty);
    }

    /// <summary>
    /// Compares the specified old and new object.
    /// </summary>
    /// <param name="type">Type of object to compare.</param>
    /// <param name="oldObject">The old object.</param>
    /// <param name="newObject">The new object.</param>
    public static IEnumerable<CompareResult> Compare(Type type, object oldObject, object newObject)
    {
      return Compare(type, oldObject, newObject, string.Empty);
    }


    private static IEnumerable<CompareResult> Compare(Type type, object oldObject, object newObject, string prefix)
    {
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
      foreach (PropertyInfo pi in properties)
      {
        if (pi.GetCustomAttribute<IgnorePropertyCompareAttribute>() != null)
        {
          continue;
        }

        object oldValue = oldObject == null ? null : pi.GetValue(oldObject);
        object newValue = newObject == null ? null : pi.GetValue(newObject);

        if (pi.GetIndexParameters().Length > 0 || pi.PropertyType.GetTypeInfo().IsArray || typeof(IEnumerable).IsAssignableFrom(pi.PropertyType))
        {
          string oldJson = JsonConvert.SerializeObject(oldValue);
          string newJson = JsonConvert.SerializeObject(newValue);
          if (oldJson != newJson)
          {
            yield return new CompareResult
            {
              PropertyName = GetName(prefix, pi.Name),
              OldValue = oldValue,
              NewValue = newValue
            };
          }
          continue;
        }
        else if ((pi.PropertyType.GetTypeInfo().IsPrimitive || pi.PropertyType.GetTypeInfo().IsEnum) && Equals(oldValue, newValue))
        {
          continue;
        }
        else if (ReferenceEquals(oldValue, newValue) || Equals(oldValue, newValue))
        {
          continue;
        }
        else if (!(pi.PropertyType.GetTypeInfo().IsPrimitive || pi.PropertyType.GetTypeInfo().IsEnum) && pi.PropertyType.Namespace != "System")
        {
          foreach (CompareResult res in Compare(pi.PropertyType, oldValue, newValue, GetName(prefix, pi.Name)))
            yield return res;

          continue;
        }

        yield return new CompareResult
        {
          PropertyName = GetName(prefix, pi.Name),
          OldValue = oldValue,
          NewValue = newValue
        };
      }
    }

    private static string GetName(string prefix, string propertyName)
    {
      return string.IsNullOrWhiteSpace(prefix) ? propertyName : prefix + "." + propertyName;
    }
  }
}
