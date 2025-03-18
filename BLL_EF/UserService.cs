using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_EF
{
    public class UserService : IUserService
    {
        private readonly WebstoreContext _context;

        public UserService(WebstoreContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDTO> LoginAsync(string login, string password)
        {
            var user = await _context.Users
                .Where(u => u.Login == login && u.Password == password && u.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return new UserResponseDTO(
                user.ID,
                user.Login,
                user.Password,
                user.Type.ToString(),
                user.IsActive,
                user.GroupId
            );
        }

        public async Task LogoutAsync()
        {
            // Tu możesz dodać logikę dotyczącą sesji, np. usuwanie tokenów
            await Task.CompletedTask;
        }
    }
}
