using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.MongoDB.Configuration;
using Phoenix.MongoDB.FileRepositories;
using Phoenix.MongoDB.Repositories;
using System.Collections.Generic;

namespace Phoenix.MongoDB
{
  /// <summary>
  /// Extends IServiceCollection with MongoDB methods.
  /// </summary>
  public static class IServiceCollectionExtensions
  {
    /// <summary>
    /// Add provider for MongoDB to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">Service collection where add MongoDB providers.</param>
    public static void AddMongo(this IServiceCollection builder)
    {
      builder.TryAddSingleton<IDictionary<string, MongoDbSetting>>(provider => {
        IConfiguration config = provider.GetRequiredService<IConfiguration>();
        IConfigurationSection section = config.GetSection("MongoDbSettings");
        IDictionary<string, MongoDbSetting> settings = new Dictionary<string, MongoDbSetting>();
        section.Bind(settings);
        return settings;
      });

      builder.TryAddSingleton<ConnectionManager>();

      builder.TryAddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
      builder.TryAddSingleton(typeof(IMongoRepository<,>), typeof(MongoRepository<,>));

      builder.TryAddSingleton(typeof(IMongoFileRepository<>), typeof(MongoFileRepository<>));
      builder.TryAddSingleton(typeof(IMongoFileRepository<,>), typeof(MongoFileRepository<,>));
    }
  }
}
