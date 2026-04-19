using IThill_academy.Auth;
using IThill_academy.Data;
using IThill_academy.DTOs;
using IThill_academy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Student> _passwordHasher;
    private readonly EmailService _emailService;
    public AuthService(ApplicationDbContext context, IPasswordHasher<Student> passwordHasher, EmailService emailService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }
    public async Task<Student?> RegisterStudent(RegisterStudentDto dto)
    {
        var existingStudent= await _context.Students.FirstOrDefaultAsync(s=>s.PhoneNumber==dto.PhoneNumber||s.Email==dto.Email);
        if (existingStudent != null)
        {
            throw new InvalidOperationException("Пользователь с таким номером или эмайлом уже сушествует");
        }
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Password = dto.Password,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            IsEmailConfirmed = false,
            CreatedAt=DateTime.UtcNow,
            Role = UserRole.Student
        };
        student.Password=_passwordHasher.HashPassword(student, student.Password);
        _context.Students.Add(student);
        var code =new Random().Next(1000, 9999).ToString();
        var sms = new EmailCode()
        {
            StudentId = student.Id,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(3),
            IsUsed = false
        };
        _context.EmailCodes.Add(sms);
        var email= new EmailDto
        {
            
            To = student.Email,
            Subject = "Код подтверждения регистрации",
            Body = $"Здравствуйте, {student.FirstName}! Ваш код подтверждения: {code}"
        
        } ;
        
        await _emailService.SendConfirmationCode(email);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> VeryfyEmail(VerifyEmailDto dto)
    {
        var student=await _context.Students.FirstOrDefaultAsync(s=>s.Email==dto.Email);
        if (student == null)
            throw new InvalidOperationException("Пользователь с таким email не найден");
        var code = await _context.EmailCodes.FirstOrDefaultAsync(c =>
            c.StudentId == student.Id && c.Code == dto.Code);
        if (code == null) throw new InvalidOperationException("Код недействителен или истёк  ");
        
        if(code.IsUsed) return false;
        
        if(code.ExpiresAt < DateTime.UtcNow) return false;
        code.IsUsed = true;
        
        
        student.IsEmailConfirmed = true;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> Login(LoginDto dto, JwtService jwtService)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == dto.Email);
        if (student == null) throw new InvalidOperationException("Неверный email или email не подтвержден");
        if (!student.IsEmailConfirmed)throw new InvalidOperationException("Email не подтвержден");

        var result = _passwordHasher.VerifyHashedPassword(student, student.Password, dto.Password);
        if (result != PasswordVerificationResult.Success) throw new InvalidOperationException("Неверный пароль!");
        
        var token = jwtService.GenerateToken(student);
        return token;
    }
    public async Task<Student?> GetStudentById(Guid id)
    {
        return await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<PagedResult<Student>> GetAllStudent(int page,int pageSize)
    {
        var totalcount=await _context.Students.CountAsync();
        var students = await _context.Students.OrderBy(s => s.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        
        return new PagedResult<Student>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalcount,
            TotalPages = (int)Math.Ceiling((double)totalcount / pageSize),
            Items = students
        };
    }

    public async Task<Student> DeleteStudent(Guid studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) throw new InvalidOperationException("Пользователь не найден!");
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return student;

    }
    public async Task<Student?> GetByEmail(string email)
    {
        return await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
    }

    
}