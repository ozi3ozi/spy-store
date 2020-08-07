﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpyStore.Dal.EfStructures;
using SpyStore.Dal.Repos.Base;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Dal.Exceptions;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;
using System.Linq.Expressions;

namespace SpyStore.Dal.Repos
{
    public class ShoppingCartRepo : RepoBase<ShoppingCartRecord>, IShoppingCartRepo
    {
        private readonly IProductRepo _productRepo;
        private readonly ICustomerRepo _customerRepo;

        public ShoppingCartRepo(StoreContext context, IProductRepo productRepo, ICustomerRepo customerRepo)
            : base(context)
        {
            _productRepo = productRepo;
            _customerRepo = customerRepo;
        }

        public ShoppingCartRepo(DbContextOptions<StoreContext> options) : base(new StoreContext(options))
        {
            _productRepo = new ProductRepo(Context);
            _customerRepo = new CustomerRepo(Context);
            base.Dispose();
        }

        public override void Dispose()
        {
            _productRepo.Dispose();
            _customerRepo.Dispose();
            base.Dispose();
        }

        public override IEnumerable<ShoppingCartRecord> GetAll() => base.GetAll(x => x.DateCreated);

        public ShoppingCartRecord GetBy(int productId)
            => Table.FirstOrDefault(x => x.ProductId == productId);

        public CartRecordWithProductInfo GetShoppingCartRecord(int id)
            => Context.CartRecordWithProductInfos.FirstOrDefault(x => x.Id == id);

        //The CartRecordWithProductInfo view model does not have a global query filter, so the CustomerId 
        //is passed into the latter for filtering
        public IEnumerable<CartRecordWithProductInfo> GetShoppingCartRecords(int customerId)
            => Context.CartRecordWithProductInfos
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.ModelName);

        public CartWithCustomerInfo GetShoppingCartRecordsWithCustomer(int customerId)
            => new CartWithCustomerInfo()
            {
                cartRecord = GetShoppingCartRecords(customerId).ToList(),
                Customer = _customerRepo.Find(customerId)
            };

        public override int Update(ShoppingCartRecord entity, bool persist = true)
        {
            var product = _productRepo.FindAsNoTracking(entity.ProductId);
            if (product == null)
            {
                throw new SpyStoreInvalidProductException("Unable to find the product");
            }
            return Update(entity, product, persist);
        }

        public int Update(ShoppingCartRecord entity, Product product, bool persist = true)
        {
            if (entity.Quantity <= 0)
            {
                return Delete(entity, persist);
            }
            if (entity.Quantity > product.UnitsInStock)
            {
                throw new SpyStoreInvalidQuantityException("Can't add more products than available in stock");
            }
            var dbRecord = Find(entity.Id);
            if (entity.TimeStamp != null && dbRecord.TimeStamp.SequenceEqual(entity.TimeStamp))
            {
                dbRecord.Quantity = entity.Quantity;
                dbRecord.LineItemTotal = entity.Quantity * product.CurrentPrice;
                return base.Update(dbRecord, persist);
            }
            throw new SpyStoreConcurrencyException("record was changed since it was loaded");
        }

        public override int UpdateRange(IEnumerable<ShoppingCartRecord> entities, bool persist = true)
        {
            int counter = 0;
            foreach (var entity in entities)
            {
                var product = _productRepo.FindAsNoTracking(entity.ProductId);
                counter += Update(entity, product, false);
            }
            return persist ? SaveChanges() : counter;
        }

        public override int Add(ShoppingCartRecord entity, bool persist = true)
        {
            var product = _productRepo.FindAsNoTracking(entity.ProductId);
            if (product == null)
            {
                throw new SpyStoreInvalidProductException("Unable to find the product");
            }
            return Add(entity, product, persist);
        }

        public int Add(ShoppingCartRecord entity, Product product, bool persist = true)
        {
            var item = GetBy(entity.ProductId);
            if (item == null)
            {
                if (entity.Quantity > product.UnitsInStock)
                {
                    throw new SpyStoreInvalidQuantityException("Cannot add more than what's in stock");
                }
                entity.LineItemTotal = entity.Quantity * product.CurrentPrice;
                return base.Add(entity, persist);
            }
            item.Quantity += entity.Quantity;
            return item.Quantity <= 0 ? Delete(item, persist) : Update(item, product, persist);
        }

        public override int AddRange(IEnumerable<ShoppingCartRecord> entities, bool persist = true)
        {
            int counter = 0;
            foreach (var item in entities)
            {
                var product = _productRepo.FindAsNoTracking(item.ProductId);
                counter += Add(item, product, false);
            }
            return persist ? SaveChanges() : counter;
        }

        public int Purchase(int customerId)
        {
            var customerIdParam = new SqlParameter("@customerId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Input,
                Value = customerId
            };
            var orderIdParam = new SqlParameter("@orderId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            try
            {
                Context.Database.ExecuteSqlCommand("EXEC [Store].[PurchaseItemsInCart] @customerId, @orderId out",
                    customerIdParam, orderIdParam);
            }
            catch (Exception ex)
            {
                return -1;
            }
            return (int)orderIdParam.Value;
        }
    }
}
