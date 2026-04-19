using IThill_academy.Auth;
using IThill_academy.DTOs;
using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IThill_academy.MVCControllers;

public class AccountMvcController : Controller
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public AccountMvcController(AuthService authService,JwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterStudentDto dto)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Что то пошло не так");
            return View(dto);
        }

        try
        {
            await _authService.RegisterStudent(dto);
            TempData["Message"] = "Регистрация прошла успешно";
            return RedirectToAction("ConfirmEmail", new { email = dto.Email }); 
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return View(dto);
        }
        
    }

    [HttpGet]
    public IActionResult ConfirmEmail(string email)
    {
        return View(new VerifyEmailDto  { Email =email });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmEmail(VerifyEmailDto dto)
    {
        try
        {
            var result = await _authService.VeryfyEmail(dto);
            if (result)
            {
                TempData["Message"] = "Телефон подтвержден. Теперь можно войти.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Код недействителен или истёк.";
            return View(dto);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return View(dto);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var token = await _authService.Login(dto, _jwtService);

                if (token == null)
                {
                    TempData["Error"] = "Телефон не подтвержден.";
                    return View(dto);
                }

                TempData["Message"] = "Вход успешен!";
                // Можно сохранить токен в cookie
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });

                var student = await _authService.GetByEmail(dto.Email);

                if (student != null && student.Role == UserRole.Admin)
                {
                    return RedirectToAction("Students", "AdminMvc");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(dto);
            }
            
        }
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "AccountMvc");
        }

        
}