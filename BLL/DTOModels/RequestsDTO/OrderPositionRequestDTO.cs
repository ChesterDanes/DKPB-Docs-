using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.RequestsDTO
{
    public class OrderPositionRequestDTO
    {
        public string ProductName { get; }
        public decimal Price { get; }
        public int Amount { get; }
        public decimal Value { get; }

        public OrderPositionRequestDTO(string productName, decimal price, int amount, decimal value)
        {
            ProductName = productName;
            Price = price;
            Amount = amount;
            Value = value;
        }
    }
}
