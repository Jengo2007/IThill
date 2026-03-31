using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace IThill_academy.Extensions;

public static class UserExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                     ?? user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
                     ?? user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        
        if (string.IsNullOrEmpty(userId)) return null;

        return Guid.TryParse(userId, out var guid) ? guid : null;
    }
}