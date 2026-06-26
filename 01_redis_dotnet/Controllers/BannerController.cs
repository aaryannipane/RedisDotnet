using Microsoft.AspNetCore.Mvc;
using redis_dotnet.Services;

namespace redis_dotnet.Controllers;

[ApiController]
[Route("api/banner")]

public class BannerController : ControllerBase
{
    private static readonly string BannerKey = "app:banner";
    private readonly RedisService _redisService;

    public BannerController(RedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpPost]
    public async Task<IActionResult> SetBanner([FromBody] string message)
    {
        this._redisService.setString(BannerKey, message);
        return Ok(new { message = "Banner updated successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetBanner()
    {
        var banner = this._redisService.getString(BannerKey);
        if (banner == null)
        {
            return NotFound(new { message = "No banner set" });
        }
        return Ok(new { banner });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBanner()
    {
        var deleted = this._redisService.deleteKey(BannerKey);
        if (!deleted)
        {
            return NotFound(new { message = "No banner to delete" });
        }
        return Ok(new { message = "Banner deleted successfully" });
    }
}