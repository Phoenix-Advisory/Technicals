using System.Collections.Generic;
using System.Reflection;

namespace Phoenix.Core.Reflection
{
  /// <summary>
  /// Obtains Assembly from context
  /// </summary>
  public interface IEntryAssemblyLoader
  {
    /// <summary>
    /// Gets the entry assembly.
    /// </summary>
    /// <returns></returns>
    Assembly GetEntryAssembly();

    /// <summary>
    /// Scans the custom assemblies.
    /// </summary>
    /// <param name="assemblyStartWith">Allows filter assemblies by full name.</param>
    /// <returns></returns>
    IEnumerable<Assembly> ScanCustomAssemblies(string assemblyStartWith);
  }
}
