using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.RequestsDTO
{
    internal class UserRequestDTO
    {
        public string Login { get; }
        public string Password { get; }
        public string Type { get; }
        public bool IsActive { get; }
        public int? GroupId { get; }

        public UserRequestDTO(string login, string password, string type, bool isActive, int? groupId)
        {
            Login = login;
            Password = password;
            Type = type;
            IsActive = isActive;
            GroupId = groupId;
        }
    }
}
