using BLL.DTOModels.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDTO> LoginAsync(string login, string password);
        Task LogoutAsync();
    }
}
