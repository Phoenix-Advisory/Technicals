using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.Core.ParameterGuard;
using Phoenix.Core.UserManagement;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Mvc.UserManagement
{
  /// <summary>
  /// Extend collection or service descriptors.
  /// </summary>
  public static class IServiceCollectionExtensions
  {
    /// <summary>
    /// Add <see cref="IUserAccessor" /> which return static informations.
    /// </summary>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="mappingFunction">The function maps claims to <see cref="IUserInfo"/>.</param>
    public static void AddStaticUserAccessor(
            this IServiceCollection services,
            Func<IEnumerable<Claim>, Task<IUserInfo>> mappingFunction
        )
    {
      Guard.IsNotNull(services, nameof(services));
      Guard.IsNotNull(mappingFunction, nameof(mappingFunction));

      services.TryAddSingleton<IUserAccessor>((provider) => {
        return new WebUserAccessor(provider.GetRequiredService<IHttpContextAccessor>(), mappingFunction);
      });
    }
  }
}
