using CMS.Modules.Content.Abstractions.Model;
using Foundation.Application;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("error")]
        public IActionResult Error(string code)
        {
            var site = SessionHelper.Get<Site>("CurrentSite");
            if (site == null) return Content("");
            
            string folderName = site.IsMobileSite ? site.MobileSiteFolder : site.SiteFolder;
            return View($"~/Views/Content/{folderName}/{code}.cshtml");
        }
    }
}