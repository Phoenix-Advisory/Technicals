﻿using Phoenix.Core.ParameterGuard;
using System;     

namespace Phoenix.MongoDB.Attributes
{
  /// <summary>
  /// Attribute used to annotate Enities with to override mongo collection name. By default, when this attribute
  /// is not specified, the classname will be used.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class CollectionNameAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the CollectionName class attribute with the desired name.
    /// </summary>
    /// <param name="name">Name of the collection.</param>         
    public CollectionNameAttribute(string name)
    {
      Guard.IsNotNullOrWhiteSpace(name, nameof(name));
      Name = name;  
    }

    /// <summary>
    /// Gets the name of the collection.
    /// </summary>
    /// <value>The name of the collection.</value>
    public virtual string Name { get; private set; }   
  }
}
