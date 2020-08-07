using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.Dal.EfStructures;
using SpyStore.Models.Entities;

namespace SpyStore.Dal.Initialization
{
    public static class SampleDataInitializer
    {
        public static void DropAndCreateDatabase(StoreContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        //This method reseeds the identity column for all tables in the database.
        internal static void ResetIdentity(StoreContext context)
        {
            var tables = new[] {"Categories","Customers",
                "OrderDetails","Orders","Products","ShoppingCartRecords"};
            foreach (string table in tables)
            {
                string rawSqlString = $"DBCC CHECKIDENT (\"Store.{table}\", RESEED, 0);";
#pragma warning disable EF100 //Possible SQL injection vulnerability.
                context.Database.ExecuteSqlCommand(rawSqlString);
#pragma warning restore EF100 //Possible SQL injection vulnerability.
            }
        }

        //This method clears the data from the database and then calls the ResetIdentity method. 
        //The default for the one-to-many relationships as set up in the SpyStore entities is to enable 
        //cascade delete. This enables the entire database to be cleared by deleting the Customer records 
        //and the Category records (in that order)
        public static void ClearData(StoreContext context)
        {
            context.Database.ExecuteSqlCommand("Delete from Store.Categories");
            context.Database.ExecuteSqlCommand("Delete from Store.Customers");
            ResetIdentity(context);
        }

        //This method adds the object graphs from the SampleData methods into an instance of the 
        //StoreContext and then persists the data to the database.
        internal static void SeedData(StoreContext context)
        {
            try
            {
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(SampleData.GetCategories());
                    context.SaveChanges();
                }
                if (!context.Customers.Any())
                {
                    var prod1 = context.Categories
                        .Include(c => c.Products).FirstOrDefault()?
                        .Products.Skip(3).FirstOrDefault();
                    var prod2 = context.Categories.Skip(2)
                        .Include(c => c.Products).FirstOrDefault()?
                        .Products.Skip(2).FirstOrDefault();
                    var prod3 = context.Categories.Skip(5)
                        .Include(c => c.Products).FirstOrDefault()?
                        .Products.Skip(1).FirstOrDefault();
                    var prod4 = context.Categories.Skip(2)
                        .Include(c => c.Products).FirstOrDefault()?
                        .Products.Skip(1).FirstOrDefault();
                    context.Customers.AddRange(SampleData.GetAllCustomerRecords(
                        new List<Product> { prod1, prod2, prod3, prod4 }));
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //This method is the InitializeData method . This method ensures the database is created 
        //and the migrations are applied and then clears and seeds the data.
        public static void InitializeData(StoreContext context)
        {
            //Ensure the database exists and is up to date
            context.Database.Migrate();
            ClearData(context);
            SeedData(context);
        }
    }
}
