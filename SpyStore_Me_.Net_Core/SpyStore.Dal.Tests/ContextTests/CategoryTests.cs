using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.Dal.EfStructures;
using SpyStore.Dal.Initialization;
using SpyStore.Models.Entities;
using Xunit;

//The rest of the tests (73) can be found her: 
//https://github.com/Apress/building-web-apps/tree/master/Chapter%204/SpyStore.Dal.Tests
namespace SpyStore.Dal.Tests.ContextTests
{
    //All classes with the Collection attribute and the same key have their tests run in serial.
    [Collection("SpyStore.Dal")]
    public class CategoryTests : IDisposable
    {
        private readonly StoreContext _db;

        public CategoryTests()
        {
            _db = new StoreContextFactory().CreateDbContext(new string[0]);
            CleanDatabase();
        }

        public void Dispose()
        {
            CleanDatabase();
            _db.Dispose();
        }

        private void CleanDatabase()
        {
            SampleDataInitializer.ClearData(_db);
        }

        [Fact]
        public void FirstTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldAddACategoryWithDbSet()
        {
            var category = new Category() { CategoryName = "Foo" };
            _db.Categories.Add(category);
            Assert.Equal(EntityState.Added, _db.Entry(category).State);
            Assert.True(category.Id < 0);
            Assert.Null(category.TimeStamp);
            _db.SaveChanges();
            Assert.Equal(EntityState.Unchanged, _db.Entry(category).State);
            Assert.NotNull(category.TimeStamp);
            Assert.Equal(1, _db.Categories.Count());
        }

        [Fact]
        public void ShouldAddACategoryWithContext()
        {
            var category = new Category() { CategoryName = "Foo" };
            _db.Add(category);
            Assert.Equal(EntityState.Added, _db.Entry(category).State);
            Assert.True(category.Id < 0);
            Assert.Null(category.TimeStamp);
            _db.SaveChanges();
            Assert.Equal(EntityState.Unchanged, _db.Entry(category).State);
            Assert.NotNull(category.TimeStamp);
            Assert.Equal(1, _db.Categories.Count());
        }

        [Fact]
        public void ShouldGetAllCategoriesOrderedByName()
        {
            var cat1 = new Category { CategoryName = "Cat1" };
            var cat2 = new Category { CategoryName = "Cat2" };
            _db.Categories.AddRange(cat1, cat2);
            _db.SaveChanges();
            var categories = _db.Categories.OrderBy(c => c.CategoryName).ToList();
            Assert.Equal(2, categories.Count());
            Assert.Equal("Cat1", categories[0].CategoryName);
            Assert.Equal("Cat2", categories[1].CategoryName);
        }

        [Fact]
        public void ShouldUpdateACategory()
        {
            var cat = new Category { CategoryName = "cat1" };
            _db.Categories.Add(cat);
            _db.SaveChanges();
            cat.CategoryName = "Modified";
            _db.Categories.Update(cat);
            Assert.Equal(EntityState.Modified, _db.Entry(cat).State);
            _db.SaveChanges();
            Assert.Equal(EntityState.Unchanged, _db.Entry(cat).State);
            StoreContext context;
            using (context = new StoreContextFactory().CreateDbContext(null))
            {
                Assert.Equal("Modified", context.Categories.First().CategoryName);
            }
        }

        //It’s important to note that Update() only works for persisted records 
        //that have been changed. If you try to call Update() on an entity that 
        //is new, EF will throw an InvalidOperationException. 
        //This test shows that.
        [Fact]
        public void ShouldNotUpdateANonAttachedCategory()
        {
            var cat = new Category { CategoryName = "cat1" };
            _db.Categories.Add(cat);
            cat.CategoryName = "Modified";
            Assert.Throws<InvalidOperationException>(
                () => _db.Categories.Update(cat)
            );
        }

        [Fact]
        public void ShouldDeleteACategory()
        {
            var cat = new Category { CategoryName = "cat1" };
            _db.Categories.Add(cat);
            _db.SaveChanges();
            Assert.Equal(1, _db.Categories.Count());
            _db.Categories.Remove(cat);
            Assert.Equal(EntityState.Deleted, _db.Entry(cat).State);
            _db.SaveChanges();
            Assert.Equal(EntityState.Detached, _db.Entry(cat).State);
            Assert.Equal(0, _db.Categories.Count());
        }

        //Remove only works on tracked entities. If the instance was not being 
        //tracked, it would have to be loaded from the database into the 
        //ChangeTracker to be deleted, or delete it using EntityState. 
        //Shown in the next test.
        [Fact]
        public void ShouldDeleteACategoryWithTimestampData()
        {
            var cat = new Category { CategoryName = "cat1" };
            _db.Categories.Add(cat);
            _db.SaveChanges();
            Assert.Equal(1, _db.Categories.Count());
            StoreContext context = new StoreContextFactory().CreateDbContext(null);
            var catToDelete = new Category
            {
                Id = cat.Id,
                TimeStamp = cat.TimeStamp
            };
            context.Entry(catToDelete).State = EntityState.Deleted;
            int affected = context.SaveChanges();
            Assert.Equal(1, affected);
        }
    }
}
