using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
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
		private static void RegisterWebApi() =>
			GlobalConfiguration.Configure(WebApiConfig.Register);

		private static void RegisterMvc()
		{
			AreaRegistration.RegisterAllAreas();
			MVCFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			MVCRouteConfig.RegisterRoutes(RouteTable.Routes);
			MVCBundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		private static void RegisterDependencies()
		{
			var container = new DependencyContainerBuilder().Build();

			// Set the MVC DependencyResolver
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			// Set the WebApi DependencyResolver
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}

		private static void BootStrapLog4Net() =>
			log4net.Config.XmlConfigurator.Configure();

		protected void Application_Start()
		{
			RegisterWebApi();
			RegisterMvc();
			RegisterDependencies();
			BootStrapLog4Net();
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