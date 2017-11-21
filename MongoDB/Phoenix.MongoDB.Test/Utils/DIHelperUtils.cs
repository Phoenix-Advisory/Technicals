using Autofac;
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

    public static IContainer InitializeContainer()
    {
      DIHelperUtils.ResetContainer();
      if (DIHelper.ApplicationContainer == null)
      {
        MongoDbSetting setting = new MongoDbSetting
        {
          Id = "Test",
          Database = $"MyTestDb",
          Servers = new List<MongoDbServer> {
                    new MongoDbServer { Address = "localhost", Port = 27017 }
                }
        };
        DIHelper.Builder.RegisterInstance(setting).Named<MongoDbSetting>("Test");
        setting = new MongoDbSetting
        {
          Id = "Test2",
          Database = $"MyTestDb2",
          Servers = new List<MongoDbServer> {
                    new MongoDbServer { Address = "localhost", Port=27017 }
                }
        };
        DIHelper.Builder.RegisterInstance(setting).Named<MongoDbSetting>("Test2");

        DIHelper.Builder.RegisterInstance(new LoggerFactory()).As<ILoggerFactory>();

        DIHelper.Builder.RegisterGeneric(typeof(MongoRepository<>)).As(typeof(IMongoRepository<>)).SingleInstance();
        DIHelper.Builder.RegisterGeneric(typeof(MongoRepository<,>)).As(typeof(IMongoRepository<,>)).SingleInstance();
        DIHelper.Builder.RegisterGeneric(typeof(MongoFileRepository<>)).As(typeof(IMongoFileRepository<>)).SingleInstance();
        DIHelper.Builder.RegisterGeneric(typeof(MongoFileRepository<,>)).As(typeof(IMongoFileRepository<,>)).SingleInstance();

        DIHelper.Builder.RegisterInstance(new MongoRepository<MyNamedEntity>("Test", "Test3")).As<IMongoRepository<MyNamedEntity>>();
        DIHelper.Builder.RegisterInstance(new MongoFileRepository<MyNamedEntity>("Test", "Test3")).As<IMongoFileRepository<MyNamedEntity>>();
        DIHelper.BuildServiceProvider(null);
      }

      return DIHelper.ApplicationContainer;
    }
  }
}
