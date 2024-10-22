using TestKeyCloak2._1.DTO.User;

namespace TestKeyCloak2._1.Service
{
    public interface IAuthService
    { 
        Task<string> RegisterUser(LoginRegisterRequest loginRegisterRequest, string realm);
        
        Task<string> LoginUser(LoginRegisterRequest loginRegisterRequest,  string realm);
    }
}