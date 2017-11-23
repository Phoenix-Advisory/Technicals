using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Phoenix.Core.Reflection
{
  /// <summary>
  /// Assembly loader for test class
  /// </summary>
  /// <seealso cref="IEntryAssemblyLoader" />
  public class TestAssemblyLoader : IEntryAssemblyLoader
  {
    private readonly Assembly _Assembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAssemblyLoader"/> class.
    /// </summary>
    /// <param name="ass">The ass.</param>
    public TestAssemblyLoader(Assembly ass)
    {
      _Assembly = ass;
    }

    /// <summary>
    /// Gets the entry assembly.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public Assembly GetEntryAssembly()
    {
      return _Assembly;
    }

    /// <summary>
    /// Scans the custom assemblies.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Assembly> ScanCustomAssemblies(string assemblyStartWith)
    {
      yield return _Assembly;

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
