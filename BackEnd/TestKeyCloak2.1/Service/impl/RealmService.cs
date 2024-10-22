using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TestKeyCloak2._1.DTO.Realm;

namespace TestKeyCloak2._1.Service.impl;

public class RealmService : IRealmService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;

    public RealmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseUri = "http://localhost:8080/admin/realms";
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

    public async Task<List<RealmResponse>> GetAllRealmsAsync()
    {
        var accessToken = await GetAccessToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
        var response = await _httpClient.GetAsync(_baseUri);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error retrieving realms: {response.StatusCode} - {errorResponse}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<RealmResponse>>(jsonResponse);
    }

    public async Task<RealmResponse> GetRealmByIdAsync(string realmId)
    {
        if (string.IsNullOrWhiteSpace(realmId))
        {
            throw new ArgumentException("Realm ID cannot be null or empty.", nameof(realmId));
        }

        var accessToken = await GetAccessToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
        var response = await _httpClient.GetAsync($"{_baseUri}/{realmId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error retrieving realm by ID: {response.StatusCode} - {errorResponse}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<RealmResponse>(jsonResponse);
    }

    public async Task CreateRealmAsync(RealmInsertRequest realmRequest)
    {
        var accessToken = await GetAccessToken();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var jsonContent = JsonConvert.SerializeObject(realmRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_baseUri, content);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRealmAsync(string realmId, RealmEditRequest realmRequest)
    {
        if (string.IsNullOrWhiteSpace(realmId))
        {
            throw new ArgumentException("Realm ID cannot be null or empty.", nameof(realmId));
        }
        
        if (realmRequest == null)
        {
            throw new ArgumentNullException(nameof(realmRequest), "Realm request cannot be null.");
        }

        var accessToken = await GetAccessToken();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var jsonContent = JsonConvert.SerializeObject(realmRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{_baseUri}/{realmId}", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error updating realm: {response.StatusCode} - {errorResponse}");
        }
    }

    public async Task DeleteRealmAsync(string realmId)
    {
        if (string.IsNullOrWhiteSpace(realmId))
        {
            throw new ArgumentException("Realm ID cannot be null or empty.", nameof(realmId));
        }

        var accessToken = await GetAccessToken();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    
        var response = await _httpClient.DeleteAsync($"{_baseUri}/{realmId}");
    
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error deleting realm: {response.StatusCode} - {errorResponse}");
        }
    }
}