using System.Security.Claims;
using System.Text;
using Application.Abstractions.Interfaces;
using Domain.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    public string Create(Guid userId, string email, string role)
    {
        string secretKey = configuration["Jwt:Secret"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
        };
        
        claims.Add(new Claim(ClaimTypes.Role, role));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationMinutes")),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
        };

        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}