namespace TestKeyCloak2._1.DTO;

public class UserInsertRequest
{
    public string username { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public bool emailVerified { get; set; } = false;
    public bool enabled { get; set; } = true;
    public string firstName { get; set; } = string.Empty;
    public string lastName { get; set; } = string.Empty;
    public string[] requiredActions { get; set; } = Array.Empty<string>();
    public string[] groups { get; set; } = Array.Empty<string>();
}