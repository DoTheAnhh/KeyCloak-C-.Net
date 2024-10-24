using Microsoft.AspNetCore.Mvc;
using TestKeyCloak2._1.Service;

namespace TestKeyCloak2._1.Controllers
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
        
    }
}