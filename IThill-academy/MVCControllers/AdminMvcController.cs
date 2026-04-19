using IThill_academy.Data;
using IThill_academy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.MVCControllers;
[Authorize(Roles = "Admin")]
public class AdminMvcController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminMvcController(ApplicationDbContext context)
    {
        _context = context;
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
}