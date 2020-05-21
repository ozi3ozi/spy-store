using System;
using System.Collections.Generic;
using System.Text;
using SpyStore.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SpyStore.Models.ViewModels
{
    public class CartRecordWithProductInfo : ShoppingCartRecordBase
    {
        //ShoppingCartRecordBase entity inherits EntityBase which contains an Id field that is set as the primary key.
        //Entities assigned to DbQuery<T> collection types cannot have a primary key, so the Id field of the base 
        //class must be hidden with a new Id property.
        public new int Id { get; set; }
        //Not supported at this time because DbQuery<T> collection types are not able to use Owned entities. This
        //prevents using the ProductDetails class in place of all of the properties like we did for the Product entity.
        //public ProductDetails Details { get; set; }
        public string Descritpion { get; set; }
        [Display(Name = "Model Number")]
        public string ModelNumber { get; set; }
        [Display(Name = "Model Name")]
        public string ModelName { get; set; }
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
