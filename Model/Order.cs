using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }

        public bool IsPaid { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public decimal Value { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }
        public ICollection<OrderPosition> OrderPositions { get; set; }
    }
}
