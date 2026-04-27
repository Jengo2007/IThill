using IThill_academy.Data;
using IThill_academy.DTOs;
using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.MVCControllers;
[Authorize(Roles = "Admin")]
public class AdminMvcController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CourseService _courseService;
    private readonly ILogger<AdminMvcController> _logger;

    public AdminMvcController(ApplicationDbContext context, CourseService courseService, ILogger<AdminMvcController> logger)
    {
        _context = context;
        _courseService = courseService;
        _logger = logger;
    }

    public async Task<IActionResult> Students()
    {
        var students = await _context.Students.ToListAsync();
        return View(students);
    }

    public async Task<IActionResult> Courses()
    {
        var courses = await _context.Courses.ToListAsync();
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
            // возвращаем HTML строки таблицы
            return PartialView("_CourseRow", course);
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при создании курса: {ex.Message}");
        }
    }
        
}