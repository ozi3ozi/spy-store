using System;
using System.Collections.Generic;
using System.Text;
using SpyStore.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;


namespace SpyStore.Models.ViewModels
{
    public class OrderDetailWithProductInfo : OrderDetailBase
    {
        public new int Id { get; set; }
        public string Description { get; set; }
        [Display(Name = "Model Name")]
        public string ModelName { get; set; }
        [Display(Name = "Model Number")]
        public string ModelNumber { get; set; }
        public string ProductImage { get; set; }
        public string ProductImageLarge { get; set; }
        public string ProductImageThumb { get; set; }
        [Display(Name = "In Stock")]
        public int UnitsInStock { get; set; }
        [Display(Name = "Price"), DataType(DataType.Currency)]
        public decimal CurrentPrice { get; set; }
        public int CategoryId { get; set; }
        [Display(Name = "Category")]
        public string CategoryName { get; set; }
    }
}
