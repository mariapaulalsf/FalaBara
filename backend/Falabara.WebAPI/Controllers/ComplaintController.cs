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
        public async Task<IActionResult> Create([FromBody] CreateComplaintRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var command = new CreateComplaintCommand
                {
                    UserId = userId,
                    Title = request.Title,
                    Description = request.Description,
                    Location = request.Location,
                    Neighborhood = request.Neighborhood,
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
            [FromQuery] int page = 1, 
            [FromQuery] int perPage = 10)
        {
            try
            {
                var query = new SearchComplaintsQuery(search, category, status, page, perPage);
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
    }

    public class CreateComplaintRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Neighborhood { get; set; }
        public ComplaintCategory Category { get; set; }
    }
}