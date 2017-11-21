using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Core.UserManagement
{
  /// <summary>
  /// Represent minimal user informations.
  /// </summary>
  public interface IUserInfo
  {
    /// <summary>
    /// Get identifier of the user.
    /// </summary>
    Guid Id { get; }   
    /// <summary>
    /// Get display name of the user.
    /// </summary>
    string DisplayName { get; }
  }
}
