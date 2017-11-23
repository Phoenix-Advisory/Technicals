using Phoenix.Core.Comparer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Phoenix.Core.Test.Comparer
{
  [Collection("ObjectComparer")]
  public class ObjectComparerTest
  {
    [Fact(DisplayName = "ObjectComparer CheckNoDif")]
    public void CheckNoDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      Assert.Empty(ObjectComparer.Compare(obj1, obj2));
      obj2.Char = 'z';
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("Char", resultItem.PropertyName);
      Assert.Equal(obj1.Char, resultItem.OldValue);
      Assert.Equal(obj2.Char, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckCharDif")]
    public void CheckCharDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.Char = 'z';
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("Char", resultItem.PropertyName);
      Assert.Equal(obj1.Char, resultItem.OldValue);
      Assert.Equal(obj2.Char, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckStringDif")]
    public void CheckStringDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.String = "Test New Value";
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("String", resultItem.PropertyName);
      Assert.Equal(obj1.String, resultItem.OldValue);
      Assert.Equal(obj2.String, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckEnumDif")]
    public void CheckEnumDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.MyEnum = TestEnum.Val2;
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("MyEnum", resultItem.PropertyName);
      Assert.Equal(obj1.MyEnum, resultItem.OldValue);
      Assert.Equal(obj2.MyEnum, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckArrayDif")]
    public void CheckArrayDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.Array = new TestClass2[0];
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("Array", resultItem.PropertyName);
      Assert.Equal(obj1.Array, resultItem.OldValue);
      Assert.Equal(obj2.Array, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckListDif")]
    public void CheckListDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.List = new List<TestClass2>();
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("List", resultItem.PropertyName);
      Assert.Equal(obj1.List, resultItem.OldValue);
      Assert.Equal(obj2.List, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckDictionaryDif")]
    public void CheckDictionaryDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.Dictionary = new Dictionary<string, TestClass2>();
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("Dictionary", resultItem.PropertyName);
      Assert.Equal(obj1.Dictionary, resultItem.OldValue);
      Assert.Equal(obj2.Dictionary, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckCollectionDif")]
    public void CheckCollectionDif()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.Collection = new ArrayList();
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("Collection", resultItem.PropertyName);
      Assert.Equal(obj1.Collection, resultItem.OldValue);
      Assert.Equal(obj2.Collection, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckSubObject")]
    public void CheckSubObject()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.Test2.Guid = Guid.NewGuid();
      Assert.Single(ObjectComparer.Compare(obj1, obj2));
      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("Test2.Guid", resultItem.PropertyName);
      Assert.Equal(obj1.Test2.Guid, resultItem.OldValue);
      Assert.Equal(obj2.Test2.Guid, resultItem.NewValue);
    }

    [Fact(DisplayName = "ObjectComparer CheckSubObjectWithNull")]
    public void CheckSubObjectWithNull()
    {
      TestClass1 obj1 = CreateInstance();
      TestClass1 obj2 = CreateInstance();
      obj2.Test2 = null;

      Assert.Equal(2, ObjectComparer.Compare(obj1, obj2).Count());

      CompareResult resultItem = ObjectComparer.Compare(obj1, obj2).First();
      Assert.Equal("Test2.Guid", resultItem.PropertyName);
      Assert.Equal(obj1.Test2.Guid, resultItem.OldValue);
      Assert.Null(resultItem.NewValue);

      resultItem = ObjectComparer.Compare(obj2, obj1).First();
      Assert.Equal("Test2.Guid", resultItem.PropertyName);
      Assert.Equal(obj1.Test2.Guid, resultItem.NewValue);
      Assert.Null(resultItem.OldValue);
    }

    private static TestClass1 CreateInstance()
    {
      return new TestClass1
      {
        Char = 'a',
        Date = DateTime.MinValue,
        Guid = Guid.Empty,
        Int = 1,
        MyEnum = TestEnum.Val1,
        String = "String1",
        Test2 = new TestClass2
        {
          DateOffset = DateTimeOffset.MinValue,
          Guid = Guid.Empty
        },
        Array = new TestClass2[] {
          new TestClass2 {
            DateOffset = DateTimeOffset.MinValue,
            Guid = Guid.Empty
          }
        },
        List = new List<TestClass2> {
          new TestClass2 {
            DateOffset = DateTimeOffset.MinValue,
            Guid = Guid.Empty
          }
        },
        Dictionary = new Dictionary<string, TestClass2> {
          {
            "Test",
            new TestClass2 {
              DateOffset = DateTimeOffset.MinValue,
              Guid = Guid.Empty
            }
          }
        },
        Collection = new ArrayList {
          new TestClass2 {
            DateOffset = DateTimeOffset.MinValue,
            Guid = Guid.Empty
          }
        }
      };
    }
  }
}
