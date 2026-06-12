using IThill_academy.Models;

namespace IThill_academy.Interfaces;

public interface IRefreshTokenService
{
    RefreshToken GenerateRefreshToken(string userId);
    bool ValidateRefreshToken(string token, string userId);
    void RevokeRefreshToken(string token);
    RefreshToken? GetToken(string token);

}