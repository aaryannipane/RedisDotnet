using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace _01_redis_dotnet.Requests
{
    public class AddEmailJobRequest
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

        [BindNever]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
