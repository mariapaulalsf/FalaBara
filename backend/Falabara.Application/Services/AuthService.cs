using BCrypt.Net;
using Falabara.Application.DTOs;
using Falabara.Domain.Entities;
using Falabara.Infrastructure.Repositories;

namespace Falabara.Application.Services
{
    public class AuthService
    {
        private readonly UserRepository _repository;

        public AuthService(UserRepository repository)
        {
            _repository = repository;
        }

        public LoginResponseDTO Registrar(RegistroDTO dto)
        {
            if (_repository.EmailExiste(dto.Email))
                throw new Exception("Email já cadastrado");

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

            var usuario = new User
            {
                Name = dto.Nome,
                Email = dto.Email,
                PasswordHash = senhaHash,
                Type = UserType.Population
            };

            _repository.Add(usuario);

            var token = GerarTokenSimples(usuario);

            return new LoginResponseDTO
            {
                Token = token,
                Nome = usuario.Name,
                Email = usuario.Email
            };
        }

        public LoginResponseDTO Login(LoginDTO dto)
        {
            var usuario = _repository.GetByEmail(dto.Email);

            if (usuario == null)
                throw new Exception("Email ou senha inválidos");

            bool senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.PasswordHash);

            if (!senhaValida)
                throw new Exception("Email ou senha inválidos");

            var token = GerarTokenSimples(usuario);

            return new LoginResponseDTO
            {
                Token = token,
                Nome = usuario.Name,
                Email = usuario.Email
            };
        }

        private string GerarTokenSimples(User usuario)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{usuario.Id}:{usuario.Email}:{DateTime.UtcNow.Ticks}"));
        }
    }
}
