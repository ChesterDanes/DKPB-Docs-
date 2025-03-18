using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Type Type { get; set; }
        public bool IsActive { get; set; }
        public int? GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public UserGroup? UserGroup { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<BasketPosition>? BasketPositions { get; set; }
    }
}
