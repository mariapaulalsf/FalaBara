namespace Falabara.Application.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}