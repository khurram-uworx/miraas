using Microsoft.AspNetCore.Mvc;
using MiraasWeb.Models;
using MiraasWeb.Views.Home;
using System.Diagnostics;

namespace MiraasWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Calculator()
        {
            this.ViewData["Title"] = "Islamic Inheritance Calculator";
            return View(new IndexModel());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
