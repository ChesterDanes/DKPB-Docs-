using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.ResponseDTO
{
    public class LoginRequestDTO
    {
        public string Login { get; }
        public string Password { get; }

        public LoginRequestDTO(string login,string password) { Login = login;Password = password; }
    }
}
