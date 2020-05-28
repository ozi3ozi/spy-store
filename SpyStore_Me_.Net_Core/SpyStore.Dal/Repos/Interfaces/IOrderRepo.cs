using System;
using System.Collections.Generic;
using System.Text;
using SpyStore.Dal.Repos.Base;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;

namespace SpyStore.Dal.Repos.Interfaces
{
    public interface IOrderRepo : IRepo<Order>
    {
        //The GetOrderHistory method doesn’t need a Customer Id due to the global query filter on the Order entity.
        IList<Order> GetOrderHistoy();
        OrderWithDetailsAndProductInfo GetOneWithDetails(int orderId);
    }
}
