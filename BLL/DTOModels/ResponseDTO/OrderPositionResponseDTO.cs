using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.ResponseDTO
{
    public class OrderPositionResponseDTO
    {
        public string ProductName { get; }
        public decimal Price { get; }
        public int Amount { get; }
        public decimal Value { get; }

        public OrderPositionResponseDTO(string productName, decimal price, int amount, decimal value)
        {
            ProductName = productName;
            Price = price;
            Amount = amount;
            Value = value;
        }
    }
}
