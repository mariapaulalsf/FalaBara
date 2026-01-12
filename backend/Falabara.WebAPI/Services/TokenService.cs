using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Falabara.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Falabara.Application.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            // Pegamos a chave secreta do appsettings.json
            var secretKey = _configuration["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            // Definimos os dados que vão dentro do Token (Claims)
            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Type.ToString()) 
                }),
                Expires = DateTime.UtcNow.AddHours(8), 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            
            return tokenHandler.WriteToken(token);
        }
    }
}