using LinkIT.Web.Filters;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace LinkIT.Web
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

			// Attribute based routing.
			config.MapHttpAttributeRoutes();

			// Conventions based routing.
			/*
			// All ids are numeric. So added a numeric constraint.
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional },
				constraints: new { id = @"(\d+)?" }
			);
			*/

			// Use caml casing when formatting json.
			var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
			jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			// Global Filters.
			config.Filters.Add(new ValidateModelAttribute());
		}
	}
}