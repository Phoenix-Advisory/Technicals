namespace Phoenix.Core.Environment
{
  /// <summary>
  /// Provide informations about current runtime environment.
  /// </summary>
  public interface IRuntimeEnvironment
  {
    /// <summary>
    /// Gets the name of current environment.
    /// </summary>  
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether this is test environment.
    /// </summary>
    bool IsTest { get; }

    /// <summary>
    /// Gets a value indicating whether this is development environment.
    /// </summary>
    bool IsDev { get; }

    /// <summary>
    /// Gets a value indicating whether this is production environment.
    /// </summary>
    bool IsProduction { get; }

    /// <summary>
    /// Gets the root path.
    /// </summary>
    string RootPath { get; }
  }
}
