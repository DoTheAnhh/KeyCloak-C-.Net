using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TestKeyCloak2._1.DTO;
using TestKeyCloak2._1.DTO.User;

namespace TestKeyCloak2._1.Service.impl
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly string _keycloakBaseUrl;
        private readonly string _realm;
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string _adminUsername;
        private readonly string _adminPassword;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _keycloakBaseUrl = _configuration["Keycloak:BaseUrl"]!;
            _realm = _configuration["Keycloak:Realm"]!;
            _clientId = _configuration["Keycloak:ClientId"]!;
            _redirectUri = _configuration["Keycloak:RedirectUri"]!;
            _adminUsername = _configuration["Keycloak:AdminCredentials:Username"]!;
            _adminPassword = _configuration["Keycloak:AdminCredentials:Password"]!;
        }

        private async Task<string> GetAdminToken()
        {
            var url = $"{_keycloakBaseUrl}/realms/master/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", "admin-cli"),
                new KeyValuePair<string, string>("username", _adminUsername),
                new KeyValuePair<string, string>("password", _adminPassword),
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
            return null!;
        }

        public async Task<string> RegisterUser(LoginRegisterRequest loginRegisterRequest, string realm)
        {
            if (loginRegisterRequest == null!)
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

            var url = $"{_keycloakBaseUrl}/admin/realms/{realm}/users";
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

        public async Task<(string AccessToken, string RefreshToken, string Realm)> LoginUser(LoginRegisterRequest loginRegisterRequest)
        {
            var url = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/token";
        
            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", loginRegisterRequest.Username),
                new KeyValuePair<string, string>("password", loginRegisterRequest.Password),
            };
        
            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);
        
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                dynamic tokenData = JsonConvert.DeserializeObject(tokenResponse);
                
                string accessToken = tokenData.access_token;
                string refreshToken = tokenData.refresh_token;
                string userRealm = _realm;
                
                return (accessToken, userRealm, refreshToken);
            }
        
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error: {response.StatusCode} - {errorResponse}");
        }
        
        public async Task<List<UserResponse>> GetAllUsers(string accessToken)
        {
            var url = $"{_keycloakBaseUrl}/admin/realms/{_realm}/users";

            // Thêm token vào Header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<UserResponse>>(jsonResponse);
            }
            else
            {
                throw new HttpRequestException($"Error fetching users: {response.ReasonPhrase}");
            }
        }

        public async Task<string> Logout(string refreshToken)
        {
            var url = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/logout";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return "Logout successful";
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {errorResponse}";
        }

        public async Task<(string AccessToken, string RefreshToken)> ExchangeCodeForToken(string code)
        {
            var url = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _redirectUri)
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                dynamic tokenData = JsonConvert.DeserializeObject(tokenResponse);
        
                string accessToken = tokenData.access_token;
                string refreshToken = tokenData.refresh_token;
        
                return (accessToken, refreshToken);
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error: {response.StatusCode} - {errorResponse} - Request Data: {JsonConvert.SerializeObject(requestData)}");
        }

        public async Task<string> RefreshToken(string refreshToken)
        {
            var url = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);
                
                return tokenData.access_token;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {errorResponse}";
        }

        public async Task<bool> IsLoggedIn(HttpContext httpContext)
        {
            var accessToken = httpContext.Request.Cookies["access_token"];
            return !string.IsNullOrEmpty(accessToken); 
        }
        
        public void RedirectToKeycloak(HttpContext httpContext)
        {
            var redirectUrl = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/auth?" +
                              $"client_id={_clientId}&response_type=code&redirect_uri={_redirectUri}&scope=openid";

            httpContext.Response.Redirect(redirectUrl);
        }
        
        public void RedirectToGoogle(HttpContext httpContext)
        {
            var redirectUrl = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/auth?" +
                              $"client_id={_clientId}&response_type=code&redirect_uri={_redirectUri}&scope=openid&kc_idp_hint=google";

            httpContext.Response.Redirect(redirectUrl);
        }
        
        public void RedirectToGithub(HttpContext httpContext)
        {
            var redirectUrl = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/auth?" +
                              $"client_id={_clientId}&response_type=code&redirect_uri={_redirectUri}&scope=openid&kc_idp_hint=github";

            httpContext.Response.Redirect(redirectUrl);
        }

    }
}
