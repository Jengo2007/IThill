using Microsoft.AspNetCore.Mvc;

namespace IThill_academy.MVCControllers;

public class HomeController : Controller
{
    // GET
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