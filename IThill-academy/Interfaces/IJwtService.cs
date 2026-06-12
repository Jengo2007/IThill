using IThill_academy.Models;

namespace IThill_academy.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Student student);
}