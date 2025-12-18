public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDTO
{
    public string Token { get; set; }
}