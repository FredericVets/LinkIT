using LinkIT.Web.Filters;
using LinkIT.Web.Models;
using System.Web.Mvc;

namespace LinkIT.Web.Controllers
{
	public class RequestDataController : Controller
	{
		[ShibbolethAuthorize(Roles = "read")]
		public ActionResult Index()
		{
			var model = new RequestDataModel
			{
				ServerVariables = Request.ServerVariables,
				Headers = Request.Headers
			};

			return View(model);
		}
	}
}