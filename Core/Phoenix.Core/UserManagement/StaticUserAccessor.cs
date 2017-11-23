using Phoenix.Core.ParameterGuard;
using System.Threading.Tasks;

namespace Phoenix.Core.UserManagement
{
  /// <summary>
  /// User accessor for internal process
  /// </summary>
  /// <seealso cref="IUserAccessor" />
  public class StaticUserAccessor : IUserAccessor
  {
    private readonly IUserInfo _User;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticUserAccessor"/> class.
    /// </summary>
    /// <param name="user">User will be send to each request.</param>
    public StaticUserAccessor(IUserInfo user)
    {
      Guard.IsNotNull(user, nameof(user));

      _User = user;
    }

    /// <summary>
    /// Gets the current user.
    /// </summary>
    public Task<IUserInfo> GetCurrentUser()
    {
      return Task.FromResult(_User);
    }
  }
}
