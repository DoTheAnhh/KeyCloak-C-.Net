using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using TestKeyCloak2._1.DTO;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TestKeyCloak2._1.Service.impl
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessToken()
        {
            var tokenEndpoint = "http://localhost:8080/realms/master/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", "admin-cli"),
                new KeyValuePair<string, string>("username", "admin"),
                new KeyValuePair<string, string>("password", "admin"),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                dynamic tokenData = JsonConvert.DeserializeObject(tokenResponse);
                return tokenData.access_token;
            }

            throw new Exception($"Error retrieving access token: {response.ReasonPhrase}");
        }

        public async Task<List<UserResponse>> GetUser(string realm)
        {
            var accessToken = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"http://localhost:8080/admin/realms/{realm}/users");

            var responseData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var userDtos = JsonSerializer.Deserialize<List<UserResponse>>(responseData, options);

            return userDtos!;
        }

        public async Task<UserResponse> GetUserById(string realm, string id)
        {
            var accessToken = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"http://localhost:8080/admin/realms/{realm}/users/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi lấy người dùng: {response.ReasonPhrase}, Chi tiết: {errorContent}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var userDto = JsonSerializer.Deserialize<UserResponse>(responseData, options);

            return userDto!;
        }

        public async Task CreateUser(UserInsertRequest userDto, string realm)
{
    var accessToken = await GetAccessToken();
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    // Tuần tự hóa UserInsertRequest thành JSON
    var jsonContent = JsonSerializer.Serialize(userDto);
    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync($"http://localhost:8080/admin/realms/{realm}/users", content);

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new Exception($"Người dùng đã tồn tại. Chi tiết: {errorContent}");
        }
        throw new Exception($"Lỗi khi tạo người dùng: {response.ReasonPhrase}, Chi tiết: {errorContent}");
    }
}

public async Task EditUser(string realm, string userId, UserEditRequest userDto)
{
    var accessToken = await GetAccessToken();
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    // Tuần tự hóa UserEditRequest thành JSON
    var jsonContent = JsonSerializer.Serialize(userDto);
    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    // Gọi API với ID của người dùng
    var response = await _httpClient.PutAsync($"http://localhost:8080/admin/realms/{realm}/users/{userId}", content);

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new Exception($"Người dùng không tồn tại. Chi tiết: {errorContent}");
        }
        throw new Exception($"Lỗi khi cập nhật người dùng: {response.ReasonPhrase}, Chi tiết: {errorContent}");
    }
}

public async Task DeleteUser(string realm, string userId)
{
    var accessToken = await GetAccessToken();
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await _httpClient.DeleteAsync($"http://localhost:8080/admin/realms/{realm}/users/{userId}");

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception($"Lỗi khi xóa người dùng. Mã trạng thái: {response.StatusCode}, Lỗi: {errorContent}");
    }
}

    }
}
