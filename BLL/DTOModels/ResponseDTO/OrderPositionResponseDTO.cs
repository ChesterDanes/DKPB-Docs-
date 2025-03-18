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
        public double Price { get; }
        public int Amount { get; }
        public double Value { get; }

        public OrderPositionResponseDTO(string productName, double price, int amount, double value)
        {
            ProductName = productName;
            Price = price;
            Amount = amount;
            Value = value;
        }
    }
}
