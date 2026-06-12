using IThill_academy.DTOs;

namespace IThill_academy.Interfaces;

public interface IEmailService
{
    Task SendConfirmationCode(EmailDto emailDto);
    Task<bool> ResendConfirmationCode(string email);
    Task SendEmailAsync(string to, string subject, string body);
}