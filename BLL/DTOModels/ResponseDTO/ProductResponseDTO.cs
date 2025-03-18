using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.ResponseDTO
{
    public class ProductResponseDTO
    {
        public int ID { get;}
        public string Name { get; }
        public double Price { get; }
        public string Image { get; }
        public bool IsActive { get; }
        public string GroupName { get; }

        public ProductResponseDTO(int id,string name, double price, string image, bool isActive, string groupName)
        {
            ID = id;
            Name = name;
            Price = price;
            Image = image;
            IsActive = isActive;
            GroupName = groupName;
        }
    }
}
