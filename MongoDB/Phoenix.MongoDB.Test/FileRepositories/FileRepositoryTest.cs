using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Phoenix.MongoDB.FileRepositories;
using Phoenix.MongoDB.Test.Entities;
using Phoenix.MongoDB.Test.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Phoenix.MongoDB.Test.FileRepositories
{
  [Collection("MongoDb FileRepository")]
  public class FileRepositoryTest
  {
    [Fact(DisplayName = "FileRepo Ctor")]
    public void Ctor()
    {
      IServiceProvider container = DIHelperUtils.InitializeContainer();
      Assert.Throws<ArgumentException>(() => new MongoFileRepository<MyTestEntity>("Test", container.GetService<ConnectionManager>()));
      Assert.Throws<ArgumentException>(() => new MongoFileRepository<MyTestEntity>("Test", "TestBadFile", null));
      IMongoFileRepository<MyNamedEntity> repo = new MongoFileRepository<MyNamedEntity>("Test", "TestFile", container.GetService<ConnectionManager>());
      Assert.NotNull(repo);
      Assert.NotNull(repo.Database);
      Assert.NotNull(repo.Bucket);
      Assert.Equal("TestFile", repo.BucketName);
      repo = new MongoFileRepository<MyNamedEntity>(container.GetService<ConnectionManager>());
      Assert.NotNull(repo);
      Assert.NotNull(repo.Database);
      Assert.Equal("MyTestDb2", repo.Database.DatabaseNamespace.DatabaseName);
      Assert.NotNull(repo.Bucket);
      Assert.Equal("TestBuck", repo.BucketName);
    }

    [Fact(DisplayName = "FileRepo Insert")]
    public async Task Insert()
    {
      IMongoFileRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(1);
      await repo.Create(TestHelper.GenerateContent(20), expected[0].Name + ".bin", expected[0]).ConfigureAwait(false);
      Assert.Equal(1, await repo.Count(x => true).ConfigureAwait(false));
      FileInfo<MyNamedEntity> info = await repo.GetFirst(x => true).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);
      byte[] content = await repo.GetContent(info.Id).ConfigureAwait(false);
      Assert.Equal(20, content.Length);
    }

    [Fact(DisplayName = "FileRepo First")]
    public async Task GetFirst()
    {
      IMongoFileRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(1);
      await repo.Create(TestHelper.GenerateContent(20), expected[0].Name + ".bin", expected[0]).ConfigureAwait(false);
      FileInfo<MyNamedEntity> info = await repo.GetFirst(x => x.Metadata.Id == expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetFirst(expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetFirst(info.Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetFirstOrDefault(x => x.Metadata.Id == expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetFirstOrDefault(expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetFirstOrDefault(info.Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetFirstOrDefault(x => x.Metadata.Id == Guid.NewGuid()).ConfigureAwait(false);
      Assert.Null(info);

      info = await repo.GetFirstOrDefault(Guid.NewGuid()).ConfigureAwait(false);
      Assert.Null(info);

      info = await repo.GetFirstOrDefault(new ObjectId()).ConfigureAwait(false);
      Assert.Null(info);
    }

    [Fact(DisplayName = "FileRepo Single")]
    public async Task GetSingle()
    {
      IMongoFileRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(1);
      await repo.Create(TestHelper.GenerateContent(20), expected[0].Name + ".bin", expected[0]).ConfigureAwait(false);
      FileInfo<MyNamedEntity> info = await repo.GetSingle(x => x.Metadata.Id == expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetSingle(expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetSingle(info.Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetSingleOrDefault(x => x.Metadata.Id == expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetSingleOrDefault(expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetSingleOrDefault(info.Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      info = await repo.GetSingleOrDefault(x => x.Metadata.Id == Guid.NewGuid()).ConfigureAwait(false);
      Assert.Null(info);

      info = await repo.GetSingleOrDefault(Guid.NewGuid()).ConfigureAwait(false);
      Assert.Null(info);

      info = await repo.GetSingleOrDefault(new ObjectId()).ConfigureAwait(false);
      Assert.Null(info);
    }

    [Fact(DisplayName = "FileRepo Exists")]
    public async Task Exists()
    {
      IMongoFileRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(1);
      await repo.Create(TestHelper.GenerateContent(20), expected[0].Name + ".bin", expected[0]).ConfigureAwait(false);
      FileInfo<MyNamedEntity> info = await repo.GetSingle(x => x.Metadata.Id == expected[0].Id).ConfigureAwait(false);
      Assert.True(await repo.Exists(expected[0].Id).ConfigureAwait(false));
      Assert.True(await repo.Exists(info.Id).ConfigureAwait(false));
      Assert.False(await repo.Exists(Guid.NewGuid()).ConfigureAwait(false));
      Assert.False(await repo.Exists(new ObjectId()).ConfigureAwait(false));
    }

    [Fact(DisplayName = "FileRepo Update")]
    public async Task Update()
    {
      IMongoFileRepository<MyNamedEntity> repo = await InitializeRepo<MyNamedEntity>().ConfigureAwait(false);
      MyNamedEntity[] expected = TestHelper.CreateTestEntity(2);
      await repo.Create(TestHelper.GenerateContent(20), expected[0].Name + ".bin", expected[0]).ConfigureAwait(false);
      FileInfo<MyNamedEntity> info = await repo.GetSingle(x => x.Metadata.Id == expected[0].Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[0], info.Metadata);
      Assert.Equal(expected[0].Name + ".bin", info.Filename);

      await repo.Update(info.Id, TestHelper.GenerateContent(30), expected[1].Name + ".bin", expected[1]).ConfigureAwait(false);
      info = await repo.GetSingle(info.Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[1], info.Metadata);
      Assert.Equal(expected[1].Name + ".bin", info.Filename);
      byte[] content = await repo.GetContent(info.Id).ConfigureAwait(false);
      Assert.Equal(30, content.Length);

      expected[1].Value = expected[1].Value + 3;
      expected[1].Date = expected[1].Date.AddDays(1);
      await repo.Update(expected[1].Id, TestHelper.GenerateContent(40), expected[1].Name + "-updated.bin", expected[1]).ConfigureAwait(false);
      info = await repo.GetSingle(info.Id).ConfigureAwait(false);
      TestHelper.CheckEntity(expected[1], info.Metadata);
      Assert.Equal(expected[1].Name + "-updated.bin", info.Filename);
      content = await repo.GetContent(info.Id).ConfigureAwait(false);
      Assert.Equal(40, content.Length);
    }

    private async Task<IMongoFileRepository<T>> InitializeRepo<T>() where T : class
    {
      IServiceProvider container = DIHelperUtils.InitializeContainer();                                     
      MongoFileRepository<T> res = new MongoFileRepository<T>("Test", "TestFile", container.GetService<ConnectionManager>());
      await res.DropCollection().ConfigureAwait(false);
      return res;
    }
  }
}
