using log4net;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LinkIT.Web
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		private static void RegisterMvc()
		{
			AreaRegistration.RegisterAllAreas();
			MVCFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			MVCRouteConfig.RegisterRoutes(RouteTable.Routes);
			MVCBundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		private static void RegisterWebApi()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}

		protected void Application_Start()
		{
			RegisterWebApi();
			RegisterMvc();
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			// ASP.NET MVC error handling.
			// For ASP.NET WebApi, a separate IExceptionLogger is used. See WebApiConfig for registration.
			var ex = Server.GetLastError();
			if (ex == null)
				return;

			var log = LogManager.GetLogger(GetType());
			log.Fatal("An unhandled exception occurred.", ex);

		}
	}
}