using IThill_academy.Auth;
using IThill_academy.DTOs;
using IThill_academy.Models;

namespace IThill_academy.Interfaces;

public interface IAuthService
{
     Task RegisterStudent(RegisterStudentDto dto);
     Task<bool> VerifyEmail(VerifyEmailDto dto);
     Task<string?> Login(LoginDto dto, IJwtService jwtService);
     Task<Student?> GetStudentById(Guid id);
     Task<PagedResult<Student>> GetAllStudent(int page, int pageSize, string sortOrder);
     Task<Student> DeleteStudent(Guid studentId);
     Task<Student?> GetByEmail(string email);
     Task<string> GenerateResetPasswordTokenAsync(string email);
     Task<Student> ValidateResetTokenAsync(string token);
     Task ResetPasswordAsync(string token, string newPassword);
}
