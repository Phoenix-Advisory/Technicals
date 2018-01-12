using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Phoenix.Core.ParameterGuard;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Phoenix.Core.Security
{
  /// <summary>
  /// Password hashing implementation with PBKDF2
  /// </summary>
  /// <seealso cref="IPasswordHasher" />
  public class PBKDF2PasswordHasher : IPasswordHasher
  {
    /* =======================
     * HASHED PASSWORD FORMATS
     * =======================
     *
     * Version 3:
     * PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, 10000 iterations.
     * Format: { 0x01, prf (UInt32), iter count (UInt32), salt length (UInt32), salt, subkey }
     * (All UInt32s are stored big-endian.)
     */

    private const int PBKDF2IterCount = 10000;
    private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
    private const int SaltSize = 128 / 8; // 128 bits
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    /// <summary>
    /// Returns a hashed representation of the specified <paramref name="password" />.
    /// </summary>
    /// <param name="password">The password to generate a hash value for.</param>
    /// <returns>
    /// The hash value for <paramref name="password" /> as a base-64-encoded string.
    /// </returns>
    public string HashPassword(string password)
    {
      Guard.IsNotNull(password, nameof(password));

      return Convert.ToBase64String(HashPasswordInternal(password, KeyDerivationPrf.HMACSHA256, PBKDF2IterCount, SaltSize, PBKDF2SubkeyLength));
    }

    /// <summary>
    /// Determines whether the specified RFC 2898 hash and password are a cryptographic match.
    /// </summary>
    /// <param name="hashedPassword">The previously-computed RFC 2898 hash value as a base-64-encoded string.</param>
    /// <param name="password">The plaintext password to cryptographically compare with hashedPassword.</param>
    /// <returns>
    /// true if the hash value is a cryptographic match for the password; otherwise, false.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <remarks>
    /// <paramref name="hashedPassword" /> must be of the format of HashPassword (salt + Hash(salt+input).
    /// </remarks>
    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
      Guard.IsNotNull(hashedPassword, nameof(hashedPassword));
      Guard.IsNotNull(password, nameof(password));
      return VerifyHashedPasswordInternal(hashedPassword, password);
    }

    private byte[] HashPasswordInternal(
            string password,
            KeyDerivationPrf prf,
            int iterCount,
            int saltSize,
            int numBytesRequested)
    {
      // Produce a version 3 (see comment above) text hash.
      byte[] salt = new byte[saltSize];
      _rng.GetBytes(salt);
      byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

      byte[] outputBytes = new byte[13 + salt.Length + subkey.Length];

      // Write format marker.
      outputBytes[0] = 0x01;

      // Write hashing algorithm version.
      WriteNetworkByteOrder(outputBytes, 1, (uint)prf);

      // Write iteration count of the algorithm.
      WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);

      // Write size of the salt.
      WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);

      // Write the salt.
      Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);

      // Write the subkey.
      Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
      return outputBytes;
    }

    private static bool VerifyHashedPasswordInternal(string hashedPassword, string password)
    {
      byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

      if (decodedHashedPassword.Length == 0)
      {
        return false;
      }

      try
      {
        // Verify hashing format.
        if (decodedHashedPassword[0] != 0x01)
        {
          // Unknown format header.
          return false;
        }

        // Read hashing algorithm version.
        KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);

        // Read iteration count of the algorithm.
        int iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);

        // Read size of the salt.
        int saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

        // Verify the salt size: >= 128 bits.
        if (saltLength < 128 / 8)
        {
          return false;
        }

        // Read the salt.
        byte[] salt = new byte[saltLength];
        Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

        // Verify the subkey length >= 128 bits.
        int subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
        if (subkeyLength < 128 / 8)
        {
          return false;
        }

        // Read the subkey.
        byte[] expectedSubkey = new byte[subkeyLength];
        Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

        // Hash the given password and verify it against the expected subkey.
        byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
        return ByteArraysEqual(actualSubkey, expectedSubkey);
      }
      catch
      {
        // This should never occur except in the case of a malformed payload, where
        // we might go off the end of the array. Regardless, a malformed payload
        // implies verification failed.
        return false;
      }
    }

    private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
    {
      return ((uint)(buffer[offset + 0]) << 24)
          | ((uint)(buffer[offset + 1]) << 16)
          | ((uint)(buffer[offset + 2]) << 8)
          | ((uint)(buffer[offset + 3]));
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
      buffer[offset + 0] = (byte)(value >> 24);
      buffer[offset + 1] = (byte)(value >> 16);
      buffer[offset + 2] = (byte)(value >> 8);
      buffer[offset + 3] = (byte)(value >> 0);
    }

    // Compares two byte arrays for equality. 
    // The method is specifically written so that the loop is not optimized.
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
      if (ReferenceEquals(a, b))
      {
        return true;
      }

      if (a == null || b == null || a.Length != b.Length)
      {
        return false;
      }

      var areSame = true;
      for (var i = 0; i < a.Length; i++)
      {
        areSame &= (a[i] == b[i]);
      }
      return areSame;
    }
  }
}
