using BLL.DTOModels.RequestsDTO;
using BLL.DTOModels.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces.Interfaces
{
    public interface IProductGroupService
    {
        Task<IEnumerable<ProductGroupResponseDTO>> GetProductGroupsAsync(
            bool topGroups = false,
            int? groupId = null,
            string sortBy = "Name",
            bool ascending = true);

        Task<ProductGroupResponseDTO> AddProductGroupAsync(ProductGroupRequestDTO groupRequest);
    }
}
