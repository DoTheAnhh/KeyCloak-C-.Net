using Microsoft.AspNetCore.Mvc;
using TestKeyCloak2._1.DTO.Realm;
using TestKeyCloak2._1.Service;

namespace TestKeyCloak2._1.Controllers;

[ApiController]
[Route("api/realm")]
public class RealmsController : ControllerBase
{
    private readonly IRealmService _realmService;

    public RealmsController(IRealmService realmService)
    {
        _realmService = realmService;
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateRealm([FromBody] RealmInsertRequest realmRequest)
    {
        await _realmService.CreateRealmAsync(realmRequest);
        return Ok();
    }

    [HttpPut("{realmId}")]
    public async Task<ActionResult> EditRealm(string realmId, [FromBody] RealmEditRequest realmRequest)
    {
        await _realmService.UpdateRealmAsync(realmId, realmRequest);
        return Ok();
    }

    [HttpDelete("{realmId}")]
    public async Task<IActionResult> DeleteRealm(string realmId)
    {
        try
        {
            await _realmService.DeleteRealmAsync(realmId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message }); // Trả về 400 Bad Request nếu có lỗi
        }
    }
}