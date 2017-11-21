using Phoenix.MongoDB.Attributes;
using System;        

namespace Phoenix.MongoDB.Test.Entities
{
  [Database("Test2")]
  internal class MyTestEntity : Entity<Guid>
  {
    public string Name { get; set; }
    public int Value { get; set; }
    public DateTime Date { get; set; }
  }
}
