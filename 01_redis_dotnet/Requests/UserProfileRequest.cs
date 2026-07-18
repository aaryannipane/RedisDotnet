namespace redis_dotnet.Requests;

public class UserProfileRequest
{
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}