
using BCrypt.Net;

public class AuthService
{
    public bool Login(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
