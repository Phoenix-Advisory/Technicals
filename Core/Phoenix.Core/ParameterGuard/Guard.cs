namespace Phoenix.Core.ParameterGuard
{
  /// <summary>
  /// Static class for parameter guard.
  /// </summary>
  public static class Guard
  {
    /// <summary>
    /// Check if parameter is null.
    /// </summary>
    /// <typeparam name="T">Type of parameter</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <returns>The <see cref="FluentGuard{T}"/> instance for parameter</returns>
    public static FluentGuard<T> IsNull<T>(T value, string argumentName)
        where T : class
    {
      return new FluentGuard<T>(value, argumentName).IsNull();
    }

    /// <summary>
    /// Check if parameter is not null.
    /// </summary>
    /// <typeparam name="T">Type of parameter</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <returns>The <see cref="FluentGuard{T}"/> instance for parameter.</returns>
    public static FluentGuard<T> IsNotNull<T>(T value, string argumentName)
        where T : class
    {
      return new FluentGuard<T>(value, argumentName).IsNotNull();
    }

    /// <summary>
    /// Check if string parameter is null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <returns>The <see cref="FluentGuard{T}"/> instance for parameter.</returns>
    public static FluentGuard<string> IsNullOrEmpty(string value, string argumentName)
    {
      return new FluentGuard<string>(value, argumentName).IsNullOrEmpty();
    }

    /// <summary>
    /// Check if string parameter is null or contains only whitespaces.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <returns>The <see cref="FluentGuard{T}"/> instance for parameter.</returns>
    public static FluentGuard<string> IsNullOrWhiteSpace(string value, string argumentName)
    {
      return new FluentGuard<string>(value, argumentName).IsNullOrWhiteSpace();
    }

    /// <summary>
    /// Check if string parameter is not null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <returns>The <see cref="FluentGuard{T}"/> instance for parameter.</returns>
    public static FluentGuard<string> IsNotNullOrEmpty(string value, string argumentName)
    {
      return new FluentGuard<string>(value, argumentName).IsNotNullOrEmpty();
    }

    /// <summary>
    /// Check if string parameter is not null or not contains only whitespaces.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="argumentName">Name of the argument.</param>
    /// <returns>The <see cref="FluentGuard{T}"/> instance for parameter.</returns>
    public static FluentGuard<string> IsNotNullOrWhiteSpace(string value, string argumentName)
    {
      return new FluentGuard<string>(value, argumentName).IsNotNullOrWhiteSpace();
    }
  }
}
