using System.Net.Http.Headers;
using Newtonsoft.Json;
using TestKeyCloak2._1.DTO;
using TestKeyCloak2._1.Service;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

}
