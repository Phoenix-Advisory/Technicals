using Autofac;
using Phoenix.MongoDB.Configuration;
using Phoenix.MongoDB.Repositories;
using Phoenix.MongoDB.Test.Entities;
using Phoenix.MongoDB.Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Phoenix.MongoDB.Test.Repositories
{
  [Collection("MongoDb Repository")]
  public class MongoRepositoryTest
  {
    [Fact(DisplayName = "Repo Ctor with Entity derived class")]
    public void CtorEntityRepository()
    {
      IContainer container = DIHelperUtils.InitializeContainer();
      Assert.NotNull(container.ResolveNamed<MongoDbSetting>("Test"));
      Assert.Throws<ArgumentException>(() => new MongoRepository<MyTestEntity>("Test", null));
      Assert.Throws<ArgumentException>(() => new MongoRepository<MyTestEntity>(null, "Test"));
      Assert.Throws<ArgumentException>(() => new MongoRepository<MyTestEntity>(null));
    }

    [Fact(DisplayName = "Repo Ctor with Collection Name attribute")]
    public void CtorEntityWithCollectionNameAttributeRepository()
    {
      IContainer container = DIHelperUtils.InitializeContainer();
      Assert.NotNull(container.ResolveNamed<MongoDbSetting>("Test"));
      IMongoRepository<MyNamedEntity> test = new MongoRepository<MyNamedEntity>("Test");
      Assert.NotNull(test);
      Assert.NotNull(test.Database);
      Assert.NotNull(test.Collection);
      Assert.Equal("TestCol", test.CollectionName);
      test = new MongoRepository<MyNamedEntity>();
      Assert.NotNull(test);
      Assert.NotNull(test.Database);
      Assert.Equal("MyTestDb2", test.Database.DatabaseNamespace.DatabaseName);
      Assert.NotNull(test.Collection);
      Assert.Equal("TestCol", test.CollectionName);
    }

    [Fact(DisplayName = "Repo Ctor with Entity derived class and Col name param")]
    public void CtorEntityColNameParamRepository()
    {
      IContainer container = DIHelperUtils.InitializeContainer();
      Assert.NotNull(container.ResolveNamed<MongoDbSetting>("Test"));
      IMongoRepository<MyTestEntity> test = new MongoRepository<MyTestEntity>("Test", "Test");
      Assert.NotNull(test);
      Assert.NotNull(test.Database);
      Assert.NotNull(test.Collection);
      Assert.Equal("Test", test.CollectionName);
    }

    [Fact(DisplayName = "Repo Insert/Single operations")]
    public async Task Insert_Single()
    {
      IMongoRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(2);
      await repo.Create(expected[0]).ConfigureAwait(false);
      // Single
      MyNamedEntity res = await repo.GetSingle(expected[0].Id).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[0], res);
      res = await repo.GetSingle(expected[0].Id).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[0], res);
      res = await repo.GetSingleOrDefault(expected[0].Id).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[0], res);
      res = await repo.GetSingleOrDefault(Guid.NewGuid()).ConfigureAwait(false);
      Assert.Null(res);
      res = await repo.GetSingleOrDefault(Guid.NewGuid(), expected[1]).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[1], res);
    }         

    [Fact(DisplayName = "Repo Insert/First operations")]
    public async Task Insert_First()
    {
      IMongoRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(2);
      await repo.Create(expected[0]).ConfigureAwait(false);
      MyNamedEntity res = await repo.GetFirst(expected[0].Id).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[0], res);
      res = await repo.GetFirst(expected[0].Id).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[0], res);
      res = await repo.GetFirstOrDefault(expected[0].Id).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[0], res);
      res = await repo.GetFirstOrDefault(Guid.NewGuid()).ConfigureAwait(false);
      Assert.Null(res);
      res = await repo.GetFirstOrDefault(Guid.NewGuid(), expected[1]).ConfigureAwait(false);
      Assert.NotNull(res);
      TestHelper.CheckEntity(expected[1], res);
    }

    [Fact(DisplayName = "Repo InsertMany/Count/Delete operations")]
    public async Task InsertManyCountDelete()
    {
      IMongoRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(30);
      await repo.Create(expected).ConfigureAwait(false);
      Assert.Equal(30, await repo.CountLong().ConfigureAwait(false));
      Assert.Equal(30, await repo.Count().ConfigureAwait(false));


      Assert.Equal(1, await repo.CountLong(x => x.Id == expected[0].Id).ConfigureAwait(false));
      Assert.Equal(1, await repo.Count(x => x.Id == expected[0].Id).ConfigureAwait(false));

      Assert.True(await repo.Delete(expected[0].Id).ConfigureAwait(false));
      Assert.False(await repo.Delete(Guid.NewGuid()).ConfigureAwait(false));
      Assert.Equal(29, await repo.CountLong().ConfigureAwait(false));
      Assert.Equal(29, await repo.Count().ConfigureAwait(false));
      Assert.Equal(1, await repo.Delete(x => x.Value < 2).ConfigureAwait(false));
      Assert.Equal(28, await repo.CountLong().ConfigureAwait(false));
      Assert.Equal(28, await repo.Count().ConfigureAwait(false));

      Assert.Equal(28, await repo.Delete(x => true).ConfigureAwait(false));
      Assert.Equal(0, await repo.CountLong().ConfigureAwait(false));
      Assert.Equal(0, await repo.Count().ConfigureAwait(false));
    }

    [Fact(DisplayName = "Repo Update operations")]
    public async Task Update()
    {
      IMongoRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(30);
      await repo.Create(expected).ConfigureAwait(false);
      foreach (MyNamedEntity ent in expected)
      {
        ent.Date = ent.Date.AddDays(1);
        ent.Value = ent.Value * 100;
        ent.Name = "Updated " + ent.Name;
        await repo.Update(ent).ConfigureAwait(false);
      }
      TestHelper.CheckList(expected, await repo.GetMany().ConfigureAwait(false));
      foreach (MyNamedEntity ent in expected)
      {
        ent.Date = ent.Date.AddDays(1);
        ent.Value = ent.Value + 100;
        ent.Name = "Updated 2 " + ent.Name;
      }
      await repo.UpdateMany(expected).ConfigureAwait(false);
      TestHelper.CheckList(expected, await repo.GetMany().ConfigureAwait(false));
    }

    [Fact(DisplayName = "Repo GetMany operations")]
    public async Task GetMany()
    {
      IMongoRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(30);
      await repo.Create(expected).ConfigureAwait(false);

      IEnumerable<MyNamedEntity> result = await repo.GetMany().ConfigureAwait(false);
      TestHelper.CheckList(expected, result);

      result = await repo.GetMany(x => x.Value < 5).ConfigureAwait(false);
      TestHelper.CheckList(expected.Where(x => x.Value < 5), result);
    }

    private async Task<IMongoRepository<T>> InitializeRepo<T>() where T : class
    {
      IMongoRepository<T> res = new MongoRepository<T>("Test");
      await res.Database.DropCollectionAsync(res.CollectionName).ConfigureAwait(false);
      return res;
    }
  }
}
