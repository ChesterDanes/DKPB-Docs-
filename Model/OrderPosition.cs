using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OrderPositions")]
    public class OrderPosition
    {
        [Key]
        public int OrderID { get; set; }
        [Key]
        public int ProductID { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }

        [ForeignKey(nameof(OrderID))]
        public Order? Order { get; set; }
        [ForeignKey(nameof(ProductID))]
        public Product? Product { get; set; }
    }
}
