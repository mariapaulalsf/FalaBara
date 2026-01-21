using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Falabara.Application.Commands.User;
using Falabara.Application.Queries.User; 
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var query = new GetUserByIdQuery(id);
                var user = await _mediator.Send(query);

                if (user == null) 
                    return NotFound(new { message = "Usuário não encontrado." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            try
            {
                var query = new SearchUsersQuery(search, page, perPage);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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