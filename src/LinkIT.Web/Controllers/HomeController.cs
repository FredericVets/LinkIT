using LinkIT.Web.Filters;
using System.Web.Mvc;

namespace LinkIT.Web.Controllers
{
    public class HomeController : Controller
    {
        [ShibbolethAuthorize(Roles = "read")]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}