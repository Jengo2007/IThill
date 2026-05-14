using IThill_academy.Data;
using IThill_academy.Extensions;
using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.MVCControllers;

[Authorize]
public class CoursesMvcController : Controller
{
    private readonly CourseService _courseService;
    private readonly EnrollmentService _enrollmentService;
    private readonly ApplicationDbContext _context;

    public CoursesMvcController(CourseService courseService, EnrollmentService enrollmentService, ApplicationDbContext context)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
        _context = context;
    }
    [AllowAnonymous]
    public async Task<IActionResult> Public()
    {
        var courses = await _context.Courses.ToListAsync();
        return View(courses); // Views/CoursesMvc/Public.cshtml
    } 
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "AccountMvc");
        }
        var courses = await _context.Courses.ToListAsync();
        return View(courses);
    }
  
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll( int courseId)
    {
        var studentId = User.GetUserId();
        if (studentId == null)
        {
            TempData["Message"] = "Ошибка: пользователь не авторизован.";
            return RedirectToAction("Index");
        }
        try
        {
            await _enrollmentService.Enroll(studentId.Value,courseId);
            TempData["Message"] = "Вы успешно записались на курс!";
            
        }
        catch (InvalidOperationException ex)
        {
            TempData["Message"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToAction("Index");
    }

    
}