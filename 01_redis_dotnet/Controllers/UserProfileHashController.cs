using Microsoft.AspNetCore.Mvc;
using redis_dotnet.Requests;
using redis_dotnet.Services;
using StackExchange.Redis;

namespace redis_dotnet.Controllers;

[ApiController]
[Route("")]
public class UserProfileHashController : ControllerBase
{
    private readonly RedisService _redisService;

    public UserProfileHashController(RedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpPost("user-profile")]
    public IActionResult SetUserProfile([FromBody] UserProfileRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            return BadRequest(new { message = "UserId is required" });
        }

        var userProfile = new HashEntry[]
        {
            new("UserId", request.UserId ?? string.Empty),
            new("Name", request.Name ?? string.Empty),
            new("Email", request.Email ?? string.Empty)
        };

        _redisService.setHash($"user:{request.UserId}:profile", userProfile);
        return Ok(new { message = "User profile stored successfully" });
    }

    [HttpGet("user-profile/{userId}")]
    public IActionResult GetUserProfile(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new { message = "UserId is required" });
        }

        var userProfile = _redisService.getHash($"user:{userId}:profile");
        if (userProfile == null || userProfile.Length == 0)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var response = new
        {
            UserId = userProfile.FirstOrDefault(x => x.Name == "UserId").Value.ToString(),
            Name = userProfile.FirstOrDefault(x => x.Name == "Name").Value.ToString(),
            Email = userProfile.FirstOrDefault(x => x.Name == "Email").Value.ToString()
        };

        return Ok(response);
    }

    [HttpGet("user-profile/{userId}/email")]
    public IActionResult GetUserEmail(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new { message = "UserId is required" });
        }

        var email = _redisService.getHashField($"user:{userId}:profile", "Email");
        if (email == null)
        {
            return NotFound(new { message = "Email not found" });
        }

        return Ok(new { Email = email.Value.ToString() });
    }

    
}