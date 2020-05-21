using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpyStore.Models.Entities.Base
{
    public class OrderDetailBase : EntityBase
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required, DataType(DataType.Currency), Display(Name ="Unit Cost")]
        public decimal UnitCost { get; set; }
        [DataType(DataType.Currency), Display(Name ="Total")]
        //instructs EF Core to populate that field from the database and never pass in any updates for that column.
        //The DatabaseGenerated attribute is overridden by the Fluent API code in the next section. I still use the 
        //attribute on the property so other developers know it’s a computed column.
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal LineItemTotal { get; set; }
    }
}
