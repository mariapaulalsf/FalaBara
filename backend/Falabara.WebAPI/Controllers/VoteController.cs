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

        [HttpPost]
        public async Task<IActionResult> ToggleVote([FromBody] ToggleVoteRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var command = new ToggleVoteCommand
                {
                    UserId = userId,
                    ComplaintId = request.ComplaintId,
                    IsLike = request.IsLike
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

    public class ToggleVoteRequest
    {
        public Guid ComplaintId { get; set; }
        public bool IsLike { get; set; }
    }
}