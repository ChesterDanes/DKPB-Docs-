using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.ResponseDTO
{
    public class OrderResponseDTO
    {
        public int ID { get; }
        public decimal Value { get; }
        public bool IsPaid { get; }
        public DateTime Date { get; }

        public OrderResponseDTO(int id, decimal value, bool isPaid, DateTime date)
        {
            ID = id;
            Value = value;
            IsPaid = isPaid;
            Date = date;
        }
    }
}
