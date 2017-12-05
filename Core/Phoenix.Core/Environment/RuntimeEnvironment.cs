using System;
using System.Linq;

namespace Phoenix.Core.Environment
{
  /// <summary>
  /// Provide basic environment class
  /// </summary>
  /// <seealso cref="IRuntimeEnvironment" />
  public class RuntimeEnvironment : IRuntimeEnvironment
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeEnvironment"/> class.
    /// </summary>
    /// <param name="name">The name of current environment.</param>
    /// <param name="rootPath">The root path.</param>
    public RuntimeEnvironment(string name, string rootPath)
    {
      Name = name;
      RootPath = rootPath;
    }

    /// <summary>
    /// Gets the list of names used for production environment.
    /// </summary>
    protected virtual string[] ProductionNames { get; } = new[] { "production", "prod" };
    /// <summary>
    /// Gets the list of names used for testing environment.
    /// </summary> 
    protected virtual string[] TestNames { get; } = new[] { "uat", "test", "staging" };

    /// <summary>
    /// Gets the name of current environment.
    /// </summary>
    public string Name
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets a value indicating whether this is development environment.
    /// </summary>
    public bool IsDev
    {
      get { return !IsProduction && !IsTest; }
    }

    /// <summary>
    /// Gets a value indicating whether this is test environment.
    /// </summary>
    public bool IsTest
    {
      get { return TestNames.Any(x => x.Equals(Name, StringComparison.OrdinalIgnoreCase)); }
    }

    /// <summary>
    /// Gets a value indicating whether this is production environment.
    /// </summary>
    public bool IsProduction
    {
      get { return ProductionNames.Any(x => x.Equals(Name, StringComparison.OrdinalIgnoreCase)); }
    }

    /// <summary>
    /// Gets the root path.
    /// </summary>
    public string RootPath
    {
      get;
      private set;
    }
  }
}
