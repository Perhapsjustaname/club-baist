namespace ClubBaist.Models;

public class User
{
    public int UserId { get; set; } // Maps to UserId IDENTITY
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Member"; 
}