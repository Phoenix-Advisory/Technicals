using Microsoft.AspNetCore.Http;
using Phoenix.Core.ParameterGuard;
using Phoenix.Core.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Phoenix.Mvc.UserManagement
{
  /// <summary>
  /// User accessor for web request.
  /// </summary>
  /// <seealso cref="IUserAccessor" />
  public class WebUserAccessor : IUserAccessor
  {
    private readonly IHttpContextAccessor _Accessor;
    private readonly Func<IEnumerable<Claim>, Task<IUserInfo>> _MapClaimsToUserInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebUserAccessor" /> class.
    /// </summary>
    /// <param name="accessor">The accessor.</param>
    /// <param name="mapClaimsToUserInfo">The map claims to user information.</param>
    public WebUserAccessor(IHttpContextAccessor accessor, Func<IEnumerable<Claim>, Task<IUserInfo>> mapClaimsToUserInfo)
    {
      Guard.IsNotNull(accessor, nameof(accessor));
      Guard.IsNotNull(mapClaimsToUserInfo, nameof(mapClaimsToUserInfo));

      _Accessor = accessor;
      _MapClaimsToUserInfo = mapClaimsToUserInfo;
    }

    /// <summary>
    /// Gets the current user.
    /// </summary>
    /// <returns></returns>                                         
    public Task<IUserInfo> GetCurrentUser()
    {
      if (_Accessor.HttpContext.User.Identity.IsAuthenticated && _Accessor.HttpContext.User.Claims.Any())
      {
        return _MapClaimsToUserInfo(_Accessor.HttpContext.User.Claims);
      }
      else
      {
        return Task.FromResult<IUserInfo>(null);
      }
    }
  }
}
