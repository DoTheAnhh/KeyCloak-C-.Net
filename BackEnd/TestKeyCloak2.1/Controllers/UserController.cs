using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestKeyCloak2._1.DTO;
using TestKeyCloak2._1.Service;

namespace TestKeyCloak2._1.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userService.GetUser();

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserInsertRequest userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Dữ liệu người dùng không hợp lệ.");
            }

            try
            {
                await _userService.CreateUser(userDto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }
        
        [HttpPut("{userId}")]
        public async Task<IActionResult> EditUser(string userId, [FromBody] UserEditRequest userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Dữ liệu người dùng không hợp lệ.");
            }

            try
            {
                await _userService.EditUser(userId, userDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        } 
        
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                await _userService.DeleteUser(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}