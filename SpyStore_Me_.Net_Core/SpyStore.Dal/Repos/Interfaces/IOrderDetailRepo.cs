using System;
using System.Collections.Generic;
using System.Text;
using SpyStore.Dal.Repos.Base;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;

namespace SpyStore.Dal.Repos.Interfaces
{
    public interface IOrderDetailRepo : IRepo<OrderDetail>
    {
        IEnumerable<OrderDetailWithProductInfo> GetOrderDetailWithProductInfoForOrder(int orderId);
    }
}
