using LinkIT.Web.Filters.Api;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace LinkIT.Web
{
	public static class WebApiConfig
	{
		private static void RegisterRouting(HttpConfiguration config)
		{
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
		}

		private static void RegisterFormatters(MediaTypeFormatterCollection formatters)
		{
			// Use caml casing when formatting json.
			var jsonFormatter = formatters.JsonFormatter;
			jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			formatters.Add(new BsonMediaTypeFormatter());
		}

		private static void RegisterGlobalFilters(HttpConfiguration config) =>
			config.Filters.Add(new ValidateModelAttribute());

		public static void Register(HttpConfiguration config)
		{
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

			RegisterRouting(config);
			RegisterFormatters(config.Formatters);
			RegisterGlobalFilters(config);
		}
	}
}