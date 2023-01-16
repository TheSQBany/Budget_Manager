using Budget_Manager.Models;
using Microsoft.AspNetCore.Mvc;

namespace Budget_Manager.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
