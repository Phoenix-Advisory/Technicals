using System.Threading.Tasks;

namespace Phoenix.Core.UserManagement
{
  /// <summary>
  /// Interface allow accessing current user informations.
  /// </summary>
  public interface IUserAccessor
  {
    /// <summary>
    /// Gets the current user.
    /// </summary>
    /// <returns></returns>
    Task<IUserInfo> GetCurrentUser();
  }
}
