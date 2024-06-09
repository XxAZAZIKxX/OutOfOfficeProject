namespace OutOfOffice.Core.Responses;

public class TokenResponse
{
    public string BearerToken { get; set; }
    public ulong UserId { get; set; }
    public string Role { get; set; }
    public long ExpireAt { get; set; }
    public string RefreshToken { get; set; }
}