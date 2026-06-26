using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using redis_dotnet.Requests;
using redis_dotnet.Services;

namespace redis_dotnet.Controllers;

[ApiController]
[Route("")]
public class LoginOtpWithTTLController : ControllerBase
{

    private readonly RedisService _redisService;

    public LoginOtpWithTTLController(RedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpPost("otp")]
    public IActionResult GenerateOtp([FromBody] string phoneNumber)
    {
        string otp = RandomNumberGenerator.GetInt32(1000, 10000).ToString();

        // Store the OTP in Redis with a TTL of 5 minutes
        _redisService.setString(OtpKey(phoneNumber), otp, TimeSpan.FromSeconds(30));
        return Ok(new { message = "OTP generated and stored successfully", otp });
    }

    [HttpPost("verify")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        string? phoneNumber = request.PhoneNumber;
        string? otp = request.Otp;

        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(otp))
        {
            return BadRequest(new { message = "Phone number and OTP are required" });
        }
    
        string? storedOtp = _redisService.getString(OtpKey(phoneNumber));
        if (storedOtp == null)
        {
            return BadRequest(new { message = "Invalid or expired OTP" });
        }

        if (storedOtp != otp)
        {
            return BadRequest(new { message = "Invalid OTP" });
        }

        // OTP is valid, delete it from Redis
        _redisService.deleteKey(OtpKey(phoneNumber));
        return Ok(new { message = "OTP verified successfully" });
    }


    [HttpGet("otp/{phoneNumber}/ttl")]
    public IActionResult OtpTTL(string phoneNumber)
    {
        if(!_redisService.keyExists(OtpKey(phoneNumber)))
        {
            return NotFound(new { message = "No OTP found for this phone number" });
        }

        TimeSpan? ttl = _redisService.getKeyTTL(OtpKey(phoneNumber));
        if (ttl == null)
        {
            return NotFound(new { message = "No TTL found for this OTP" });
        }

        return Ok(new { ttl = ttl.Value.TotalSeconds });
    }

    private string OtpKey(string phoneNumber)
    {
        return $"otp:{phoneNumber}";
    }    
}