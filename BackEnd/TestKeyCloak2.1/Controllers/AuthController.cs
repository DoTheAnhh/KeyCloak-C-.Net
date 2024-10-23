﻿using Microsoft.AspNetCore.Mvc;
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

        // Đăng ký người dùng
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterUser(request, "DemoRealm");
            if (result.Contains("Error"))
            {
                return BadRequest(new { message = result });
            }

            return Ok(new { message = result });
        }

        // Đăng nhập người dùng
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginUser(request);
            if (result.Contains("Error"))
            {
                return Unauthorized(new { message = result });
            }

            return Ok(new { token = result });
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromQuery] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token cannot be null or empty." });
            }

            var result = await _authService.Logout(refreshToken);
            if (result.Contains("Error"))
            {
                return BadRequest(new { message = result });
            }

            return Ok(new { message = "Logout successful." });
        }

        // Redirect đến Keycloak để đăng nhập
        [HttpGet("redirect")]
        public IActionResult RedirectToKeycloak()
        {
            _authService.RedirectToKeycloak(HttpContext);
            return new EmptyResult();
        }

        // Xử lý đổi code lấy token để truy cập
        [HttpPost("exchange")]
        public async Task<IActionResult> ExchangeCodeForToken([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { message = "Code cannot be null or empty." });
            }

            var result = await _authService.ExchangeCodeForToken(code);
            if (result.Contains("Error"))
            {
                return BadRequest(new { message = result });
            }

            return Ok(new { token = result });
        }

        // Làm mới token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            if (string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token cannot be null or empty." });
            }

            var result = await _authService.RefreshToken(refreshTokenRequest.RefreshToken);
            if (result.Contains("Error"))
            {
                return BadRequest(new { message = result });
            }

            return Ok(new { token = result });
        }
    }
}
