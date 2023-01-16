using Microsoft.AspNetCore.Mvc;

namespace Budget_Manager.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
