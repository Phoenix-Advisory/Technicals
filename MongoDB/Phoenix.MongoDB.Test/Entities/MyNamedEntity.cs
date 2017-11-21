using Phoenix.MongoDB.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.MongoDB.Test.Entities
{
  [CollectionName("TestCol")]
  [BucketName("TestBuck")]
  internal class MyNamedEntity : MyTestEntity
  {  
  }
}
