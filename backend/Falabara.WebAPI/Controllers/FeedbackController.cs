using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Falabara.Application.Commands.Feedback;
using Falabara.Domain.Entities;

namespace Falabara.WebAPI.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var command = new CreateFeedbackCommand
            {
                UserId = userId,
                Message = request.Message,
                Type = request.Type,
                Rating = request.Rating
            };

            await _mediator.Send(command);
            return Ok(new { mensagem = "Obrigado pelo seu feedback!" });
        }
    }

    public class CreateFeedbackRequest
    {
        public string Message { get; set; }
        public FeedbackType Type { get; set; }
        public int Rating { get; set; }
    }
}