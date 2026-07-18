using _01_redis_dotnet.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redis_dotnet.Services;
using System.Text.Json;

namespace _01_redis_dotnet.Controllers
{
    [Route("")]
    [ApiController]
    public class EmailQueue : ControllerBase
    {
        // Email Queue using redis lists

        private readonly RedisService _redisService;
        private readonly string JobKey = "EMAIL_QUEUE";
        public EmailQueue(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpPost]
        [Route("emails")]
        public IActionResult AddEmailJob([FromBody] AddEmailJobRequest requestBody)
        {
            var job = new
            {
                requestBody.To,
                requestBody.Subject,
                requestBody.Body,
                requestBody.CreatedAt
            };

            _redisService.addJob(JobKey, JsonSerializer.Serialize(job));

            return Ok(new { queue = true, job });
        }

        [HttpGet]
        [Route("emails/process-one")]
        public IActionResult ProcessOneJob()
        {
            string rawJob = _redisService.getJob(JobKey);

            if(rawJob == null)
            {
                return BadRequest(new { message = "No jobs in the queue."});
            }

            var job = JsonSerializer.Deserialize<AddEmailJobRequest>(rawJob);

            // Simulate Email Sending

            return Ok(new { message = "Email Sent", job });
        }
    }
}
