using Microsoft.AspNetCore.Mvc;
using MediatR;
using Falabara.Application.Queries; 
using Falabara.Application.DTOs;   
using Falabara.Application.Queries.Dashboard;

namespace Falabara.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet] 
        public async Task<IActionResult> GetMetrics()
        {
            var result = await _mediator.Send(new GetDashboardMetricsQuery());
            return Ok(result);
        }
    }
}