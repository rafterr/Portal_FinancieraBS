using System.Diagnostics;
using FinancieraBS.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraBS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Redirigir al login si no está autenticado, o a clientes si ya lo está
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Clientes");
            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
