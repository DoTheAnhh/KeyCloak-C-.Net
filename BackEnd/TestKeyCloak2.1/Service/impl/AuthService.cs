﻿using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TestKeyCloak2._1.DTO;

namespace TestKeyCloak2._1.Service.impl
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> GetAdminToken()
        {
            var url = "http://localhost:8080/realms/master/protocol/openid-connect/token";

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

        public async Task<string> RegisterUser(LoginRegisterRequest loginRegisterRequest)
        {
            var adminToken = await GetAdminToken();
            if (string.IsNullOrEmpty(adminToken))
            {
                return "Failed to obtain admin token.";
            }

            var url = "http://localhost:8080/admin/realms/DemoRealm/users";
            var newUser = new
            {
                username = loginRegisterRequest.Username,
                enabled = true,
                email = $"{loginRegisterRequest.Username}@example.com", 
                credentials = new[] { new { type = "password", value = loginRegisterRequest.Password, temporary = false } }
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
            return $"Error: {errorResponse}";
        }

        public async Task<string> LoginUser(LoginRegisterRequest loginRegisterRequest)
        {
            var url = "http://localhost:8080/realms/DemoRealm/protocol/openid-connect/token";

            var requestData = new[]
            {
                new KeyValuePair<string, string>("client_id", "democlient"),
                new KeyValuePair<string, string>("username", loginRegisterRequest.Username),
                new KeyValuePair<string, string>("password", loginRegisterRequest.Password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                return tokenResponse; // Return token
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Error: {errorResponse}";
        }
    }
}
