using System;
using System.Collections.Generic;
using System.Text;
using SpyStore.Dal.Repos.Base;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;

namespace SpyStore.Dal.Repos.Interfaces
{
    public interface IShoppingCartRepo : IRepo<ShoppingCartRecord>
    {
        //The first three retrieve data into view models.
        CartRecordWithProductInfo GetShoppingCartRecord(int id);
        IEnumerable<CartRecordWithProductInfo> GetShoppingCartRecords(int customerId);
        CartWithCustomerInfo GetShoppingCartRecordsWithCustomer(int customerId);
        //Gets a single cart record based on Customer Id and Product Id (the Customer Id is provided by the global query filter). 
        ShoppingCartRecord GetBy(int productId);
        //The Update method is used to update an existing ShoppingCartRecord
        int Update(ShoppingCartRecord entity, bool persist = true);
        //The Add method creates a record
        int Add(ShoppingCartRecord entity, bool persist = true);
        //The Purchase method converts the ShoppingCartRecord(s) to an Order and OrderDetailRecord(s).
        int Purchase(int customerId);
    }
}
