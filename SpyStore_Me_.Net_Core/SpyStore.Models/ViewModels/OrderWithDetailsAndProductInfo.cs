using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
//Automapper is an open source project used for the creation of class instances based on another class instance. 
//The classes can be of the same type or completely different types. The new class instance’s property values are set from the 
//original class instance’s property values, based on a mapping from the source class to the target. The mappings are by default 
//based on reflection but can be customized to meet the applications’ needs.
//https://docs.automapper.org/en/stable/
using AutoMapper;
using SpyStore.Models.Entities;
using SpyStore.Models.Entities.Base;


namespace SpyStore.Models.ViewModels
{
    //This view model inherits OrderBase and also serves as a transport mechanism for the Customer record and the list of 
    //OrderDetailWithProductInfo view model records related to the Order.
    public class OrderWithDetailsAndProductInfo : OrderBase
    {
        private static readonly MapperConfiguration _mapperCfg;
        public Customer Customer { get; set; }
        public IList<OrderDetailWithProductInfo> OrderDetails { get; set; }

        static OrderWithDetailsAndProductInfo()
        {
            //The code creates a new Automapper configuration with one mapping. The mapping is created with the CreateMap call, 
            //and it defines the from (Order) and the to (OrderWithDetailsAndProductInfo) types. After setting the from and to 
            //types, additional customizations can be added. In this case, the mapping will not copy the OrderDetails property from 
            //the source to the target.
            _mapperCfg = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Order, OrderWithDetailsAndProductInfo>()
                        .ForMember(OrderWithDetailsAndProductInfo => OrderWithDetailsAndProductInfo.OrderDetails, 
                                od => od.Ignore());
                });
        }

        public static OrderWithDetailsAndProductInfo Create(Order order, Customer customer, 
            IEnumerable<OrderDetailWithProductInfo> details)
        {
            OrderWithDetailsAndProductInfo viewModel = _mapperCfg.CreateMapper().Map<OrderWithDetailsAndProductInfo>(order);
            viewModel.OrderDetails = details.ToList();
            viewModel.Customer = customer;
            return viewModel;
        }
    }
}
