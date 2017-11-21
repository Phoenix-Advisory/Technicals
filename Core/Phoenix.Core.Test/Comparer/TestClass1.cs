using System;
using System.Collections;
using System.Collections.Generic;

namespace Phoenix.Core.Test.Comparer
{
  internal class TestClass1
  {
    public DateTime Date { get; set; }
    public int Int { get; set; }
    public char Char { get; set; }
    public string String { get; set; }
    public TestEnum MyEnum { get; set; }
    public Guid Guid { get; set; }
    public TestClass2 Test2 { get; set; }

    public TestClass2[] Array { get; set; }
    public IList<TestClass2> List { get; set; }
    public IDictionary<string, TestClass2> Dictionary { get; set; }
    public ICollection Collection { get; set; }
  }
}
