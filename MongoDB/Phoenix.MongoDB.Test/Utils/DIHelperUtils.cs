using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Phoenix.Core.DependencyInjection;
using Phoenix.MongoDB.Configuration;
using Phoenix.MongoDB.FileRepositories;
using Phoenix.MongoDB.Repositories;
using Phoenix.MongoDB.Test.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Phoenix.MongoDB.Test.Utils
{
  /// <summary>
  /// DI container tools helper
  /// </summary>
  internal static class DIHelperUtils
  {
    /// <summary>
    /// Resets the container.
    /// </summary>
    public static void ResetContainer()
    {
      Type t = typeof(DIHelper);
      PropertyInfo prop = t.GetProperty("ApplicationContainer");
      prop.SetValue(t, null, null);

      prop = t.GetProperty("Builder");
      prop.SetValue(t, new ContainerBuilder(), null);
    }

    public static IServiceProvider InitializeContainer()
    {
      IServiceCollection services = new ServiceCollection();
      services.AddMongo();
      services.AddSingleton<IDictionary<string, MongoDbSetting>>(
        (prov) =>
        {
          IDictionary<string, MongoDbSetting> settings = new Dictionary<string, MongoDbSetting>();
          MongoDbSetting setting = new MongoDbSetting
          {
            Id = "Test",
            Database = $"MyTestDb",
            Servers = new List<MongoDbServer> {
                    new MongoDbServer { Address = "localhost", Port = 27017 }
                }
          };
          settings.Add(setting.Id, setting);
          setting = new MongoDbSetting
          {
            Id = "Test2",
            Database = $"MyTestDb2",
            Servers = new List<MongoDbServer> {
                    new MongoDbServer { Address = "localhost", Port=27017 }
                }
          };
          settings.Add(setting.Id, setting);
          return settings;
        }

        );
      services.AddSingleton<IMongoRepository<MyNamedEntity>>((prov) => new MongoRepository<MyNamedEntity>("Test", "Test3", prov.GetService<ConnectionManager>()));
      services.AddSingleton<IMongoFileRepository<MyNamedEntity>>((prov) => new MongoFileRepository<MyNamedEntity>("Test", "Test3", prov.GetService<ConnectionManager>()));
      return services.BuildServiceProvider();


    }
  }
}
