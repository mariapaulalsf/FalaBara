using BCrypt.Net;

public class AuthService
{
    private readonly UserRepository _repository;

    public AuthService(UserRepository repository)
    {
        _repository = repository;
    }

    public LoginResponseDTO Login(LoginDTO dto)
    {
        var user = _repository.GetByEmail(dto.Email);

        if (user == null)
            throw new Exception("Usuário ou senha inválidos");

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            dto.Password, user.PasswordHash
        );

        if (!validPassword)
            throw new Exception("Usuário ou senha inválidos");

        var token = JwtService.GenerateToken(user);

        return new LoginResponseDTO { Token = token };
    }
}
