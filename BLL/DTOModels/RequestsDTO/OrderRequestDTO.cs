using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.RequestsDTO
{
    internal class OrderRequestDTO
    {
        public int UserID { get;}
        public DateTime Date { get;}
        public decimal Value { get;}

        public OrderRequestDTO(int userID, DateTime date, decimal value)
        {
            UserID = userID;
            Date = date;
            Value = value;
        }
    }
}
