using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EAD_Web_Service.Services.Impl;

public class TokenService(IConfiguration configuration)
{
    private readonly string _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
    private readonly string _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");

    public string GenerateToken(string id, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = "your-app",
            Audience = "your-app-users",
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, id), 
                new Claim(ClaimTypes.Email, email), 
                new Claim(ClaimTypes.Role, role)]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

}
