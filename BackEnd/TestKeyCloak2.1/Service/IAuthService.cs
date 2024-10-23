using TestKeyCloak2._1.DTO.User;

namespace TestKeyCloak2._1.Service
{
    public interface IAuthService
    { 
        Task<string> RegisterUser(LoginRegisterRequest loginRegisterRequest, string realm);
        
        Task<string> LoginUser(LoginRegisterRequest loginRegisterRequest);

        Task<string> Logout(string refreshToken);
        
        Task<string> RefreshToken(string refreshToken); // Làm mới token

        Task<bool> IsLoggedIn(HttpContext httpContext); // Kiểm tra trạng thái đăng nhập (yêu cầu HttpContext)

        void RedirectToKeycloak(HttpContext httpContext); // Chuyển hướng người dùng đến Keycloak (yêu cầu HttpContext)

        Task<string> ExchangeCodeForToken(string code); // Trao đổi mã lấy access token
    }
}