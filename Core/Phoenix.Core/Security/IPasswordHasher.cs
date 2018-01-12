using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Core.Security
{
  /// <summary>
  /// Interface for Password Hashing
  /// </summary>
  public interface IPasswordHasher
  {
    /// <summary>
    /// Returns a hashed representation of the specified <paramref name="password"/>.
    /// </summary>
    /// <param name="password">The password to generate a hash value for.</param>
    /// <returns>The hash value for <paramref name="password" /> as a base-64-encoded string.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Determines whether the specified RFC 2898 hash and password are a cryptographic match.
    /// </summary>
    /// <param name="hashedPassword">The previously-computed RFC 2898 hash value as a base-64-encoded string.</param>
    /// <param name="password">The plaintext password to cryptographically compare with hashedPassword.</param>
    /// <returns>true if the hash value is a cryptographic match for the password; otherwise, false.</returns>
    /// <remarks>
    /// <paramref name="hashedPassword" /> must be of the format of HashPassword (salt + Hash(salt+input).
    /// </remarks>  
    bool VerifyHashedPassword(string hashedPassword, string password);
  }
}
