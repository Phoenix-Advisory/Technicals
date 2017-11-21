using System;
using System.Collections.Generic;
using Phoenix.Core.Collections;
using System.Text;
using Xunit;

namespace Phoenix.Core.Test.Collections
{
  [Collection("IListExtensions")]
  public class IListExtensionsTest
  {
    [Fact(DisplayName = "Add range (skip null values)")]
    public void AddRangeSkipNull()
    {
      object[] source = new[] { new object(), new object(), null };
      IList<object> destination = new List<object>();
      destination.AddRange(source, true);
      Assert.Equal(2, destination.Count);
      Assert.Same(source[0], destination[0]);
      Assert.Same(source[1], destination[1]);

      destination = new List<object> { new object() };
      destination.AddRange(source);
      Assert.Equal(3, destination.Count);
      Assert.Same(source[0], destination[1]);
      Assert.Same(source[1], destination[2]); 
    }

    [Fact(DisplayName = "Add range (Not nullable type)")]
    public void AddRangeNotNullable()
    {
      IList<int> destination = new List<int>();
      destination.AddRange(new[] { 1, 2, 3 }, true);
      Assert.Equal(3, destination.Count);
    }

    [Fact(DisplayName = "Add range (Include null values)")]
    public void AddRangeWithNull()
    {
      object[] source = new[] { new object(), new object(), null };
      IList<object> destination = new List<object>();
      destination.AddRange(source, false);
      Assert.Equal(3, destination.Count);
      Assert.Same(source[0], destination[0]);
      Assert.Same(source[1], destination[1]);
      Assert.Same(source[2], destination[2]);

      destination = new List<object> { new object() };
      destination.AddRange(source, false);
      Assert.Equal(4, destination.Count);
      Assert.Same(source[0], destination[1]);
      Assert.Same(source[1], destination[2]);
      Assert.Same(source[2], destination[3]);
    }

    [Fact(DisplayName = "Add range (where destination is null)")]
    public void AddRangeNullDestination()
    {
      IList<object> destination = null;
      Assert.Throws<ArgumentException>(()=> {
        destination.AddRange(new[] { new object(), new object(), null }, false);
      });
    }
  }
}
