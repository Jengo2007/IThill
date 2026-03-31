using IThill_academy.Auth;
using IThill_academy.Data;
using IThill_academy.DTOs;
using IThill_academy.Models;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Student?> RegisterStudent(RegisterStudentDto dto)
    {
        var existingStudent= await _context.Students.FirstOrDefaultAsync(s=>s.PhoneNumber==dto.PhoneNumber);
        if (existingStudent != null)
        {
            throw new InvalidOperationException("Пользователь с таким номером уже сушествует");
        }
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            IsPhoneConfirmed = false,
            CreatedAt=DateTime.UtcNow,
        };
        _context.Students.Add(student);
        var code =new Random().Next(1000, 9999).ToString();
        var sms = new SmsCode()
        {
            PhoneNumber = student.PhoneNumber,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(3),
            IsUsed = false
        };
        _context.SmsCodes.Add(sms);
        await _context.SaveChangesAsync();
        Console.WriteLine($"[SMS] Код для {student.PhoneNumber}: {code}");
        return student;
    }

    public async Task<bool> VeryfyPhone(VerifyPhoneDto dto)
    {
        var code=await _context.SmsCodes.FirstOrDefaultAsync(s=>s.PhoneNumber==dto.PhoneNumber&&s.Code==dto.Code);
        if (code == null) throw new InvalidOperationException("Код недействителен или истёк  ");
        if(code.IsUsed) return false;
        if(code.ExpiresAt < DateTime.UtcNow) return false;
        code.IsUsed = true;
        
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.PhoneNumber == dto.PhoneNumber);

        if (student != null)
        {
            student.IsPhoneConfirmed = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> Login(LoginDto dto, JwtService jwtService)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.PhoneNumber == dto.PhoneNumber);
        if (student == null) throw new InvalidOperationException("Неверный номер телефона или номер не подтвержден");
        if (!student.IsPhoneConfirmed) return null;
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
}