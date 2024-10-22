using TestKeyCloak2._1.DTO.Realm;

namespace TestKeyCloak2._1.Service;

public interface IRealmService
{
    Task<List<RealmResponse>> GetAllRealmsAsync();
    Task<RealmResponse> GetRealmByIdAsync(string realmId);
    Task CreateRealmAsync(RealmInsertRequest realmRequest);
    Task UpdateRealmAsync(string realmId, RealmEditRequest realmRequest);
    Task DeleteRealmAsync(string realmId);
}