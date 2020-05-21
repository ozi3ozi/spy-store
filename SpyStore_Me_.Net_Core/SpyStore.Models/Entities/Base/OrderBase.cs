using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpyStore.Models.Entities.Base
{
    public class OrderBase : EntityBase
    {
        [DataType(DataType.Date)]
        [Display(Name ="Date Ordered")]
        public DateTime OrderDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date Shipped")]
        public DateTime ShipDate { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Total")]
        //The OrderTotal property on the Order class will use the Store.GetOrderTotal SQL Server function 
        //to total the cost of all of the OrderDetail records related to each Order record.
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal OrderTotal { get; set; }
    }
}
