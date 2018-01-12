using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.Core.ParameterGuard;

namespace Phoenix.Core.Security
{
  /// <summary>
  /// Extend collection or service descriptors.
  /// </summary>
  public static class IServiceCollectionExtensions
  {
    /// <summary>
    /// Add <see cref="IPasswordHasher"/> to Dependency injection.
    /// </summary>
    /// <param name="services">The collection of service descriptors.</param>
    public static void AddPasswordHasher(
            this IServiceCollection services
        )
    {
      Guard.IsNotNull(services, nameof(services)); 

      services.TryAddSingleton<IPasswordHasher, PBKDF2PasswordHasher>();
    }
  }
}
