using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SpyStore.Models.Entities.Base
{
    //The Owned attribute configures the class to be able to be added to another entity as a property
    [Owned]
    public class ProductDetails
    {
        [MaxLength(3800)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string ModelName { get; set; }
        [MaxLength(50)]
        public string ModelNumber { get; set; }
        [MaxLength(150)]
        public string ProductImage { get; set; }
        [MaxLength(150)]
        public string ProductImageLarge { get; set; }
        [MaxLength(150)]
        public string ProductImageThumb { get; set; }
    }
}
