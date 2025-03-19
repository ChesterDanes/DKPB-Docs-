using BLL.DTOModels.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDTO>> GetOrdersAsync(
            string orderIdFilter = null,
            bool? isPaidFilter = null,
            string sortBy = "ID",
            bool ascending = true);

        Task<OrderPositionResponseDTO> GetOrderPositionAsync(int orderId);

        Task<OrderResponseDTO> GenerateOrderAsync(int userId);

        Task<bool> PayOrderAsync(int orderId, decimal amountPaid);
    }
}
