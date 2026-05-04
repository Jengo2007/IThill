using System.Net;
using System.Net.Mail;
using IThill_academy.Data;
using IThill_academy.DTOs;
using IThill_academy.Models;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;

namespace IThill_academy.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    public EmailService(IConfiguration config, ApplicationDbContext context)
    {
        _config = config;
        _context = context;
    }
    
       
    public async Task SendConfirmationCode(EmailDto emailDto)
    {
        var fromAddress = _config["EmailSettings:From"];
        if (string.IsNullOrEmpty(fromAddress))
            throw new InvalidOperationException("EmailSettings:From is not configured");

        if (string.IsNullOrEmpty(emailDto.To))
            throw new InvalidOperationException("Recipient email is missing");

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(fromAddress));
        email.To.Add(MailboxAddress.Parse(emailDto.To));
        email.Subject = emailDto.Subject ?? "Код подтверждения";
        email.Body = new TextPart(TextFormat.Plain)
        {
            Text = emailDto.Body ?? "Ваш код подтверждения"
        };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"],
            int.Parse(_config["EmailSettings:Port"]!),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(fromAddress, _config["EmailSettings:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
    public async Task<bool> ResendConfirmationCode(string email)
    {
        var pending = await _context.PendingRegistrations.FirstOrDefaultAsync(p => p.Email == email);
        if (pending == null) return false;

        var newCode = new Random().Next(1000, 9999).ToString();

        pending.Code = newCode;
        pending.ExpiresAt = DateTime.UtcNow.AddMinutes(3);

        await _context.SaveChangesAsync();

        var emailDto = new EmailDto
        {
            To = email,
            Subject = "Новый код подтверждения",
            Body = $"Здравствуйте, {pending.FirstName}! Ваш новый код подтверждения: {newCode}"
        };

        await SendConfirmationCode(emailDto);

        return true;
    }

    
}