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
        public double Price { get; }
        public int Amount { get; }
        public double Value { get; }

        public OrderPositionRequestDTO(string productName, double price, int amount, double value)
        {
            ProductName = productName;
            Price = price;
            Amount = amount;
            Value = value;
        }
    }
}
