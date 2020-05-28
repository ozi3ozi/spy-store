using System;
using System.Collections.Generic;
using System.Text;
using SpyStore.Dal.Repos.Base;
using SpyStore.Models.Entities;

namespace SpyStore.Dal.Repos.Interfaces
{
    public interface IProductRepo : IRepo<Product>
    {
        IList<Product> Search(string searchString);
        IList<Product> GetProductsForCategory(int categoryId);
        IList<Product> GetFeaturedWithCategoryName();
        Product GetOneWithCategoryName(int id);
    }
}
