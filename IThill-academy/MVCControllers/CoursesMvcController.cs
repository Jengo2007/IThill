using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Mvc;

namespace IThill_academy.MVCControllers;

public class CoursesMvcController : Controller
{
    private readonly CourseService _courseService;
    private readonly EnrollmentService _enrollmentService;

    public CoursesMvcController(CourseService courseService, EnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }
    public async Task<IActionResult> Index(string? search, string? sort)
    {
        var courses = await _courseService.Getcourses(search, sort);
        return View(courses);
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(Guid studentId, int courseId)
    {
        try
        {
            var response = await _enrollmentService.Enroll(studentId, courseId);
            TempData["Message"] = "Вы успешно записались на курс!";
            
        }
        catch (InvalidOperationException ex)
        {
            TempData["Message"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToAction("Index");
    }

}