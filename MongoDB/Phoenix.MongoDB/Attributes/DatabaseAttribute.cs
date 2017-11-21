using Phoenix.Core.ParameterGuard;
using System;      

namespace Phoenix.MongoDB.Attributes
{
  /// <summary>
  /// Attribute used to annotate Enities with to override mongo connection settings name. 
  /// </summary>
  /// <seealso cref="Attribute" />
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class DatabaseAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseAttribute"/> class.
    /// </summary>
    /// <param name="settingName">Name of the setting.</param>
    public DatabaseAttribute(string settingName)
    {
      Guard.IsNotNullOrWhiteSpace(settingName, nameof(settingName));

      SettingsName = settingName;
    }

    /// <summary>
    /// Gets the name of the settings.
    /// </summary>
    public virtual string SettingsName { get; private set; }
  }
}
