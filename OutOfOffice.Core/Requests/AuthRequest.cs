namespace OutOfOffice.Core.Requests;

public class AuthRequest
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}