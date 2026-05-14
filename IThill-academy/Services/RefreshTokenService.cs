using IThill_academy.Data;
using IThill_academy.Models;

namespace IThill_academy.Services;

public class RefreshTokenService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public RefreshToken GenerateRefreshToken(string userId)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            Expires = DateTime.UtcNow.AddDays(7), // живёт неделю
            Created = DateTime.UtcNow,
            UserId = userId
        };

        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        return refreshToken;
    }

    public bool ValidateRefreshToken(string token, string userId)
    {
        var refreshToken = _context.RefreshTokens
            .FirstOrDefault(rt => rt.Token == token && rt.UserId == userId);

        return refreshToken != null && !refreshToken.IsExpired;
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
        if (refreshToken != null)
        {
            _context.RefreshTokens.Remove(refreshToken);
            _context.SaveChanges();
        }
    }
    
    public RefreshToken? GetToken(string token)
    {
        return _context.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
    }
    
    

}