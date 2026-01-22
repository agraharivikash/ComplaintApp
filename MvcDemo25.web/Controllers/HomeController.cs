using Microsoft.AspNetCore.Mvc;

namespace MvcDemo25.web.Controllers;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    public IActionResult About()
    {
        return View();
    }
}

