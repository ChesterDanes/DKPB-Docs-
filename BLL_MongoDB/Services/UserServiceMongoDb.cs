using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using BLL_MongoDb.Context;
using BLL_MongoDb.Documents;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BLL_MongoDb.Services
{
    public class UserServiceMongoDb : IUserService
    {
        private readonly MongoDbContext _context;

        public UserServiceMongoDb(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDTO> LoginAsync(string login, string password)
        {
            var filter = Builders<UserDocument>.Filter.And(
                Builders<UserDocument>.Filter.Eq(u => u.Login, login),
                Builders<UserDocument>.Filter.Eq(u => u.Password, password),
                Builders<UserDocument>.Filter.Eq(u => u.IsActive, true)
            );

            var user = await _context.Users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
                return null;

            return new UserResponseDTO(
                user.ID,
                user.Login,
                user.Password,
                user.Type,
                user.IsActive,
                user.GroupId
            );
        }

        public Task LogoutAsync()
        {
            // W MongoDb bez sesji/logowania przez tokeny nie mamy "Logout" klasycznego.
            // Jeśli nie zarządzasz tokenami -> może być po prostu pusty Task.CompletedTask.
            return Task.CompletedTask;
        }
    }
}
