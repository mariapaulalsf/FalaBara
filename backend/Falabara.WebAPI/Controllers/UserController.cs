using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Falabara.Application.Commands.User;
using Falabara.Domain.Entities; 

namespace Falabara.WebAPI.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleString = User.FindFirst(ClaimTypes.Role)?.Value; 
                
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                var userId = Guid.Parse(userIdString);

                // só Desenvolvedor muda Email/Senha
                string? emailParaAtualizar = null;
                string? senhaParaAtualizar = null;

                if (!string.IsNullOrEmpty(request.Email) || !string.IsNullOrEmpty(request.Senha))
                {
                    if (roleString == UserType.Developer.ToString()) 
                    {
                        emailParaAtualizar = request.Email;
                        senhaParaAtualizar = request.Senha;
                    }
                    else
                    {
                        return StatusCode(403, new { mensagem = "Apenas Desenvolvedores podem alterar Email e Senha." });
                    }
                }

                var command = new UpdateUserCommand
                {
                    Id = userId, 
                    Name = request.Nome,
                    Department = request.Department,
                    FoneNumber = request.FoneNumber,
                    NewEmail = emailParaAtualizar,
                    NewPassword = senhaParaAtualizar
                };

                await _mediator.Send(command);

                return Ok(new { mensagem = "Perfil atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
    public class UpdateUserRequest
    {
        public string Nome { get; set; }
        public string? Department { get; set; }
        public string? FoneNumber { get; set; }
        public string? Email { get; set; }  
        public string? Senha { get; set; }  
    }
}