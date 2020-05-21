using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpyStore.Models.Entities;
using SpyStore.Models.Entities.Base;
using SpyStore.Models.ViewModels;


namespace SpyStore.Dal.EfStructures
{
    public class StoreContext : DbContext
    {
        //Used in the global query filter
        public int CustomerId { get; set; } 

        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
            
        }

        //SQL Server scalar functions can be mapped to C# functions, allowing them to be used in LINQ queries 
        //and maintaining server-side execution.
        [DbFunction(FunctionName = "GetOrderTotal", Schema = "Store")]
        public static int GetOrderTotal(int orderId)
        {
            //The actual code in the C# method doesn’t matter, as it is never executed. The method is used only 
            //to map to the SQL Server function.
            throw new Exception();
        }//Functions can also be mapped using the Fluent API.

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartRecord> ShoppingCartRecords { get; set; }
        

        public DbQuery<CartRecordWithProductInfo> CartRecordWithProductInfos { get; set; }
        public DbQuery<OrderDetailWithProductInfo> orderDetailWithProductInfos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.EmailAddress).HasName("IX_Customers").IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.Property(e => e.ShipDate).HasColumnType("datetime").HasDefaultValueSql("getdate()");

                //Instead of adding the customer id to every query, a global query filter is used to add a 
                //where clause to every query limiting the results to the customer id set on the context. 
                //This filter can be ignored in LINQ using the IgnoreQueryFilters(). 
                entity.HasQueryFilter(e => e.CustomerId == CustomerId);
                entity.Property(e => e.OrderTotal).HasColumnType("money")
                                                .HasComputedColumnSql("Store.GetOrderTotal([Id])");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.UnitCost).HasColumnType("money");
                entity.Property(e => e.LineItemTotal).HasColumnType("money")
                                                    .HasComputedColumnSql("[Quantity]*[UnitCost]");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.UnitCost).HasColumnType("money");
                entity.Property(e => e.CurrentPrice).HasColumnType("money");
                //This Fluent API is used to override the default naming conventions for Owned entities.
                entity.OwnsOne(e => e.Details, pd =>
                    {
                        pd.Property(p => p.Description).HasColumnName(nameof(ProductDetails.Description));
                        pd.Property(p => p.ModelName).HasColumnName(nameof(ProductDetails.ModelName));
                        pd.Property(p => p.ModelNumber).HasColumnName(nameof(ProductDetails.ModelNumber));
                        pd.Property(p => p.ProductImage).HasColumnName(nameof(ProductDetails.ProductImage));
                        pd.Property(p => p.ProductImageLarge).HasColumnName(nameof(ProductDetails.ProductImageLarge));
                        pd.Property(p => p.ProductImageThumb).HasColumnName(nameof(ProductDetails.ProductImageThumb));
                    });
            });

            modelBuilder.Entity<ShoppingCartRecord>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.Property(e => e.Quantity).HasDefaultValue(1);
                entity.HasIndex(e => 
                    new { ShoppingCartRecordId = e.Id, e.ProductId, e.CustomerId }).HasName("IX_ShoppingCart").IsUnique();
                entity.HasQueryFilter(e => e.CustomerId == CustomerId);
            });

            //map the view model to the view
            modelBuilder.Query<CartRecordWithProductInfo>().ToView("CartRecordWithProductInfo", "Store");
            modelBuilder.Query<OrderDetailWithProductInfo>().ToView("OrderDetailWithProductInfo", "Store");
        }
    }
}
