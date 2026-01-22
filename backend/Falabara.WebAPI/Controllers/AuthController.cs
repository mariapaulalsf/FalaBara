using Microsoft.AspNetCore.Mvc;
using MediatR;
using Falabara.Application.Commands.User;
using Falabara.Domain.Entities;
using Falabara.Application.Services; // Importante

namespace Falabara.WebAPI.Controllers
{
    [Route("api/auth")] 
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TokenService _tokenService; // Adicionamos o serviço

        // Injetamos o TokenService no construtor
        public AuthController(IMediator mediator, TokenService tokenService)
        {
            _mediator = mediator;
            _tokenService = tokenService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
        {
            try
            {
                var command = new CreateUserCommand
                {
                    Name = request.Nome,
                    Email = request.Email,
                    Password = request.Senha,
                    Cpf = request.Cpf, 
                    Type = request.Type, 
                    Department = request.Department,
                    FoneNumber = request.FoneNumber,
                };
                await _mediator.Send(command);
                
                return Ok(new { mensagem = "Usuário criado! Faça login para obter o token." });
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var command = new LoginCommand
                {
                    Email = request.Email,
                    Password = request.Senha
                };

                var user = await _mediator.Send(command);

                if (user == null)
                {
                    return Unauthorized(new { mensagem = "Email ou senha inválidos." });
                }

                var token = _tokenService.GenerateToken(user);

                return Ok(new { 
                    token = token, 
                    id = user.Id,     
                    nome = user.Name, 
                    email = user.Email,
                    cpf = user.Cpf,   
                    tipo = user.Type,
                    role = user.Type   
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao processar login: " + ex.Message });
            }
        }
    }
    public class RegistroRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Cpf { get; set; }
        public UserType Type { get; set; }
        public string? Department { get; set; }
        public string? FoneNumber { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}