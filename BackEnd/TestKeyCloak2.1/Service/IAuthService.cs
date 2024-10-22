using TestKeyCloak2._1.DTO;

namespace TestKeyCloak2._1.Service
{
    public interface IAuthService
    { 
        Task<string> RegisterUser(LoginRegisterRequest loginRegisterRequest);
        
        Task<string> LoginUser(LoginRegisterRequest loginRegisterRequest);
    }
}