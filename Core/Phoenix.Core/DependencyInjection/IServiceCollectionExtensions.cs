using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.Core.ParameterGuard;
using System;  

namespace Phoenix.Core.DependencyInjection
{
  /// <summary>
  /// Extend collection or service descriptors.
  /// </summary>
  public static class IServiceCollectionExtensions 
  {
    /// <summary>
    /// Read configuration and add result to Dependency Injection.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="pocoProvider">The function instanciate configurable object.</param>
    /// <returns>
    /// The filled configurable object.
    /// </returns>
    public static TConfig ConfigureSetting<TConfig>(
            this IServiceCollection services,
            IConfiguration configuration,
            Func<TConfig> pocoProvider
        )
        where TConfig : class
    {
      Guard.IsNotNull(services, nameof(services));
      Guard.IsNotNull(configuration, nameof(configuration));
      Guard.IsNotNull(pocoProvider, nameof(pocoProvider));

      TConfig config = pocoProvider();
      return ConfigureSetting(services, configuration, config);
    }

    /// <summary>
    /// Read configuration and add result to Dependency Injection.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="config">The configurable object.</param>
    /// <returns>The filled configurable object.</returns>
    public static TConfig ConfigureSetting<TConfig>(
            this IServiceCollection services,
            IConfiguration configuration,
            TConfig config
        )
        where TConfig : class
    {
      Guard.IsNotNull(services, nameof(services));
      Guard.IsNotNull(configuration, nameof(configuration));
      Guard.IsNotNull(config, nameof(config));

      configuration.Bind(config);
      services.TryAddSingleton(config);
      return config;
    }

    /// <summary>
    /// Read configuration and add result to Dependency Injection.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>
    /// The filled configurable object.
    /// </returns>
    public static TConfig ConfigureSetting<TConfig>(
            this IServiceCollection services,
            IConfiguration configuration
        )
        where TConfig : class, new()
    {
      Guard.IsNotNull(services, nameof(services));
      Guard.IsNotNull(configuration, nameof(configuration));

      TConfig config = new TConfig();
      return ConfigureSetting(services, configuration, config);
    }
  }
}
