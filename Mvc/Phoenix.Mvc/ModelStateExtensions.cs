using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Phoenix.Mvc
{
  /// <summary>
  /// Extend ModelState.
  /// </summary>
  public static class ModelStateExtensions
  {
    /// <summary>
    /// Adds the identity errors to model.
    /// </summary>
    /// <param name="modelState">State of the model.</param>
    /// <param name="identityResult">The identity result.</param>
    /// <returns></returns>
    public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, IdentityResult identityResult)
    {
      foreach (var e in identityResult.Errors)
      {
        modelState.AddError(e.Code, e.Description);
      }

      return modelState;
    }

    /// <summary>
    /// Adds the error to ModelState.
    /// </summary>
    /// <param name="modelState">State of the model.</param>
    /// <param name="code">The code.</param>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    public static ModelStateDictionary AddError(this ModelStateDictionary modelState, string code, string description)
    {
      modelState.TryAddModelError(code, description);
      return modelState;
    }
  }
}
