namespace redis_dotnet.Requests;

public class VerifyOtpRequest
{
    public string? PhoneNumber { get; set; }
    public string? Otp { get; set; }
}