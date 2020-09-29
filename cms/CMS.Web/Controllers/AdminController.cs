using Microsoft.AspNetCore.Mvc;

namespace WebView
{
    public class AdminController : Controller
    {
        [HttpGet("admin/{*url}")]
        public IActionResult Index()
        {
            return File("/admin/index.html", "text/html");
        }
    }
}