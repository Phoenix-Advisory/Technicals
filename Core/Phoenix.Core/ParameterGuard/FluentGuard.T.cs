using System;

namespace Phoenix.Core.ParameterGuard
{
  /// <summary>
  /// Class for Parameter Guarding
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class FluentGuard<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FluentGuard{T}"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="argumentName">Name of the argument.</param>
    public FluentGuard(T value, string argumentName)
    {
      Value = value;
      ArgumentName = argumentName;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public T Value { get; private set; }
    /// <summary>
    /// Gets the name of the argument.
    /// </summary>
    public string ArgumentName { get; private set; }

    /// <summary>
    /// Throws the <see cref="ArgumentException"/> corresponding to parameter.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <exception cref="ArgumentException"></exception>
    public void ThrowException(string errorMessage)
    {
      throw new ArgumentException(string.Format(errorMessage, ArgumentName), ArgumentName);
    }
  }
}
