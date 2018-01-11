using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.Core.ParameterGuard;

namespace Phoenix.Core.UserManagement
{
  /// <summary>
  /// Extend collection or service descriptors.
  /// </summary>
  public static class IServiceCollectionExtensions 
  {
    /// <summary>
    /// Add <see cref="IUserAccessor"/> which return static informations.
    /// </summary>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="userInfo">The user informations.</param>
    public static void AddStaticUserAccessor(
            this IServiceCollection services,
            IUserInfo userInfo
        )
    {
      Guard.IsNotNull(services, nameof(services));
      Guard.IsNotNull(userInfo, nameof(userInfo));
                                         
      services.TryAddSingleton<IUserAccessor>(new StaticUserAccessor(userInfo));
    }    
  }
}
