using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.ResponseDTO
{
    public class ProductGroupResponseDTO
    {
        public int ID { get;}
        public string Name { get;}
        public int? ParentId { get;}

        public string? Path { get;}

        public ProductGroupResponseDTO(int id, string name, int? parentId,string? path)
        {
            ID = id;
            Name = name;
            ParentId = parentId;
            Path = path;
        }
    }
}
