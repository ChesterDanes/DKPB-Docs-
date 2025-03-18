using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Model
{
    [Table("ProductGroups")]
    public class ProductGroup
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public ProductGroup? ParentGroup { get; set; }

        public ICollection<ProductGroup>? ChildColletion { get; set; }
        public ICollection<Product>? Products { get; set;}
    }
}
