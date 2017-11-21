using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Core.DependencyInjection
{
  /// <summary>
  /// Helper for access to Dependency Injection
  /// </summary>
  public static class DIHelper
  {
    /// <summary>
    /// Gets the Application Dependency Injection container.
    /// </summary>
    public static IContainer ApplicationContainer { get; private set; }

    /// <summary>
    /// Gets the Dependency Injection Container builder.
    /// </summary>
    public static ContainerBuilder Builder { get; private set; } = new ContainerBuilder();

    /// <summary>
    /// Builds the service provider for ASP.NET Core.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceProvider"/>.</returns>
    /// <exception cref="InvalidOperationException">BuildServiceProvider can't be called more than once.</exception>
    public static IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
      if (Builder == null)
        throw new InvalidOperationException("BuildServiceProvider can't be called more than once.");

      if (services != null)
      {
        Builder.Populate(services);
      }

      ApplicationContainer = Builder.Build();
      Builder = null;

      return new AutofacServiceProvider(ApplicationContainer);
    }
  }
}
