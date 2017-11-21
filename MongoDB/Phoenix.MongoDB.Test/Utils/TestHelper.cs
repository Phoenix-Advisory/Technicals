using Phoenix.MongoDB.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Phoenix.MongoDB.Test.Utils
{
  internal static class TestHelper
  {
    public static void CheckList(IEnumerable<MyNamedEntity> expected, IEnumerable<MyNamedEntity> actual)
    {
      Assert.Equal(expected.Count(), actual.Count());
      foreach (MyNamedEntity entity in expected)
      {
        Assert.Contains(actual, x => x.Id == entity.Id);
      }

      foreach (MyNamedEntity entity in actual)
      {
        Assert.Contains(expected, x => x.Id == entity.Id);
      }
    }

    public static void CheckEntity(MyNamedEntity expected, MyNamedEntity actual)
    {
      Assert.Equal(expected.Date, actual.Date);
      Assert.Equal(expected.Id, actual.Id);
      Assert.Equal(expected.Value, actual.Value);
      Assert.Equal(expected.Name, actual.Name);
    }

    public static byte[] GenerateContent(int size)
    {
      byte[] res = new byte[size];
      new Random().NextBytes(res);
      return res;
    }

    public static MyNamedEntity[] CreateTestEntity(int count)
    {
      IList<MyNamedEntity> res = new List<MyNamedEntity>(count);
      for (int i = 0; i < count; i++)
      {
        res.Add(new MyNamedEntity
        {
          Id = Guid.NewGuid(),
          Name = $"Test {i}",
          Date = new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMinutes(i),
          Value = i
        }
        );
      }

      return res.ToArray();
    }
  }
}
