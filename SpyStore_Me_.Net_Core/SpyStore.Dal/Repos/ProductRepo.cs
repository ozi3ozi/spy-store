using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.Dal.EfStructures;
using SpyStore.Dal.Repos.Base;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Models.Entities;

namespace SpyStore.Dal.Repos
{
    public class ProductRepo : RepoBase<Product>, IProductRepo
    {
        public ProductRepo(StoreContext storeContext) : base(storeContext)
        {

        }

        internal ProductRepo(DbContextOptions<StoreContext> options) : base(options)
        {

        }

        //return the Product records in order of the ModelName property
        public override IEnumerable<Product> GetAll() => base.GetAll(x => x.Details.ModelName);

        public IList<Product> GetProductsForCategory(int categoryId)
            => Table.Where(p => p.CategoryId == categoryId)
                .Include(p => p.CategoryNavigation)
                .OrderBy(p => p.Details.ModelName)
                .ToList();

        public IList<Product> GetFeaturedWithCategoryName()
            => Table.Where(p => p.IsFeatured)
                .Include(p => p.CategoryNavigation)
                .OrderBy(p => p.Details.ModelName)
                .ToList();

        public Product GetOneWithCategoryName(int id)
            => Table.Where(p => p.Id == id)
                .Include(p => p.CategoryNavigation)
                .FirstOrDefault();

        public IList<Product> Search(string searchString)
            => Table.Where(p => EF.Functions.Like(p.Details.Description, $"%{searchString}%")
             || EF.Functions.Like(p.CategoryName, $"%{searchString}%"))
                .Include(p => p.CategoryNavigation)
                .OrderBy(p => p.Details.ModelName)
                .ToList();

    }
}
