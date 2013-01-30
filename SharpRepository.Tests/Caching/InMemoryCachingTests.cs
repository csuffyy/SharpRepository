﻿using System.Linq;
using System.Runtime.Caching;
using SharpRepository.InMemoryRepository;
using NUnit.Framework;
using SharpRepository.Repository.Caching;
using SharpRepository.Repository.Queries;
using SharpRepository.Tests.TestObjects;
using Should;

namespace SharpRepository.Tests.Caching
{
    [TestFixture]
    public class InMemoryCachingTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
            // need to clear out the InMemory cache before each test is run so that each is independent and won't effect the next one
            var cache = MemoryCache.Default;
            foreach (var item in cache)
            {
                cache.Remove(item.Key);
            }
        }

        [TearDown]
        public void Teardown()
        {

        }

        [Test]
        public void ExecuteGetAll_With_Selector_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1"});
            repos.Add(new Contact { Name = "Test2"});

            var items = repos.GetAll(x => x.Name);
            repos.CacheUsed.ShouldBeFalse();
            items.Count().ShouldEqual(2);

            items = repos.GetAll(x => x.Name);
            repos.CacheUsed.ShouldBeTrue();
            items.Count().ShouldEqual(2);
        }

        [Test]
        public void ExecuteGetAll_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1" });
            repos.Add(new Contact { Name = "Test2" });

            var items = repos.GetAll();
            repos.CacheUsed.ShouldBeFalse();
            items.Count().ShouldEqual(2);

            items = repos.GetAll();
            repos.CacheUsed.ShouldBeTrue();
            items.Count().ShouldEqual(2);
        }

        [Test]
        public void ExecuteFindAll_With_Selector_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1" });
            repos.Add(new Contact { Name = "Test2" });

            var items = repos.FindAll(x => x.ContactId < 3, x => x.Name);
            repos.CacheUsed.ShouldBeFalse();
            items.Count().ShouldEqual(2);

            items = repos.FindAll(x => x.ContactId < 3, x => x.Name);
            repos.CacheUsed.ShouldBeTrue();
            items.Count().ShouldEqual(2);
        }

        [Test]
        public void ExecuteFindAll_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1" });
            repos.Add(new Contact { Name = "Test2" });

            var items = repos.FindAll(x => x.ContactId < 3);
            repos.CacheUsed.ShouldBeFalse();
            items.Count().ShouldEqual(2);

            items = repos.FindAll(x => x.ContactId < 3);
            repos.CacheUsed.ShouldBeTrue();
            items.Count().ShouldEqual(2);
        }

        [Test]
        public void ExecuteFind_With_Selector_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1" });
            repos.Add(new Contact { Name = "Test2" });

            var item = repos.Find(x => x.ContactId == 1, x => x.Name);
            repos.CacheUsed.ShouldBeFalse();
            item.ShouldNotBeNull();

            item = repos.Find(x => x.ContactId ==1, x => x.Name);
            repos.CacheUsed.ShouldBeTrue();
            item.ShouldNotBeNull();
        }

        [Test]
        public void ExecuteFind_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1" });
            repos.Add(new Contact { Name = "Test2" });

            var item= repos.Find(x => x.ContactId == 1);
            repos.CacheUsed.ShouldBeFalse();
            item.ShouldNotBeNull();

            item = repos.Find(x => x.ContactId == 1);
            repos.CacheUsed.ShouldBeTrue();
            item.ShouldNotBeNull();
        }

        [Test]
        public void ExecuteGet_With_Selector_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1" });
            repos.Add(new Contact { Name = "Test2" });

            var item = repos.Get(1, x => x.Name);
            repos.CacheUsed.ShouldBeFalse();
            item.ShouldNotBeNull();

            item = repos.Get(1, x => x.Name);
            repos.CacheUsed.ShouldBeTrue();
            item.ShouldNotBeNull();
        }

        [Test]
        public void ExecuteGet_Should_Use_Cache_After_First_Call()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { Name = "Test1" });
            repos.Add(new Contact { Name = "Test2" });

            var item= repos.Get(1);
            repos.CacheUsed.ShouldBeTrue();
            item.ShouldNotBeNull();
        }

        [Test]
        public void ExecuteFindAll_With_Paging_Should_Save_TotalItems_In_Cache()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { ContactId = 1, Name = "Test1" });
            repos.Add(new Contact { ContactId = 2, Name = "Test2" });
            repos.Add(new Contact { ContactId = 3, Name = "Test3" });
            repos.Add(new Contact { ContactId = 4, Name = "Test4" });

            var pagingOptions = new PagingOptions<Contact>(1, 1, "Name");

            var items = repos.FindAll(x => x.ContactId >= 2, x => x.Name, pagingOptions);
            repos.CacheUsed.ShouldBeFalse();
            items.Count().ShouldEqual(1);
            pagingOptions.TotalItems.ShouldEqual(3);

            // reset paging options so the TotalItems is default
            pagingOptions = new PagingOptions<Contact>(1, 1, "Name");

            items = repos.FindAll(x => x.ContactId >= 2, x => x.Name, pagingOptions);
            repos.CacheUsed.ShouldBeTrue();
            items.Count().ShouldEqual(1);
            pagingOptions.TotalItems.ShouldEqual(3);
        }

        [Test]
        public void ExecuteGetAll_With_Paging_Should_Save_TotalItems_In_Cache()
        {
            var repos = new InMemoryRepository<Contact>(new StandardCachingStrategy<Contact>());

            repos.Add(new Contact { ContactId = 1, Name = "Test1" });
            repos.Add(new Contact { ContactId = 2, Name = "Test2" });
            repos.Add(new Contact { ContactId = 3, Name = "Test3" });
            repos.Add(new Contact { ContactId = 4, Name = "Test4" });

            var pagingOptions = new PagingOptions<Contact>(1, 1, "Name");

            var items = repos.GetAll(x => x.Name, pagingOptions);
            repos.CacheUsed.ShouldBeFalse();
            items.Count().ShouldEqual(1);
            pagingOptions.TotalItems.ShouldEqual(4);

            // reset paging options so the TotalItems is default
            pagingOptions = new PagingOptions<Contact>(1, 1, "Name");

            items = repos.GetAll(x => x.Name, pagingOptions);
            repos.CacheUsed.ShouldBeTrue();
            items.Count().ShouldEqual(1);
            pagingOptions.TotalItems.ShouldEqual(4);
        }
    }
}
