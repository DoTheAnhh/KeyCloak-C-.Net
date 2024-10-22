using TestKeyCloak2._1.DTO.Realm;

namespace TestKeyCloak2._1.Service;

public interface IRealmService
{
    Task CreateRealmAsync(RealmInsertRequest realmRequest);
    Task UpdateRealmAsync(string realmId, RealmEditRequest realmRequest);
    Task DeleteRealmAsync(string realmId);
}