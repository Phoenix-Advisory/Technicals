using Phoenix.Core.ParameterGuard;
using System;        
using Xunit;

namespace Phoenix.Core.Test.ParameterGuard
{
  [Collection("Guard")]
  public class GuardTest
  {
    private const string ParamName = "MyParameter";
    [Fact(DisplayName = "Guard IsNotNull")]
    public void IsNotNullTest()
    {
      object o = null;
      Assert.Throws<ArgumentException>(ParamName, () => Guard.IsNotNull(o, ParamName));
      o = new object();
      Assert.IsType<FluentGuard<object>>(Guard.IsNotNull(o, ParamName));

      Assert.IsType<FluentGuard<string>>(Guard.IsNotNull(string.Empty, ParamName));
    }

    [Fact(DisplayName = "Guard IsNotNullOrWhiteSpace")]
    public void IsNotNullOrWhiteSpaceTest()
    {
      string o = null;
      Assert.Throws<ArgumentException>(ParamName, () => Guard.IsNotNullOrWhiteSpace(o, ParamName));
      o = string.Empty;
      Assert.Throws<ArgumentException>(ParamName, () => Guard.IsNotNullOrWhiteSpace(o, ParamName));
      o = "         ";
      Assert.Throws<ArgumentException>(ParamName, () => Guard.IsNotNullOrWhiteSpace(o, ParamName));
      o = ParamName;
      Assert.IsType<FluentGuard<string>>(Guard.IsNotNullOrWhiteSpace(o, ParamName));
    }

    [Fact(DisplayName = "Guard IsNotNullOrEmpty")]
    public void IsNotNullOrEmptyTest()
    {
      string o = null;
      Assert.Throws<ArgumentException>(ParamName, () => Guard.IsNotNullOrEmpty(o, ParamName));
      o = string.Empty;
      Assert.Throws<ArgumentException>(ParamName, () => Guard.IsNotNullOrEmpty(o, ParamName));
      o = "         ";
      Assert.IsType<FluentGuard<string>>(Guard.IsNotNullOrEmpty(o, ParamName));
      o = ParamName;
      Assert.IsType<FluentGuard<string>>(Guard.IsNotNullOrEmpty(o, ParamName));
    }
  }
}
