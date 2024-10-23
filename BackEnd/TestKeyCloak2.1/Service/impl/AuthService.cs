using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TestKeyCloak2._1.DTO.User;

namespace TestKeyCloak2._1.Service.impl
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        
        private const string KeycloakBaseUrl = "http://localhost:8080";
        private const string Realm = "DemoRealm";
        private const string ClientId = "democlient";
        private const string RedirectUri = "http://localhost:3000/callback"; // URI nhận code từ Keycloak trả về

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy token admin để thực hiện management
        private async Task<string> GetAdminToken()
        {
            var url = $"{KeycloakBaseUrl}/realms/master/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", "admin-cli"),
                new KeyValuePair<string, string>("username", "admin"),
                new KeyValuePair<string, string>("password", "admin"),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                dynamic tokenData = JsonConvert.DeserializeObject(tokenResponse);
                return tokenData.access_token;
            }
            return null;
        }

        // Đăng kí user thông qua Keycloak
        public async Task<string> RegisterUser(LoginRegisterRequest loginRegisterRequest, string realm)
        {
            if (loginRegisterRequest == null)
            {
                return "LoginRegisterRequest cannot be null.";
            }

            if (string.IsNullOrEmpty(realm))
            {
                return "Realm cannot be null or empty.";
            }

            var adminToken = await GetAdminToken();
            if (string.IsNullOrEmpty(adminToken))
            {
                return "Failed to obtain admin token.";
            }

            var url = $"{KeycloakBaseUrl}/admin/realms/{realm}/users";
            var newUser = new
            {
                username = loginRegisterRequest.Username,
                enabled = true,
                email = $"{loginRegisterRequest.Username}@example.com",
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = loginRegisterRequest.Password,
                        temporary = false
                    }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(newUser);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return "User registered successfully.";
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {response.StatusCode} - {errorResponse}";
        }

        public async Task<string> LoginUser(LoginRegisterRequest loginRegisterRequest)
        {
            var url = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", loginRegisterRequest.Username),
                new KeyValuePair<string, string>("password", loginRegisterRequest.Password)
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

                SaveToken(tokenData.access_token);

                return tokenResponse;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {response.StatusCode} - {errorResponse}";
        }
        
        public async Task<string> Logout(string refreshToken)
        {
            var url = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/logout";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                //RemoveToken();

                return "Logout successful";
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {errorResponse}"; // Trả về lỗi nếu có
        }

        // Redirect người dùng đến trang login của Keycloak
        public void RedirectToKeycloak(HttpContext httpContext)
        {
            var redirectUrl = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/auth?" +
                              $"client_id={ClientId}&response_type=code&redirect_uri={RedirectUri}&scope=openid";

            httpContext.Response.Redirect(redirectUrl);
        }

        // Lấy code trả về từ redirect ròi exchange
        public async Task<string> ExchangeCodeForToken(string code)
        {
            var url = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),  // Mã code từ URL
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

                SaveToken(tokenData.access_token);

                return tokenResponse;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {errorResponse}";
        }

        // Lưu token vào cookie hoặc local storage
        private void SaveToken(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            };
            //HttpContext.Current.Response.Cookies.Append("access_token", token, cookieOptions);
        }
        
        // Xóa token
        private void RemoveToken()
        {
            // Nếu lưu token trong cookie thì xóa
            // _httpContextAccessor.HttpContext.Response.Cookies.Delete("access_token");
            // _httpContextAccessor.HttpContext.Response.Cookies.Delete("refresh_token");
        }

        // Làm mới token bằng refresh token
        public async Task<string> RefreshToken(string refreshToken)
        {
            var url = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

                SaveToken(tokenData.access_token);

                return tokenData.access_token;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {errorResponse}";
        }

        // Check người dùng đã login hay chưa dựa vào token lưu trữ
        public async Task<bool> IsLoggedIn(HttpContext httpContext)
        {
            var accessToken = httpContext.Request.Cookies["access_token"];
            return !string.IsNullOrEmpty(accessToken); 
        }
    }
}
