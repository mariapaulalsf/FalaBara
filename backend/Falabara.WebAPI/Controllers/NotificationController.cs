using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Falabara.Domain.Entities;

namespace Falabara.WebAPI.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _repository;

        public NotificationController(INotificationRepository repo)
        {
            _repository = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var notifs = await _repository.GetByUserIdAsync(userId);
            return Ok(notifs);
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _repository.MarkAllAsReadAsync(userId);
            return Ok();
        }
    }
}