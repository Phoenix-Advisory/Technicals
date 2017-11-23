namespace Phoenix.Core.ParameterGuard
{
  /// <summary>
  /// Class with sxtension methods of <see cref="FluentGuard{T}"/> for Object guarding.
  /// </summary>
  public static class FluentGuardObjectExtensions
  {
    /// <summary>
    /// Check if parameter is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guard">The guard.</param>
    /// <returns>A validation object for fluent chaining.</returns>
    public static FluentGuard<T> IsNull<T>(this FluentGuard<T> guard)
        where T : class
    {
      if (guard.Value != null)
      {
        guard.ThrowException("{0} must be null.");
      }

      return guard;
    }

    /// <summary>
    /// Check if parameter is not null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guard">The guard.</param>
    /// <returns>A validation object for fluent chaining.</returns>
    public static FluentGuard<T> IsNotNull<T>(this FluentGuard<T> guard)
        where T : class
    {
      if (guard.Value == null)
      {
        guard.ThrowException("{0} can't be null.");
      }

      return guard;
    }
  }
}
