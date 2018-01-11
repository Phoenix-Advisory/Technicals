using Microsoft.Extensions.DependencyInjection;
using Phoenix.Core.ParameterGuard;

namespace Phoenix.Core.Environment
{
  /// <summary>
  /// Extend collection or service descriptors.
  /// </summary>
  public static class IServiceCollectionExtensions 
  {       
    /// <summary>
    /// Add <see cref="IRuntimeEnvironment"/> to dependency injection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="environmentName">Name of the environment.</param>
    /// <param name="rootPath">The root path.</param>
    public static void AddEnvironment(
            this IServiceCollection services,
            string environmentName,
            string rootPath
        )
    {
      Guard.IsNotNull(services, nameof(services));
      Guard.IsNotNullOrWhiteSpace(environmentName, nameof(environmentName));
      Guard.IsNotNullOrWhiteSpace(rootPath, nameof(rootPath));

      services.AddSingleton<IRuntimeEnvironment>(new RuntimeEnvironment(environmentName, rootPath));
      
      // if we want get standard environment name (same aspcore) : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    }

    
  }
}
