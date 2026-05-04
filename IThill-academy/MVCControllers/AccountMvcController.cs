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
    private readonly EmailService _emailService;
    private readonly JwtService _jwtService;
    private readonly ILogger<AccountMvcController> _logger;

    public AccountMvcController(AuthService authService,JwtService jwtService,ILogger<AccountMvcController> logger,EmailService emailService)
    {
        _authService = authService;
        _jwtService = jwtService;
        _logger = logger;
        _emailService = emailService;
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
            _logger.LogWarning("Регистрация не прошла валидацию для {Email}", dto.Email);
            return View(dto);
        }

        try
        {
            await _authService.RegisterStudent(dto);
            _logger.LogInformation("Пользователь {Email} успешно зарегистрирован", dto.Email);
            TempData["Message"] = "\"На ваш email отправлен код подтверждения";
            return RedirectToAction("ConfirmEmail", new { email = dto.Email }); 
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}", dto.Email);
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
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Код подтверждения не совподает или истек ");
            return View(dto);
        }

        try
        {
            var result = await _authService.VerifyEmail(dto);
            if (result)
            {
                _logger.LogInformation("Email подтвержден. Регистрация завершена.");
                TempData["Message"] = "Регистрация прошла успешно! Теперь можно войти";
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
                    _logger.LogWarning("Email не подтвержден");
                    TempData["Error"] = "Email не подтвержден.";
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
                    return RedirectToAction("Index", "CoursesMvc");
                }
                
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(dto);
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> ResendCode(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Email не указан.";
                return RedirectToAction("ConfirmEmail");
            }

            var result = await _emailService.ResendConfirmationCode(email);

            if (!result)
            {
                TempData["Error"] = "Не удалось отправить код. Проверьте email.";
            }
            else
            {
                TempData["Message"] = "Новый код отправлен на вашу почту.";
            }

            // Возвращаем обратно на страницу подтверждения
            return RedirectToAction("ConfirmEmail", new { email });
        }

        
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "AccountMvc");
        }

        
}