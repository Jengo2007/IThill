using System.Security.Claims;
using IThill_academy.DTOs;
using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Authorization;
using IThill_academy.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace IThill_academy.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : Controller
{
    private readonly EnrollmentService _service;

    public EnrollmentsController(EnrollmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(EnrollCourseDto dto)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);
        
        var userId = User.GetUserId();
        
        if (userId == null) return Unauthorized("Не удалось получить Id пользователя из токена.");
        try
        {
            var enrollment = await _service.Enroll(userId.Value, dto.CourseId);

            if (enrollment == null)
            {
                return NotFound("Курс не найден");
            }


            return Ok(new
            {
                enrollment.Id,
                enrollment.StudentId,
                enrollment.CourseId,
                CreatedAt = enrollment.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message); 
        }
    }

    [Authorize]
    [HttpGet("mycourses")]
    public async Task<IActionResult> GetMyCourses()
    {
        var userId = User.GetUserId();
        
        if (userId == null) return Unauthorized("Не удалось получить Id пользователя из токена.");
        
        var courses = await _service.GetMyCourses(userId.Value);
        
        return Ok(courses.Select(c=>new
        {
            c.Title,
            c.Description,
            c.Price,
            c.Duration
        }));
        
    }

    
    

}