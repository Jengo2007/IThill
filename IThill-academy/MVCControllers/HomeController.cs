using Microsoft.AspNetCore.Mvc;

namespace IThill_academy.MVCControllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}