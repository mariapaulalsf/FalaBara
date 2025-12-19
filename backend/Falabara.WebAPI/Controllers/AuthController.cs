using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Falabara.WebAPI.Controllers
{
    [Route("api/auth")] 
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static List<UsuarioModelo> usuarios = new List<UsuarioModelo>();

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] RegistroRequest request)
        {
            if (usuarios.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { mensagem = "Email já cadastrado." });
            }

            var novoUsuario = new UsuarioModelo
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = request.Senha
            };

            usuarios.Add(novoUsuario);

            // Retorna o token e dados para o JS salvar no localStorage
            return Ok(new { 
                token = "token_falso_123456", 
                nome = novoUsuario.Nome, 
                email = novoUsuario.Email 
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = usuarios.FirstOrDefault(u => u.Email == request.Email && u.Senha == request.Senha);

            if (usuario == null)
            {
                return Unauthorized(new { mensagem = "Email ou senha inválidos." });
            }

            return Ok(new { 
                token = "token_falso_123456", 
                nome = usuario.Nome, 
                email = usuario.Email 
            });
        }
    }

    public class UsuarioModelo
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class RegistroRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}