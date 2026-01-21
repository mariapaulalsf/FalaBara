using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Falabara.Application.Commands.Complaint;
using Falabara.Application.Queries.Complaint;
using Falabara.Domain.Entities;

namespace Falabara.WebAPI.Controllers
{
    [Route("api/complaints")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ComplaintController(IMediator mediator)
        {
            _mediator = mediator;
        }

       [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateComplaintRequest request)
        {
            try
            {

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { mensagem = "Token inválido ou usuário não identificado." });
                }

                var userId = Guid.Parse(userIdClaim);

                var command = new CreateComplaintCommand
                {
                    UserId = userId,
                    Title = request.Title,
                    Description = request.Description,
                    Location = request.Location,
                    Neighborhood = request.Neighborhood,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Image = request.Image,
                    Category = request.Category
                };

                await _mediator.Send(command);
                return Ok(new { mensagem = "Reclamação postada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                bool isAdmin = role == UserType.Developer.ToString();

                var command = new DeleteComplaintCommand
                {
                    ComplaintId = id,
                    UserId = userId,
                    IsAdmin = isAdmin
                };

                await _mediator.Send(command);
                return Ok(new { mensagem = "Reclamação excluída." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] ComplaintCategory? category,
            [FromQuery] ComplaintStatus? status,
            [FromQuery] string? orderBy,
            [FromQuery] bool onlyMine = false,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            try
            {
                Guid? filtroUsuario = null;
                if (onlyMine)
                {
                    var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdString))
                        return Unauthorized(new { message = "Faça login para ver suas reclamações." });
                    filtroUsuario = Guid.Parse(userIdString);
                }
                var query = new SearchComplaintsQuery(search, category, status, orderBy, filtroUsuario, page, perPage);

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/complaints/{id} 
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetComplaintByIdQuery(id);
            var complaint = await _mediator.Send(query);

            if (complaint == null) return NotFound(new { message = "Reclamação não encontrada." });

            return Ok(complaint);
        }
        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (role != UserType.CityHall.ToString() && role != UserType.Developer.ToString())
                {
                    return StatusCode(403, new { mensagem = "Apenas a Prefeitura pode alterar o status da reclamação." });
                }

                var command = new UpdateComplaintStatusCommand
                {
                    ComplaintId = id,
                    UserId = userId,
                    NewStatus = request.Status,
                    OfficialResponse = request.RespostaOficial
                };

                await _mediator.Send(command);
                return Ok(new { mensagem = "Status atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

    }

    public class CreateComplaintRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Location { get; set; }
        public string? Neighborhood { get; set; }
        public double Latitude { get; set; }  
        public double Longitude { get; set; } 
        public ComplaintCategory Category { get; set; }
        public IFormFile? Image { get; set; } 
    }
    public class UpdateStatusRequest
    {
        public ComplaintStatus Status { get; set; }
        public string? RespostaOficial { get; set; }
    }
}