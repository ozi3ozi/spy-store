using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.Dal.EfStructures;
using SpyStore.Dal.Repos.Base;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;

namespace SpyStore.Dal.Repos
{
    public class OrderRepo : RepoBase<Order>, IOrderRepo
    {
        //The OrderRepo requires an instance of the OrderDetailRepo injected into the constructors. 
        private IOrderDetailRepo _orderDetailRepo;

        public OrderRepo(StoreContext context, IOrderDetailRepo orderDetailRepo) : base(context)
        {
            _orderDetailRepo = orderDetailRepo;
        }
        internal OrderRepo(DbContextOptions<StoreContext> options) : base(options)
        {
            _orderDetailRepo = new OrderDetailRepo(Context);
        }

        public override void Dispose()
        {
            _orderDetailRepo.Dispose();
            base.Dispose();
        }

        //The GetOrderHistory method is a call to GetAll sorted by OrderDate
        public IList<Order> GetOrderHistoy() => GetAll(e => e.OrderDate).ToList();

        //The final method uses two of the view models created earlier in this chapter. The first part of the method retrieves 
        //the Order with the Customer based on the Order Id. If the CustomerId on the order doesn’t match the CustomerId set on 
        //the context (for the global query filter), no order record will be returned, even if the order record exists in the 
        //database. The next part of the method gets all of the OrderDetailsWithProductInfo records using the OrderDetailRepo.
        //Next, an instance of OrderWithDetailsAndProductInfo is created using the static Create method. 
        //Finally, the instance is returned to the calling method.
        public OrderWithDetailsAndProductInfo GetOneWithDetails(int orderId)
        {
            var order = Table.IgnoreQueryFilters().Include(e => e.CustomerNavigation).FirstOrDefault(e => e.Id == orderId);
            if (order == null)
            {
                return null;
            }
            var orderDetailsWithProductInfoForOrder = _orderDetailRepo.GetOrderDetailWithProductInfoForOrder(order.Id);
            var orderWithDetailsAndProductInfo = OrderWithDetailsAndProductInfo.Create(order, 
                                                                                        order.CustomerNavigation, 
                                                                                        orderDetailsWithProductInfoForOrder);
            return orderWithDetailsAndProductInfo;
        }
    }
}
