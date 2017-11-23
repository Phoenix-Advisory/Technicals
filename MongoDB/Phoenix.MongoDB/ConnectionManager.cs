using Autofac;
using MongoDB.Bson;
using MongoDB.Driver;                   
using Phoenix.Core.ParameterGuard;
using Phoenix.MongoDB.Attributes;
using Phoenix.MongoDB.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Phoenix.MongoDB
{
  /// <summary>
  /// Help to manage MongoDb connection. 
  /// </summary>
  public class ConnectionManager
  {
    private readonly IDictionary<string, IMongoDatabase> _Databases = new Dictionary<string, IMongoDatabase>();
    private readonly IDictionary<string, IMongoClient> _Clients = new Dictionary<string, IMongoClient>();
    private readonly object _Lock = new object();
    private readonly IDictionary<string, MongoDbSetting> _Settings;

    private const string BUCKET_METADATA_SUFFIX = ".files";

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public ConnectionManager(IDictionary<string, MongoDbSetting> settings)
    {
      Guard.IsNotNull(settings, nameof(settings));
      _Settings = settings;
    }

    /// <summary>
    /// Gets the database connection.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <returns>
    /// The <see cref="IMongoDatabase" /> corresponding to <paramref name="settings" />.
    /// </returns>
    public IMongoDatabase GetDatabase(MongoDbSetting settings)
    {
      Guard.IsNotNull(settings, nameof(settings));

      string cacheKey = settings.Id;
      if (!_Databases.ContainsKey(cacheKey))
      {
        lock (_Lock)
        {
          if (!_Databases.ContainsKey(cacheKey))
          {
            if (!_Clients.ContainsKey(settings.Id))
            {
              MongoClientSettings mongoSettings = new MongoClientSettings();
              mongoSettings.Servers = settings.Servers.Select(x => new MongoServerAddress(x.Address, x.Port)).ToList();
              if (!string.IsNullOrWhiteSpace(settings.Username))
              {
                mongoSettings.Credentials = new[] {
                                MongoCredential.CreateCredential(
                                    string.IsNullOrWhiteSpace(settings.AuthenticationDatabase) ? settings.Database : settings.AuthenticationDatabase,
                                    settings.Username,
                                    settings.Password
                                )
                            };
              }
              if (!string.IsNullOrWhiteSpace(settings.ReplicaSet))
              {
                mongoSettings.ConnectionMode = ConnectionMode.ReplicaSet;
                mongoSettings.ReplicaSetName = settings.ReplicaSet;

                mongoSettings.ReadConcern = new ReadConcern(settings.ReadConcernLevel);
                mongoSettings.ReadPreference = new ReadPreference(settings.ReadPreferenceMode);
                if (!string.IsNullOrWhiteSpace(settings.WriteConcernMode))
                {
                  mongoSettings.WriteConcern = new WriteConcern(settings.WriteConcernMode);
                }
              }
              mongoSettings.ConnectTimeout = TimeSpan.Parse(settings.ConnectTimeout);
              mongoSettings.GuidRepresentation = GuidRepresentation.Standard;
              mongoSettings.IPv6 = false;

              _Clients.Add(settings.Id, new MongoClient(mongoSettings));
            }
            _Databases.Add(cacheKey, _Clients[settings.Id].GetDatabase(settings.Database));
          }
        }
      }
      return _Databases[cacheKey];
    }

    /// <summary>
    /// Gets the database connection.
    /// </summary>
    /// <param name="connectionName">Name of the connection settings.</param>
    /// <returns>
    /// The <see cref="IMongoDatabase" /> corresponding to <paramref name="connectionName" />
    /// </returns>
    /// <exception cref="ArgumentException">Can't found connection settings for {connectionName}</exception>
    public IMongoDatabase GetDatabase(string connectionName)
    {
      Guard.IsNotNullOrWhiteSpace(connectionName, nameof(connectionName));

      MongoDbSetting setting = null;
      if (!_Settings.TryGetValue(connectionName, out setting) || setting == null)
      { 
        throw new ArgumentException($"Can't found connection settings for '{connectionName}'", nameof(connectionName));
      }

      return GetDatabase(setting);
    }

    /// <summary>
    /// Determines the collection name for <paramref name="entityType"/> and assures it is not empty
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <returns>
    /// Returns the collectionname for <paramref name="entityType"/>.
    /// </returns>
    /// <exception cref="ArgumentException">Collection name cannot be empty for this entity</exception>
    internal static string GetCollectionName(Type entityType)
    {
      Type tmp = entityType;
      string res;
      while (tmp != null && tmp != typeof(object))
      {
        if (tmp.GetTypeInfo().IsGenericType)
        {
          Type[] genericType = tmp.GetGenericArguments();
          foreach (Type tp in genericType)
          {
            res = GetCollectionName(tp);
            if (!string.IsNullOrEmpty(res))
            {
              return res;
            }
          }
        }

        var att = tmp.GetTypeInfo().GetCustomAttribute(typeof(CollectionNameAttribute), true);
        if (att != null)
        {
          // It does! Return the value specified by the CollectionName attribute
          return ((CollectionNameAttribute)att).Name;
        }
        tmp = tmp.GetTypeInfo().BaseType;
      }

      res = GetBucketName(entityType);
      if (!string.IsNullOrEmpty(res))
      {
        return $"{res}{BUCKET_METADATA_SUFFIX}";
      }

      return null;
    }

    /// <summary>
    /// Determines the collection name for <paramref name="entityType"/> and assures it is not empty
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <returns>
    /// Returns the collectionname for <paramref name="entityType"/>.
    /// </returns>
    /// <exception cref="ArgumentException">Collection name cannot be empty for this entity</exception>
    internal static string GetBucketName(Type entityType)
    {
      Type tmp = entityType;
      while (tmp != null && tmp != typeof(object))
      {
        if (tmp.GetTypeInfo().IsGenericType)
        {
          Type[] genericType = tmp.GetGenericArguments();
          foreach (Type tp in genericType)
          {
            string res = GetBucketName(tp);
            if (!string.IsNullOrEmpty(res))
            {
              return res;
            }
          }
        }

        var att = tmp.GetTypeInfo().GetCustomAttribute(typeof(BucketNameAttribute), true);
        if (att != null)
        {
          // It does! Return the value specified by the CollectionName attribute
          return ((BucketNameAttribute)att).Name;
        }
        tmp = tmp.GetTypeInfo().BaseType;
      }

      return null;                                                                                                          
    }          

    internal static string GetDatabaseSettingsName(Type entityType)
    {
      Type tmp = entityType;
      while (tmp != null && tmp != typeof(object))
      {
        if (tmp.GetTypeInfo().IsGenericType)
        {
          Type[] genericType = tmp.GetGenericArguments();
          foreach (Type tp in genericType)
          {
            string res = GetDatabaseSettingsName(tp);
            if (!string.IsNullOrEmpty(res))
            {
              return res;
            }
          }
        }

        var att = tmp.GetTypeInfo().GetCustomAttribute(typeof(DatabaseAttribute));
        if (att != null)
        {
          // It does! Return the value specified by the CollectionName attribute
          return ((DatabaseAttribute)att).SettingsName;
        }
        tmp = tmp.GetTypeInfo().BaseType;
      }

      return null;
    }
  }
}
