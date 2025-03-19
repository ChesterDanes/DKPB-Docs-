using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Model
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image {  get; set; }
        public bool IsActive { get; set; }
        public int? GroupID { get; set; }
        [ForeignKey (nameof(GroupID))]
        public ProductGroup? ProductGroup { get; set; }
        public ICollection<OrderPosition> OrderPositions { get; }
        public ICollection<BasketPosition> BasketPositions { get; }
    }
}

