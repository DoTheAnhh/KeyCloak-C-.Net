namespace TestKeyCloak2._1.DTO.User;

public class UserInfoResponse
{
    public string Sub { get; set; } // ID của người dùng
    public string Username { get; set; }
    public string Realm_id { get; set; } // ID của realm
}