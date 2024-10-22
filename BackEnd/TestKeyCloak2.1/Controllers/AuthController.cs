using Microsoft.AspNetCore.Mvc;
using TestKeyCloak2._1.DTO;
using TestKeyCloak2._1.Service;

namespace TestKeyCloak2._1.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRegisterRequest loginRegisterRequest)
        {
            var result = await _authService.RegisterUser(loginRegisterRequest);
            if (result.StartsWith("Error:"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRegisterRequest loginRegisterRequest)
        {
            var result = await _authService.LoginUser(loginRegisterRequest);
            if (result.StartsWith("Error:"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}