namespace TestKeyCloak2._1.DTO.Realm;

public class RealmInsertRequest
{
    public string realm { get; set; }
    public bool enabled { get; set; } = true;
}