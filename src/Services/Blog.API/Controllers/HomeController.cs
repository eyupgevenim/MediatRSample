using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => new RedirectResult("~/swagger");
    }
}
