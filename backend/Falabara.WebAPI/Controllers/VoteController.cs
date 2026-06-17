using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Falabara.Application.Commands.Vote;

namespace Falabara.WebAPI.Controllers
{
    [Route("api/votes")]
    [ApiController]
    [Authorize] 
    public class VoteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{complaintId}")]
        public async Task<IActionResult> ToggleVote(Guid complaintId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                var command = new ToggleVoteCommand
                {
                    UserId = userId,
                    ComplaintId = complaintId,
                    IsLike = true 
                };

                var message = await _mediator.Send(command);
                return Ok(new { mensagem = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}