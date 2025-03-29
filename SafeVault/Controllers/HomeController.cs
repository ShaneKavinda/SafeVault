using Microsoft.AspNetCore.Mvc;

namespace SafeVault.Controllers
{
    public class HomeController : Controller
    {
        // Action for the home page
        public IActionResult Index()
        {
            return View();
        }

        // Action for an About page
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        // Action for a Contact page
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        // Action for handling errors
        public IActionResult Error()
        {
            return View();
        }
    }
}