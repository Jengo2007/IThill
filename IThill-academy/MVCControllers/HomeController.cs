using IThill_academy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.MVCControllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    //GET
    public IActionResult Index()
    {
        return View();
    }
  

    public IActionResult StartLearning()
    {
        if (!User.Identity.IsAuthenticated)
        {
            // Пользователь не авторизован → отправляем на Login
            return RedirectToAction("Login", "AccountMvc");
        }

        // Если авторизован → отправляем на курсы
        return RedirectToAction("Index", "CoursesMvc");
    }
}