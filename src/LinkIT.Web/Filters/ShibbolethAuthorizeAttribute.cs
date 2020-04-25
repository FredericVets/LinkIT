using LinkIT.Data.Extensions;
using LinkIT.Web.Infrastructure.Shibboleth.Auth;
using System;
using System.Web;
using System.Web.Mvc;

namespace LinkIT.Web.Filters
{
	/// <summary>
	/// To be used in MVC Controllers.
	/// </summary>
	public class ShibbolethAuthorizeAttribute : AuthorizeAttribute
	{
		public ShibbolethAuthorizer Authorizer { get; set; }

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (string.IsNullOrWhiteSpace(Roles))
				throw new ArgumentNullException("No roles are specified!");

			var splitted = Roles.SplitCommaSeparated();

			return Authorizer.IsCurrentUserAuthorized(splitted);
		}
	}
}