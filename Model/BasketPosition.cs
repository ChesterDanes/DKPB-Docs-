using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("BasketPositions")]
    public class BasketPosition
    {
        [Key]
        public int ProductID { get; set; }
        [Key]
        public int UserID { get; set; }
        public int Amount { get; set; }
        [ForeignKey(nameof(ProductID))]
        public Product? Product { get; set; }
        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
    }
}
