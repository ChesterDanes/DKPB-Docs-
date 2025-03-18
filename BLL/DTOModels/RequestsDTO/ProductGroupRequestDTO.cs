using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.RequestsDTO
{
    public class ProductGroupRequestDTO
    {
        public string Name { get;}
        public int? ParentId { get;}

        public ProductGroupRequestDTO(string  name, int? parentId)
        {
            Name = name;
            ParentId = parentId;
        }
    }
}
