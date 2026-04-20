using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IThill_academy.Auth;
using IThill_academy.DTOs;
using IThill_academy.Extensions;
using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IThill_academy.Controllers;
[ApiController]
[Route("api/[controller]")] // => api/auth
public class AuthController : Controller
{ 
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterStudentDto dto)
    {
        try
        {
            await _authService.RegisterStudent(dto);
            return Ok(new { message = "Регистрация прошла успешно, проверьте email для подтверждения" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPost("Verify")]
    public async Task<ActionResult> Verify(VerifyEmailDto dto)
    {
        try
        {
            var result = await _authService.VerifyEmail(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    
        return Ok("Телефон успешно подтверждён!");
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto, JwtService jwtService)
    {
        try
        {
            var token = await _authService.Login(dto, jwtService);
            if (token == null)
            {
                return BadRequest("Неверный номер телефона или пароль.");
            }

            return Ok(new { Token = token });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера: " + ex.Message);
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> Me()
    {
        var userId = User.GetUserId();     
        if (userId == null)
        {
            return Unauthorized("Не удалось получить Id пользователя из токена.");
        }
        
        var student = await _authService.GetStudentById(userId.Value);
        if (student == null)
        {
            return NotFound("Пользователь не найден.");
        }

        var response = new
        {
            student.Id,
            student.FirstName,
            student.LastName,
            student.PhoneNumber,
            student.Email,
            student.IsEmailConfirmed,
            CreatedAt = student.CreatedAt.ToString("dd.MM.yyyy HH:mm")
        };
        return Ok(response);
    }
    
   
}