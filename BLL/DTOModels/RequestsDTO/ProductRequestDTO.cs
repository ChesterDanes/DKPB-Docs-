using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels
{
    public class ProductRequestDTO
    {
        public string Name { get;}
        public decimal Price { get; }
        public string Image { get;}
        public bool IsActive { get;}
        public int? GroupID { get;}

        public ProductRequestDTO(string name, decimal price, string image, bool isActive, int? groupID)
        {
            Name = name;
            Price = price;
            Image = image;
            IsActive = isActive;
            GroupID = groupID;
        }
    }
}
