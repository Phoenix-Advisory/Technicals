using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Phoenix.Core.Reflection
{
  /// <summary>
  /// Default assembly loader
  /// </summary>
  /// <seealso cref="IEntryAssemblyLoader" />
  public class DefaultAssemblyLoader : IEntryAssemblyLoader
  {
    /// <summary>
    /// Gets the entry assembly.
    /// </summary>
    /// <returns></returns>
    public Assembly GetEntryAssembly()
    {
      return Assembly.GetEntryAssembly();
    }


    /// <summary>
    /// Scans the custom assemblies.
    /// </summary>
    /// <param name="assemblyStartWith">Allows filter assemblies by full name.</param>
    /// <returns></returns>
    public IEnumerable<Assembly> ScanCustomAssemblies(string assemblyStartWith)
    {
      yield return Assembly.GetEntryAssembly();

      foreach (AssemblyName assName in GetEntryAssembly()
                                        .GetReferencedAssemblies()
                                        .Where(x =>
                                          x.FullName.StartsWith(assemblyStartWith, StringComparison.OrdinalIgnoreCase)
                                        ))
      {
        yield return Assembly.Load(assName);
      }
    }
  }
}
