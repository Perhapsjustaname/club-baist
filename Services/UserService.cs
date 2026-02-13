using BCrypt.Net;

public class UserService {
    // To Register
    public void Register(string password) {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        // Save passwordHash to SQL via EF Core...
    }

    // To Login
    public bool Verify(string password, string storedHash) {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}