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
    public async Task RegisterStudent(RegisterStudentDto dto)
    {
        var existingStudent = await _context.Students
            .FirstOrDefaultAsync(s => s.PhoneNumber == dto.PhoneNumber || s.Email == dto.Email);
        if (existingStudent != null)
            throw new InvalidOperationException("Пользователь с таким номером или email уже существует");

        var code = new Random().Next(1000, 9999).ToString();

        // Проверяем, есть ли уже PendingRegistration для этого email
        var pending = await _context.PendingRegistrations.FirstOrDefaultAsync(p => p.Email == dto.Email);

        if (pending != null)
        {
            // Обновляем существующую запись
            pending.FirstName = dto.FirstName;
            pending.LastName = dto.LastName;
            pending.Password = _passwordHasher.HashPassword(null!, dto.Password);
            pending.PhoneNumber = dto.PhoneNumber;
            pending.Code = code;
            pending.ExpiresAt = DateTime.UtcNow.AddMinutes(3);
        }
        else
        {
            // Создаём новую запись
            pending = new PendingRegistration
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = _passwordHasher.HashPassword(null!, dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(3)
            };
            _context.PendingRegistrations.Add(pending);
        }

        await _context.SaveChangesAsync();

        var email = new EmailDto
        {
            To = dto.Email,
            Subject = "Код подтверждения регистрации",
            Body = $"Здравствуйте, {dto.FirstName}! Ваш код подтверждения: {code}"
        };

        await _emailService.SendConfirmationCode(email);
    }



    public async Task<bool> VerifyEmail(VerifyEmailDto dto)
    {
        var pending = await _context.PendingRegistrations.FirstOrDefaultAsync(p => p.Email == dto.Email);
        if (pending == null)
            throw new InvalidOperationException("Код недействителен или истёк");

        // Проверяем только последний код
        if (pending.Code != dto.Code || pending.ExpiresAt < DateTime.UtcNow)
            return false;

        var student = new Student
        {
            FirstName = pending.FirstName,
            LastName = pending.LastName,
            Password = pending.Password,
            PhoneNumber = pending.PhoneNumber,
            Email = pending.Email,
            IsEmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            Role = UserRole.Student
        };

        _context.Students.Add(student);
        _context.PendingRegistrations.Remove(pending);

        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<string?> Login(LoginDto dto, JwtService jwtService)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == dto.Email||s.Password == dto.Password);
        if (student == null) throw new InvalidOperationException("Неверный email или email не подтвержден или пароль неверный");
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

    public async Task<PagedResult<Student>> GetAllStudent(int page,int pageSize,string sortOrder)
    {
        var query = _context.Students.AsQueryable();

        switch (sortOrder)
        {
            case "name_asc":
                query = query.OrderBy(s => s.FirstName);
                break;
            case "name_desc":
                query = query.OrderByDescending(s => s.FirstName);
                break;
            case "date_asc":
                query = query.OrderBy(s => s.CreatedAt);
                break;
            case "date_desc":
                query = query.OrderByDescending(s => s.CreatedAt);
                break;
            case "newest":
                query = query.OrderByDescending(s => s.CreatedAt); // новые сверху
                break;
            default:
                query = query.OrderBy(s => s.LastName); // сортировка по умолчанию
                break;
        }
        var totalCount=await query.CountAsync();
        var students = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        
        return new PagedResult<Student>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
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