using IThill_academy.Data;
using IThill_academy.DTOs;
using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.MVCControllers;
[Authorize(Roles = "Admin")]
public class AdminMvcController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CourseService _courseService;
    private readonly AuthService _authService;
    private readonly ILogger<AdminMvcController> _logger;

    public AdminMvcController(ApplicationDbContext context, CourseService courseService, ILogger<AdminMvcController> logger, AuthService authService)
    {
        _context = context;
        _courseService = courseService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IActionResult> Students(int page=1,int pageSize=6)
    {
        var result = await _authService.GetAllStudent(page, pageSize);
        return View(result);
    }

    public async Task<IActionResult> Courses(int page=1,int pageSize=5)
    {
        var courses = await _courseService.GetAllCourses( page,pageSize);
        return View(courses);
    }

    public async Task<IActionResult> Enrollments()
    {
        var enrollments = await _context.Enrollments.
            Include(e => e.Student).
            Include(e => e.Course).
            ToListAsync();
        return View(enrollments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourses(CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
        { 
            _logger.LogWarning("Попытка создать курс с некорректными данными");
            return BadRequest(ModelState);
        }

        try
        {
            var course = await _courseService.CreateCourse(dto);
            return PartialView("_CourseRow", course);
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при создании курса: {ex.Message}");
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetCourseById(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound("Курс не найден");
        }

        return Json(new
        {
            id = course.Id,
            title = course.Title,
            description = course.Description,
            price = course.Price,
            duration = course.Duration,
            imagePath = course.ImagePath
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCourse(UpdateCourseDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var course = await _courseService.UpdateCourse(dto);

            _logger.LogInformation("Курс {Title} (ID={Id}) обновлён", course.Title, course.Id);

            return PartialView("_CourseRow", course);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Попытка обновить несуществующий курс (ID={Id})", dto.Id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при обновлении курса (ID={Id})", dto.Id);
            return BadRequest($"Ошибка при обновлении курса: {ex.Message}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCourses(int id)
    {
        try
        {
            var course = await _courseService.DeleteCourse(id);
            _logger.LogInformation("Курс {Title} (ID={Id}) удалён", course.Title, course.Id);

            // можно вернуть JSON для AJAX
            return Json(new { success = true, id = course.Id });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Попытка удалить несуществующий курс (ID={Id})", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при удалении курса (ID={Id})", id);
            return BadRequest($"Ошибка при удалении курса: {ex.Message}");
        }
    }

        
}