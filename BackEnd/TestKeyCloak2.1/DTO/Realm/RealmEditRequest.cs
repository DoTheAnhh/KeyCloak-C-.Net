﻿namespace TestKeyCloak2._1.DTO.Realm;

public class RealmEditRequest
{
    public string realm { get; set; }
    public string displayName { get; set; }
    public bool enabled { get; set; } = true;
}