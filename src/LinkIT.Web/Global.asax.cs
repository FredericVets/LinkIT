using Autofac.Integration.WebApi;
using log4net;
using System;
using System.Web.Http;

namespace LinkIT.Web
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		private static void RegisterWebApi() =>
			GlobalConfiguration.Configure(WebApiConfig.Register);

		private static void RegisterDependencies()
		{
			var container = new DependencyContainerBuilder().Build();

			// Set the WebApi DependencyResolver
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}

		private static void BootStrapLog4Net() =>
			log4net.Config.XmlConfigurator.Configure();

		protected void Application_Start()
		{
			RegisterWebApi();
			RegisterDependencies();
			BootStrapLog4Net();

			LogManager.GetLogger(GetType()).Debug("Application started");
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			// General ASP.NET error handling.
			// For ASP.NET WebApi, a separate IExceptionLogger is used. See WebApi dependencies for registration.
			if (Server == null)
				return;

			var ex = Server.GetLastError();
			if (ex == null)
				return;

			var log = LogManager.GetLogger(GetType());
			log.Fatal("An unhandled exception occurred.", ex);
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			log4net.ThreadContext.Properties["clientAddr"] = Request.UserHostAddress;
		}
	}
}