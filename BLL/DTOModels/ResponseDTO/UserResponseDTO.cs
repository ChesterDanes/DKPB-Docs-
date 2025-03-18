using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.ResponseDTO
{
    public class UserResponseDTO
    {
        public int ID { get; }
        public string Login { get; }
        public string Password { get; }
        public string Type { get;}
        public bool IsActive { get; }
        public int? GroupId { get; }

        public UserResponseDTO(int id,string login, string password, string type, bool isActive, int? groupId)
        {
            ID = id;
            Login = login;
            Password = password;
            Type = type;
            IsActive = isActive;
            GroupId = groupId;
        }
    }
}
