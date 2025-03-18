using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw2.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDTO>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Login) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Niepoprawne dane logowania.");
            }

            var user = await _userService.LoginAsync(loginRequest.Login, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized("Błędny login lub hasło.");
            }

            Program.LoggedInUser = user; // to trzeba pozniej zamienic na inne miejsce przechowywania usera


            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task LogoutAsync()
        {
            Program.LoggedInUser = null; // to trzeba pozniej zamienic na inne miejsce przechowywania usera
        }
    }
}
