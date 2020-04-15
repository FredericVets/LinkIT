using LinkIT.Data;
using LinkIT.Web.Infrastructure.Shibboleth.Auth;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;

namespace LinkIT.Web.Filters.Api
{
	/// <summary>
	/// To be used in Web Api Controllers.
	/// Use ServiceLocation to get the dependencies. 
	/// Dependencies can not be injected via the constructor since attributes are instantiated by reflection.
	/// Property injection is also a problem since webapi filters are singletons.
	/// See link for more information :
	/// https://autofac.readthedocs.io/en/latest/integration/webapi.html#standard-web-api-filter-attributes-are-singletons
	/// </summary>
	public class ShibbolethAuthorizeAttribute : AuthorizeAttribute
	{
		private ShibbolethAuthorizer GetAuthorizer(IDependencyScope scope) =>
			scope.GetService(typeof(ShibbolethAuthorizer)) as ShibbolethAuthorizer;

		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			if (string.IsNullOrWhiteSpace(Roles))
				throw new ArgumentNullException("No roles are specified!");

			// Get the request lifetime scope so you can resolve services.
			var requestScope = actionContext.Request.GetDependencyScope();
			var splitted = Roles.SplitCommaSeparated();

			return GetAuthorizer(requestScope).IsCurrentUserAuthorized(splitted);
		}
	}
}