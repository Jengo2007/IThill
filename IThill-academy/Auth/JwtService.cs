using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IThill_academy.Models;
using Microsoft.IdentityModel.Tokens;

namespace IThill_academy.Auth;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(Student student)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, student.Id.ToString()),
            new Claim("phoneNumber", student.PhoneNumber),
            new Claim(ClaimTypes.Role, student.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}