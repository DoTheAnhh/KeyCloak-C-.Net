using Microsoft.AspNetCore.Mvc;
using TestKeyCloak2._1.DTO.User;
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

        // https://localhost:44333/api/auth/register?realm=DemoRealm
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRegisterRequest loginRegisterRequest, [FromQuery] string realm)
        {
            var result = await _authService.RegisterUser(loginRegisterRequest, realm);
            if (result.StartsWith("Error:"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // https://localhost:44333/api/auth/login?realm=DemoRealm
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRegisterRequest loginRegisterRequest, [FromQuery] string realm)
        {
            var result = await _authService.LoginUser(loginRegisterRequest, realm);
            if (result.StartsWith("Error:"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}