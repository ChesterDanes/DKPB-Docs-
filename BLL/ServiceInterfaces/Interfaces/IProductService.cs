using BLL.DTOModels;
using BLL.DTOModels.RequestsDTO;
using BLL.DTOModels.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponseDTO>> GetProductsAsync(
            string nameFilter = null,
            string groupNameFilter = null,
            int? groupIdFilter = null,
            bool includeInactive = false,
            string sortBy = "Name",
            bool ascending = true);

        Task AddProductAsync(ProductRequestDTO productRequest);

        Task DeactivateProductAsync(int productId);

        Task ActivateProductAsync(int productId);

        Task DeleteProductAsync(int productId);

        Task AddToCartAsync(int productId, int userId, int amount);

        Task UpdateProductAmountInCartAsync(int productId, int userId, int amount);

        Task RemoveFromCartAsync(int productId, int userId);

        

        
    }
}
