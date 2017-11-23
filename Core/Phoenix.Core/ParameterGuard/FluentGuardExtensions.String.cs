namespace Phoenix.Core.ParameterGuard
{
  /// <summary>
  /// Class with sxtension methods of <see cref="FluentGuard{T}"/> for String guarding.
  /// </summary>
  public static class FluentGuardStringExtensions
  {
    /// <summary>
    /// Check if string parameter is null or empty.
    /// </summary>
    /// <param name="guard">The guard.</param>
    /// <returns>A validation object for fluent chaining.</returns>
    public static FluentGuard<string> IsNullOrEmpty(this FluentGuard<string> guard)
    {
      if (!string.IsNullOrEmpty(guard.Value))
      {
        guard.ThrowException("{0} must be null or string empty.");
      }

      return guard;
    }

    /// <summary>
    /// Check if string parameter is null or contains only whitespaces.
    /// </summary>
    /// <param name="guard">The guard.</param>
    /// <returns>A validation object for fluent chaining.</returns>
    public static FluentGuard<string> IsNullOrWhiteSpace(this FluentGuard<string> guard)
    {
      if (!string.IsNullOrWhiteSpace(guard.Value))
      {
        guard.ThrowException("{0} must be null or contains only whitespaces.");
      }

      return guard;
    }

    /// <summary>
    /// Check if string parameter is not null or empty.
    /// </summary>
    /// <param name="guard">The guard.</param>
    /// <returns>A validation object for fluent chaining.</returns>
    public static FluentGuard<string> IsNotNullOrEmpty(this FluentGuard<string> guard)
    {
      if (string.IsNullOrEmpty(guard.Value))
      {
        guard.ThrowException("{0} can't be null or string empty.");
      }

      return guard;
    }

    /// <summary>
    /// Check if string parameter is not null or not contains only whitespaces.
    /// </summary>
    /// <param name="guard">The guard.</param>
    /// <returns>A validation object for fluent chaining.</returns>
    public static FluentGuard<string> IsNotNullOrWhiteSpace(this FluentGuard<string> guard)
    {
      if (string.IsNullOrWhiteSpace(guard.Value))
      {
        guard.ThrowException("{0} can't be null or can't contains only whitespaces.");
      }

      return guard;
    }
  }
}
