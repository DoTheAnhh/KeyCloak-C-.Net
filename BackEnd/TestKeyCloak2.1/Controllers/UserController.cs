using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetUser([FromQuery] string realm)
        {
            var users = await _userService.GetUser(realm);
            return Ok(users);
        }

        // https://localhost:44333/api/user?realm=DemoRealm
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string realm, string id)
        {
            var user = await _userService.GetUserById(realm, id);
            if (user == null)
            {
                return NotFound($"Người dùng với ID {id} không tìm thấy trong realm {realm}.");
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserInsertRequest userDto, [FromQuery] string realm)
        {
            if (userDto == null)
            {
                return BadRequest("Dữ liệu người dùng không hợp lệ.");
            }

            try
            {
                await _userService.CreateUser(userDto, realm);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser([FromQuery] string realm, string id, [FromBody] UserEditRequest userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Dữ liệu người dùng không hợp lệ.");
            }

            try
            {
                await _userService.EditUser(realm, id, userDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromQuery] string realm, string id)
        {
            try
            {
                await _userService.DeleteUser(realm, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }
    }
}