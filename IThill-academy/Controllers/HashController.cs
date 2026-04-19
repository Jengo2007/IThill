using IThill_academy.Data;
using IThill_academy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IThill_academy.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HashController : Controller
{
    private readonly IPasswordHasher<Student> _passwordHasher;
    private readonly ApplicationDbContext _context;

    public HashController(IPasswordHasher<Student> passwordHasher, ApplicationDbContext context)
    {
        _passwordHasher = passwordHasher;
        _context = context;
    }
    [HttpGet("hash")]
    public IActionResult GetHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return BadRequest("Пароль не может быть пустым");

        var student = new Student(); // нужен только для типа
        var hash = _passwordHasher.HashPassword(student, password);

        return Ok(new { PlainPassword = password, HashedPassword = hash });
    }

    
}