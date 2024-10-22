using TestKeyCloak2._1.DTO;

namespace TestKeyCloak2._1.Service;

public interface IUserService
{
    Task<List<UserResponse>> GetUser();
    Task<UserResponse> GetUserById(string id);
    Task CreateUser(UserInsertRequest userDto);
    Task EditUser(string userId, UserEditRequest userDto);
    Task DeleteUser(string userId);
}
