using Microsoft.AspNetCore.Mvc;

namespace SportsDiarys.Controllers
{
    public class CalculatorsController : Controller
    {
        [HttpGet]
        public IActionResult Tdee()
        {
            return View();
        }
    }
}