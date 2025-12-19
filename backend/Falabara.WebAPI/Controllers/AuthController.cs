using Microsoft.AspNetCore.Mvc;
using Falabara.Application.DTOs;
using Falabara.Application.Services;

namespace Falabara.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] RegistroDTO dto)
        {
            try
            {
                var resultado = _authService.Registrar(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            try
            {
                var resultado = _authService.Login(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
