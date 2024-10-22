using TestKeyCloak2._1.DTO;

namespace TestKeyCloak2._1.Service;

public interface IUserService
{
    Task<List<UserResponse>> GetUser(string realm);
    Task<UserResponse> GetUserById(string id, string realm);
    Task CreateUser(UserInsertRequest userDto, string realm);
    Task EditUser(string realm, string userId, UserEditRequest userEditRequest);
    Task DeleteUser(string userId, string realm);
}
