using Microsoft.AspNetCore.Mvc;
using redis_dotnet.Services;

namespace redis_dotnet.Controllers;

[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    private readonly RedisService _redisService;
    private readonly MongoService _mongoService;

    public HealthController(RedisService redisService, MongoService mongoService)
    {
        _redisService = redisService;
        _mongoService = mongoService;
    }

    [HttpGet("redis")]
    public async Task<IActionResult> Redis()
    {
        var result = await _redisService.PingAsync();

        return Ok(new
        {
            redis = result
        });
    }

    [HttpGet("mongo")]
    public async Task<IActionResult> Mongo()
    {
        return Ok(new
        {
            mongo = "connected",
            database = _mongoService.GetDatabaseName()
        });
    }

}