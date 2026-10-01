using manutencao_api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
// Lembre-se de adicionar o using da sua pasta de Models
// using manutencao_api.Models; 

namespace manutencao_api.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

            // 1. Define o que vai dentro do Token (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                
                // O "Role" é o que fará o [Authorize(Roles = "Tecnico")] funcionar!
                new Claim(ClaimTypes.Role, usuario.PerfilBase.Nome)
            };

            // (Opcional) Adicionar os Grupos que o usuário faz parte no token
            // foreach (var grupo in usuario.Grupos) { claims.Add(new Claim("Grupo", grupo.Nome)); }

            // 2. Configura os dados do Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8), // Tempo de validade do Token (1 turno de trabalho)
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // 3. Gera e retorna o Token como String
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}